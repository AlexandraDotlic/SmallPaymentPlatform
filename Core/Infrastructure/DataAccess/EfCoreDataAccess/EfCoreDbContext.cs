using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EfCoreDataAccess
{
    public class EfCoreDbContext : DbContext
    {
        public EfCoreDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}

