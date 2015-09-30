using System.Reflection;

namespace FrameLog
{
    public interface IHasLoggingReference
    {
        object Reference();
        string ReferenceKey();
    }
}
