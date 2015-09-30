using System;
using FrameLog.Translation.Serializers;

namespace FrameLog.Translation
{
    [Obsolete("\"LegacySerializationManager\" is deprecated and will be removed in future versions of FrameLog. " +
        "Consider switching to \"ValueTranslationManager\", which supports both value serialization and value binding.")]
    public class LegacySerializationManager : ISerializationManager
    {
        public string Serialize(object obj)
        {
            return (obj != null)
                ? obj.ToString()
                : null;
        }

        public bool Compare(object oldObject, object newObject)
        {
            return Equals(oldObject, newObject);
        }
    }
}
