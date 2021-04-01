using Common.Utils;
using Core.Domain.Repositories;
using Core.Infrastructure.DataAccess.EfCoreDataAccess.Repositories;
using EfCoreDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess
{
    public class EfCoreUnitOfWork : ICoreUnitOfWork
    {
        private readonly EfCoreDbContext Context;
        private IDbContextTransaction Transaction;

        public IWalletRepository WalletRepository { get; }


        public EfCoreUnitOfWork(EfCoreDbContext context)
        {
            Context = context;

            WalletRepository = new WalletRepository(context);
        }

        public async Task BeginTransactionAsync()
        {
            Transaction = await Context.Database.BeginTransactionAsync();
        }
        public Task CommitTransactionAsync()
        {
            return Transaction.CommitAsync();
        }
        public Task RollbackTransactionAsync()
        {
            return Transaction.RollbackAsync();
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException duce)
            {
                //todo: log
                throw duce;
            }
            catch (DbUpdateException due)
            {
                throw due;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        #region IDisposable implementation

        private bool disposedValue = false; // To detect redundant calls


        protected virtual void Dispose(bool disposing)
        {

            if (disposedValue)
            {
                return;
            }
            if (disposing)
            {
                Transaction?.Dispose();
                Context.Dispose();
            }
            disposedValue = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion IDisposable implementation

    }
}
