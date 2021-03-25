using Common.Utils;

namespace Core.Domain.Repositories
{
    public interface ICoreUnitOfWork : IUnitOfWork
    {
        IWalletRepository WalletRepository { get;  }

    }
}
