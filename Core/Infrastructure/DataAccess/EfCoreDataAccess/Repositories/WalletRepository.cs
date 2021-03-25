using Core.Domain.Entities;
using Core.Domain.Repositories;
using EfCoreDataAccess;
using EfCoreDataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess.Repositories
{
    public class WalletRepository : EfCoreRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(EfCoreDbContext context) : base(context)
        {
        }
    }
}
