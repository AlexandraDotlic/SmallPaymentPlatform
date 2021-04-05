using Core.Domain.Services.External.BankService;
using System;
using System.Threading.Tasks;

namespace MockBankService
{
    public class FirstBankService : IFirstBankService
    {
        public FirstBankService()
        {
                
        }
        public async Task<BankResponse> CheckStatus(string jmbg, string bankPIN)
        {
            if (string.IsNullOrEmpty(jmbg))
                return new BankResponse(false, "JMBG can't be null");
            if (string.IsNullOrEmpty(bankPIN))
                return new BankResponse(false, "Bank PIN can't be null");
            if (jmbg.Length != 13)
                return new BankResponse(false, "JMBG must be 13 characters long");
            foreach (var item in jmbg)
            {
                if (char.IsLetter(item))
                    return new BankResponse(false, "JMBG can't contain letters");
            }
            if (bankPIN.Length != 4)
                return new BankResponse(false, "Bank PIN must be 4 characters long");

            foreach (var item in bankPIN)
            {
                if (char.IsLetter(item))
                    return new BankResponse(false, "Bank PIN can't contain letters");
            }
            if (bankPIN == "0000")
                return new BankResponse(false, "Invalid Bank PIN");
            return new BankResponse(true);
        }

        public async Task<BankResponse> Deposit(string jmbg, string bankPIN, decimal amount)
        {
            if (amount < 0)
                return new BankResponse(false, "Amount can't be less than zero");
            if (string.IsNullOrEmpty(bankPIN)
                || bankPIN.Length != 4)
                return new BankResponse(false, "Invalid Bank PIN");
            if (string.IsNullOrEmpty(jmbg)
                || jmbg.Length != 13)
                return new BankResponse(false, "Invalid JMBG");
            return new BankResponse(true);


        }

        public async Task<BankResponse> Withdraw(string jmbg, string bankPIN, decimal amount)
        {
            if (amount < 0)
                return new BankResponse(false, "Amount can't be less than zero");
            if (string.IsNullOrEmpty(bankPIN)
                || bankPIN.Length != 4)
                return new BankResponse(false, "Invalid Bank PIN");
            if (string.IsNullOrEmpty(jmbg)
                || jmbg.Length != 13)
                return new BankResponse(false, "Invalid JMBG");
            return new BankResponse(true);
        }

        
    }
}
