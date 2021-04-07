using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess.EntityConfigurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
         
            builder.Property(t => t.Amount).HasPrecision(12, 2);
            builder.HasOne(a => a.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(a => a.WalletJMBG)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
