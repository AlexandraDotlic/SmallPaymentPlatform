using Core.Domain.Entities;
using Core.Domain.Services.External.BankService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services.Internal.BankRoutinService.Interface
{
    public interface IBankRoutingService
    {
        Task<BankResponse> CheckStatus(string jmbg, string bankPIN, BankType bankType);
        Task<BankResponse> Deposit(string jmbg, string bankPIN, decimal amount, BankType bankType);
        Task<BankResponse> Withdraw(string jmbg, string bankPIN, decimal amount, BankType bankType);
    }
}
