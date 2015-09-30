﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using FrameLog.History;
using FrameLog.Models;
using FrameLog.Translation.Serializers;
using FrameLog.Helpers;

namespace FrameLog.Logging
{
    public class Recorder<TChangeSet, TPrincipal> : IOven<TChangeSet, TPrincipal>
        where TChangeSet : IChangeSet<TPrincipal>
    {
        private TChangeSet set;
        private IChangeSetFactory<TChangeSet, TPrincipal> factory;
        private IDictionary<object, DeferredObjectChange<TPrincipal>> deferredObjectChanges;
        private ISerializationManager serializer;

        public Recorder(IChangeSetFactory<TChangeSet, TPrincipal> factory)
        {
            this.deferredObjectChanges = new Dictionary<object, DeferredObjectChange<TPrincipal>>(new ReferenceEqualityComparer());
            this.factory = factory;
            this.serializer = null;
        }

        public Recorder(IChangeSetFactory<TChangeSet, TPrincipal> factory, ISerializationManager serializer)
            : this(factory)
        {
            this.serializer = serializer;
        }

        public bool HasChangeSet { get { return set != null; } }

        public void Record(object entity, ChangeType changeType, Func<string> deferredReference, string propertyName, Func<object> deferredValue, Func<object> deferredOldValue, Boolean isManyMany = false)
        {
            ensureChangeSetExists();

            var typeName = ObjectContext.GetObjectType(entity.GetType()).Name;
            var deferredObjectChange = getOrCreateNewDeferredObjectChangeFor(set, entity, changeType, typeName, deferredReference);
            record(deferredObjectChange, propertyName, deferredValue, deferredOldValue, isManyMany);
        }
        private void record(DeferredObjectChange<TPrincipal> deferredObjectChange, string propertyName, Func<object> deferredValue, Func<object> deferredOldValue, Boolean isManyMany = false)
        {
            var deferredValues = deferredObjectChange.FutureValues;
            var propertyChange = getOrCreateNewPropertyChangeFor(deferredObjectChange.ObjectChange, propertyName, isManyMany);
            if (deferredValue != null)
            {
                deferredValues.Store(propertyName, deferredValue, deferredOldValue);
            }
        }

        /// <summary>
        /// The work required to calculate the values for some changes (many-to-many)
        /// is not side-effect free, so we can't do it while the changes from the db
        /// commit are hanging around.
        /// Instead, we record deferred values - pieces of calculation to do once the
        /// database changes are safely saved out. Then, before committing the change
        /// set, we "bake" in the values - we actually do the deferred calculation and
        /// store it in the PropertyChange.
        /// </summary>
        public TChangeSet Bake(DateTime timestamp, TPrincipal author)
        {
            set.Author = author;
            set.TimestampDate = timestamp;

            foreach (var deferredObjectChange in deferredObjectChanges.Values)
            {
                deferredObjectChange.Bake();
            }
            return set;
        }

        private DeferredObjectChange<TPrincipal> getOrCreateNewDeferredObjectChangeFor(TChangeSet set, object entity, ChangeType changeType,  string typeName, Func<string> deferredReference)
        {
            var deferredObjectChange = deferredObjectChanges.SingleOrDefault(doc => ReferenceEquals(doc.Key, entity)).Value;
            if (deferredObjectChange != null)
                return deferredObjectChange;

            var result = factory.ObjectChange();
            result.TypeName = typeName;
            result.ObjectReference = null;
            result.ChangeType = changeType;
            result.ChangeSet = set;
            set.Add(result);

            deferredObjectChange = new DeferredObjectChange<TPrincipal>(result, deferredReference, serializer);
            deferredObjectChanges.Add(entity, deferredObjectChange);
            return deferredObjectChange;
        }
        private IPropertyChange<TPrincipal> getOrCreateNewPropertyChangeFor(IObjectChange<TPrincipal> objectChange, string propertyName, Boolean isManyMany = false)
        {
            var result = objectChange.PropertyChanges.SingleOrDefault(pc => pc.PropertyName == propertyName);
            if (result == null)
            {
                result = factory.PropertyChange();
                result.ObjectChange = objectChange;
                result.PropertyName = propertyName;
                result.Value = null;
                result.ValueAsInt = null;
                result.OldValue = null;
                result.OldValueAsInt = null;
                result.IsManyMany = isManyMany;
                objectChange.Add(result);
            }
            return result;
        }
        private void ensureChangeSetExists()
        {
            if (set == null)
                set = factory.ChangeSet();
        }
    }
}
