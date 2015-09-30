using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace FrameLog.Logging.ValuePairs
{
    public interface IValuePair
    {
        bool HasChanged { get; }
        string PropertyName { get; }
        object OriginalValue { get; }
        object NewValue { get; }
        EntityState State { get; }
    }
}
