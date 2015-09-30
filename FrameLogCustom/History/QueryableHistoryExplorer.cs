using FrameLog.Contexts;
using FrameLog.Exceptions;
using FrameLog.Helpers;
using FrameLog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using FrameLog.Translation;
using FrameLog.Translation.Binders;

namespace FrameLog.History
{
    /// <summary>
    /// This class reconstitutes logs into sequences of values and objects, accompanied
    /// by timestamp and author data.
    /// </summary>
    public class QueryableHistoryExplorer<TChangeSet, TPrincipal> : HistoryExplorer<TChangeSet, TPrincipal> 
        where TChangeSet : IChangeSet<TPrincipal>
    {
        private IHistoryContext<TChangeSet, TPrincipal> db;
        private IBindManager binder;
        private HistoryExplorerCloneStrategies cloneStrategy;

        public QueryableHistoryExplorer(IHistoryContext<TChangeSet, TPrincipal> db, IBindManager binder = null, HistoryExplorerCloneStrategies cloneStrategy = HistoryExplorerCloneStrategies.Default)
            : base(db, binder, cloneStrategy)
        {
            this.db = db;
            this.binder = (binder ?? new ValueTranslationManager(db));
            this.cloneStrategy = cloneStrategy;
        }

        /// <summary>
        /// Retrieve the values that a single property has gone through, most recent
        /// first (descending date order).
        /// </summary>
        public virtual IQueryable<IObjectChange<TPrincipal>> ChangesOf<TModel, TValue>(TModel model, Expression<Func<TModel, TValue>> property)
        {
            string propertyName = property.GetPropertyName();
            string propertyPrefix = propertyName + ".";            

            return changesToAsQueryable(model)
                .SelectMany(o => o.PropertyChanges)
                .Where(p => p.PropertyName == propertyName || p.PropertyName.StartsWith(propertyPrefix))
                .GroupBy(p => p.ObjectChange)
                .Select(g => new FilteredObjectChange<TPrincipal>(g.Key, g));
        }

        /// <summary>
        /// Rehydrates versions of the object, one for each logged change to the object,
        /// most recent first (descending date order).
        /// </summary>
        public virtual IQueryable<IObjectChange<TPrincipal>> ChangesOf<TModel>(TModel model)
            where TModel : class
        {
            return changesToAsQueryable(model)
                .OrderBy(c => c.ChangeSet.TimestampDate);
        }
        /// <summary>
        /// Rehydrates versions of the object, one for each logged change to the object,
        /// most recent first (descending date order).
        /// </summary>
        public virtual IQueryable<IObjectChange<TPrincipal>> ChangesOfByReference<TModel>(string reference)
            where TModel : class
        {
            return changesToAsQueryable<TModel>(reference)
                .OrderByDescending(t => t.ChangeSet.TimestampDate);            
        }

        /// <summary>
        /// Returns the timestamp and author information for the creation of the object.
        /// If the creation of the object is not recorded in the log, throws a
        /// CreationDoesNotExistInLogException.
        /// </summary>
        /// TODO: Is usefull
        public virtual IChange<TModel, TPrincipal> ChangeOfCreation<TModel>(TModel model)
        {
            var firstChange = changesToAsQueryable(model).FirstOrDefault(t => t.ChangeType == ChangeType.Add);
            if (firstChange == null)
                throw new CreationDoesNotExistInLogException(model);
            else
                return Change.FromObjectChange(model, firstChange);
        }


        /// <summary>
        /// Returns the timestamp and author information for the creation of the object.
        /// If the creation of the object is not recorded in the log, throws a
        /// CreationDoesNotExistInLogException.
        /// </summary>
        public virtual IObjectChange<TPrincipal> ChangesOfCreation<TModel>(TModel model)
        {
            return changesToAsQueryable(model).FirstOrDefault(t => t.ChangeType == ChangeType.Add);
        }


        /// <summary>
        /// Returns all IObjectChanges that are relevant to this object, earliest first
        /// </summary>
        protected virtual IOrderedQueryable<IObjectChange<TPrincipal>> changesToAsQueryable<TModel>(TModel model)
        {
            string reference = db.GetReferenceForObject(model);
            return changesToAsQueryable<TModel>(reference);
        }

        /// <summary>
        /// Returns all IObjectChanges that are relevant to the object identified by this reference, earliest first
        /// </summary>
        protected virtual IOrderedQueryable<IObjectChange<TPrincipal>> changesToAsQueryable<TModel>(string reference)
        {
            string typeName = typeof(TModel).Name;
            var changes = db.ObjectChanges
                .Include(t => t.PropertyChanges)
                .Include(t => t.ChangeSet)
                .Where(o => o.TypeName == typeName)
                .Where(o => o.ObjectReference == reference)
                .OrderBy(o => o.ChangeSet.TimestampDate);
            return changes;
        }

    }
}
