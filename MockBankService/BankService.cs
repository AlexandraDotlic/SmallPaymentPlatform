using Core.Domain.Services.External.BankService;
using System;
using System.Threading.Tasks;

namespace MockBankService
{
    public class BankService : IBankService
    {
        public BankService()
        {
                
        }
        public async Task<bool> CheckStatus(string jmbg, string bankPIN)
        {
            if (string.IsNullOrEmpty(jmbg) || string.IsNullOrEmpty(bankPIN))
                return false;
            if (jmbg.Length != 13)
                return false;
            foreach (var item in jmbg)
            {
                if (char.IsLetter(item))
                    return false;
            }
            if (bankPIN.Length != 4)
                return false;

            foreach (var item in bankPIN)
            {
                if (char.IsLetter(item))
                    return false;
            }
            if (bankPIN == "0000")
                return false;
            return true;
        }

        public async Task<BankResponseDTO> Deposit(string jmbg, string bankPIN, decimal amount)
        {
            if (amount < 0)
                return new BankResponseDTO(false, "Amount can't be less than zero");
            if (string.IsNullOrEmpty(bankPIN)
                || bankPIN.Length != 4)
                return new BankResponseDTO(false, "Invalid PIN");
            if (string.IsNullOrEmpty(jmbg)
                || jmbg.Length != 13)
                return new BankResponseDTO(false, "Invalid JMBG");
            return new BankResponseDTO(true);


        }

        public async Task<BankResponseDTO> Withdraw(string jmbg, string bankPIN, decimal amount)
        {
            if (amount < 0)
                return new BankResponseDTO(false, "Amount can't be less than zero");
            if (string.IsNullOrEmpty(bankPIN)
                || bankPIN.Length != 4)
                return new BankResponseDTO(false, "Invalid PIN");
            if (string.IsNullOrEmpty(jmbg)
                || jmbg.Length != 13)
                return new BankResponseDTO(false, "Invalid JMBG");
            return new BankResponseDTO(true);
        }

        
    }
}
