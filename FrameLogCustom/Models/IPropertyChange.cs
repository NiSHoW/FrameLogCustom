
namespace FrameLog.Models
{
    public interface IPropertyChange<TPrincipal>
    {
        IObjectChange<TPrincipal> ObjectChange { get; set; }
        string PropertyName { get; set; }
        string Value { get; set; }
        int? ValueAsInt { get; set; }
        bool IsManyMany { get; set; }
        string OldValue { get; set; }
        int? OldValueAsInt { get; set; }        
    }
}
