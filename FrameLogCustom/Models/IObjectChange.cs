using System.Collections.Generic;
using FrameLog.History;

namespace FrameLog.Models
{
    public interface IObjectChange<TPrincipal>
    {
        IChangeSet<TPrincipal> ChangeSet { get; set; }
        IEnumerable<IPropertyChange<TPrincipal>> PropertyChanges { get; }
        void Add(IPropertyChange<TPrincipal> propertyChange);

        ChangeType ChangeType { get; set; }
        string TypeName { get; set; }
        string ObjectReference { get; set; }        
    }
}
