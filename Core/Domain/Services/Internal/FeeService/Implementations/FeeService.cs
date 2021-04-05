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
        public FeeService()
        {
            
        }
        public async Task<decimal> CalculateTransferFee(
            Wallet wallet, 
            decimal transferAmount, 
            int DaysAfterWalletCreationWithNoFee,
            bool IsFirstTransferFreeInMonth,
            decimal FixedFee,
            int PercentageFee,
            decimal FeeLimit
            )
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
