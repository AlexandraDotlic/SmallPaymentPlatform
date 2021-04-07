using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task SaveChangesAsync();
        void ClearTracker();
    }
}
