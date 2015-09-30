using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameLog.Models
{
    public interface IChangeSet<TPrincipal>
    {
        IEnumerable<IObjectChange<TPrincipal>> ObjectChanges { get; }
        void Add(IObjectChange<TPrincipal> objectChange);

        DateTimeOffset TimestampDate { get; set; }

        TPrincipal Author { get; set; }
    }
}
