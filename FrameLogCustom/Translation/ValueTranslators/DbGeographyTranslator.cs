using System;
using System.Data.Entity.Spatial;
using FrameLog.Translation.Binders;
using FrameLog.Translation.Serializers;

namespace FrameLog.Translation.ValueTranslators
{
    public class DbGeographyTranslator : IBinder, ISerializer, IComparator
    {
        public bool Supports(Type type)
        {
            return type == typeof(DbGeography);
        }

        public bool Compare(object oldValue, object newValue)
        {
            try
            {
                return ((DbGeography) oldValue).SpatialEquals((DbGeography) newValue);
            }
            catch (Exception ex) { }

            return Equals(oldValue, newValue);
        }

        public object Bind(string raw, Type type, object existingValue)
        {
            try
            {
                if (raw == null)
                    return null;

                return DbGeography.FromText(raw);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public string Serialize(object obj)
        {
            var geo = obj as DbGeography;
            if(geo != null) return geo.AsText();
            return (obj != null ? obj.ToString() : null);
        }


    }
}
