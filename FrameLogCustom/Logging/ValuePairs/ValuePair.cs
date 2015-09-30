using FrameLog.Logging;
using System;
using System.Data.Entity;
using FrameLog.Translation.Serializers;

namespace FrameLog.Logging.ValuePairs
{
    internal class ValuePair : IValuePair
    {
        protected readonly object originalValue;
        protected readonly object newValue;
        protected readonly string propertyName;
        protected readonly EntityState state;
        protected readonly ISerializationManager serializer;

        internal ValuePair(object originalValue, object newValue, string propertyName, EntityState state, ISerializationManager serializer)
        {
            this.originalValue = get(originalValue);
            this.newValue = get(newValue);
            this.propertyName = propertyName;
            this.state = state;
            this.serializer = serializer;
        }

        private object get(object value)
        {
            if (value is DBNull)
                return null;
            return value;
        }

        internal IChangeType Type
        {
            get
            {
                var value = originalValue ?? newValue;
                return value.GetChangeType();
            }
        }

        public bool HasChanged
        {
            get
            {
                return state == EntityState.Added
                    || state == EntityState.Deleted
                    || !serializer.Compare(newValue, originalValue);
            }
        }

        public string PropertyName
        {
            get { return propertyName; }
        }

        public object NewValue
        {
            get { return newValue; }
        }

        public object OriginalValue
        {
            get { return originalValue; }
        }

        public EntityState State
        {
            get { return state; }
        }
    }
}