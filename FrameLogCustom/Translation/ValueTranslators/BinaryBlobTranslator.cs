using System;
using System.Linq;
using FrameLog.Translation.Binders;
using FrameLog.Translation.Serializers;

namespace FrameLog.Translation.ValueTranslators
{
    public class BinaryBlobTranslator : IBinder, ISerializer, IComparator
    {
        public bool Supports(Type type)
        {
            return typeof(byte[]).IsAssignableFrom(type);
        }

        public object Bind(string raw, Type type, object existingValue)
        {
            if (raw == null)
                return null;

            return Convert.FromBase64String(raw);
        }

        public string Serialize(object obj)
        {
            var blob = (obj as byte[]);
            if (blob == null)
                return null;

            byte[] encode = null;
            try
            {
                encode = Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(blob));
            }
            catch (Exception ex)
            {
                encode = blob;
            }

            return Convert.ToBase64String(encode);
        }

        public bool Compare(object oldValue, object newValue)
        {
            try { 
                if(oldValue != null && newValue != null)
                    return ((byte[]) oldValue).SequenceEqual((byte[]) newValue);
            }
            catch (Exception ex) { }

            return Equals(oldValue, newValue);
        }
    }
}
