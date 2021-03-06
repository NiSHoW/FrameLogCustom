﻿using FrameLog.Logging;
using System;
using System.Data.Entity;
using FrameLog.Translation.Serializers;

namespace FrameLog.Logging.ValuePairs
{
    internal class ValuePair : IValuePair
    {
        protected readonly Func<object> originalValue;
        protected readonly Func<object> newValue;
        protected readonly string propertyName;
        protected readonly EntityState state;
        protected readonly ISerializationManager serializer;

        internal ValuePair(Func<object> originalValue, Func<object> newValue, string propertyName, EntityState state, ISerializationManager serializer)
        {
            this.originalValue = checkDbNull(originalValue);
            this.newValue = checkDbNull(newValue);
            this.propertyName = propertyName;
            this.state = state;
            this.serializer = serializer;
        }

        private Func<object> checkDbNull(Func<object> value)
        {
            return () =>
            {
                var obj = (value != null ? value() : null);
                if (obj is DBNull)
                    return null;
                return obj;
            };
        }

        internal IChangeType Type
        {
            get
            {
                var value = originalValue() ?? newValue();
                return value.GetChangeType();
            }
        }

        public bool HasChanged
        {
            get
            {
                return state == EntityState.Added
                    || state == EntityState.Deleted
                    || !serializer.Compare(newValue(), originalValue());
            }
        }

        public string PropertyName
        {
            get { return propertyName; }
        }

        public Func<object> NewValue
        {
            get { return newValue; }
        }

        public Func<object> OriginalValue
        {
            get { return originalValue; }
        }

        public EntityState State
        {
            get { return state; }
        }
    }
}