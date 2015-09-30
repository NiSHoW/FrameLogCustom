using System;
using System.Data.Entity;
using FrameLog.Models;

namespace FrameLog.Contexts
{
    public abstract partial class DbContextAdapter<TChangeSet, TPrincipal> 
        : ObjectContextAdapter<TChangeSet, TPrincipal>
        where TChangeSet : IChangeSet<TPrincipal>
    {
        private readonly DbContext context;

        public DbContextAdapter(DbContext context)
            : base(((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext)
        {
            this.context = context;
        }

        public override int SaveAndAcceptAllChanges()
        {
            return context.SaveChanges();
        }
    }
}
