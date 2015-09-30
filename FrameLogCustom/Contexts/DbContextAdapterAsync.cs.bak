using System;
using System.Threading;
using System.Threading.Tasks;
using FrameLog.Models;

namespace FrameLog.Contexts
{
    public abstract partial class DbContextAdapter<TChangeSet, TPrincipal>
        where TChangeSet : IChangeSet<TPrincipal>
    {
        public override async Task<int> SaveAndAcceptAllChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
