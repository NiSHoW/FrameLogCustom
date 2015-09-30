using System;
using FrameLog.Translation.ValueTranslators;

namespace FrameLog.Translation.Serializers
{
    public interface ISerializer : IValueTranslator
    {
        string Serialize(object obj);
    }

    public interface IComparator : IValueTranslator
    {
        Boolean Compare(object oldValue, object newValue);
    }
}
