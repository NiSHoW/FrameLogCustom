using FrameLog.Models;
using System.Data.Entity.Core.Objects;
using System.Threading;
using System.Threading.Tasks;

namespace FrameLog.Contexts
{
    public abstract partial class ObjectContextAdapter<TChangeSet, TPrincipal> 
        where TChangeSet : IChangeSet<TPrincipal>
    {
        public virtual async Task<int> SaveAndAcceptAllChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await context.SaveChangesAsync(SaveOptions.AcceptAllChangesAfterSave, cancellationToken);
        }
    }
}
