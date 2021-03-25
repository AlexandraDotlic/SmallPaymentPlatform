using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Infrastructure.DataAccess.EfCoreDataAccess.EntityConfigurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.JMBG);
            builder.Property(w => w.JMBG).HasMaxLength(13);
            builder.Property(w => w.FirstName).HasMaxLength(100);
            builder.Property(w => w.LastName).HasMaxLength(100);
            builder.Property(w => w.BankPIN).HasMaxLength(4);
            builder.Property(w => w.BankAccountNumber).HasMaxLength(18);
            builder.Property(w => w.PASS).HasMaxLength(6);
        }
    }
}
