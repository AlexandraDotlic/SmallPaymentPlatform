using Core.Domain.Entities;
using Core.Domain.Services.Internal.FeeService.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services.Internal.FeeService.Implementations
{
    public class FeeService : IFeeService
    {
        private readonly int DaysAfterWalletCreationWithNoFee;
        private readonly bool IsFirstTransferFreeInMonth;
        private readonly decimal FixedFee;
        private readonly int PercentageFee;
        private readonly decimal FeeLimit;
        public FeeService(IConfiguration configuration)
        {
            DaysAfterWalletCreationWithNoFee = Int32.Parse(configuration["DaysAfterWalletCreationWithNoFee"]);
            IsFirstTransferFreeInMonth = bool.Parse(configuration["IsFirstTransferFreeInMonth"]);
            FixedFee = decimal.Parse(configuration["FixedFee"]);
            PercentageFee = Int32.Parse(configuration["PercentageFee"]);
            FeeLimit = decimal.Parse(configuration["FeeLimit"]);
        }
        public async Task<decimal> CalculateTransferFee(Wallet wallet, decimal transferAmount)
        {
            if (wallet.WalletCreationTime.Date.AddDays(DaysAfterWalletCreationWithNoFee) > DateTime.Now.Date)
            {
                return 0m;
            }

            if (IsFirstTransferFreeInMonth && 
                (wallet.LastTransferDateTime.Month != DateTime.Now.Month || wallet.LastTransferDateTime.Year != DateTime.Now.Year))
            {
                return 0m;
            }

            if (transferAmount < FeeLimit)
            {
                return FixedFee;
            }
            else
            {
                return (transferAmount * PercentageFee) / 100m;
            }
        }
    }
}
