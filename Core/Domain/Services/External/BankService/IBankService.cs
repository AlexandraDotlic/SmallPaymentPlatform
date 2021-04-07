using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services.External.BankService
{
    public interface IBankService
    {
        Task<BankResponse> CheckStatus(string jmbg, string bankPIN);
        Task<BankResponse> Deposit(string jmbg, string bankPIN, decimal amount);
        Task<BankResponse> Withdraw(string jmbg, string bankPIN, decimal amount);

    }
}
