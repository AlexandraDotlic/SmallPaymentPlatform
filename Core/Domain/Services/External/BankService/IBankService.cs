using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services.External.BankService
{
    public interface IBankService
    {
        Task<bool> CheckStatus(string jmbg, string bankPIN);
        Task<BankResponseDTO> Deposit(string jmbg, string bankPIN, decimal amount);
        Task<BankResponseDTO> Withdraw(string jmbg, string bankPIN, decimal amount);

    }
}
