using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services.Internal.FeeService.Interface
{
    public interface IFeeService
    {
        Task<decimal> CalculateTransferFee(Wallet wallet, decimal transferAmount);
    }
}
