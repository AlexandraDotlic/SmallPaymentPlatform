using Core.Domain.Entities;
using Core.Domain.Services.External.BankService;
using Core.Domain.Services.Internal.BankRoutinService.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services.Internal.BankRoutinService.Implementations
{
    public class BankRoutingService : IBankRoutingService
    {
        private IFirstBankService FirstBankService;

        public BankRoutingService(IFirstBankService firstBankService)
        {
            FirstBankService = firstBankService;
        }

        public async Task<BankResponse> CheckStatus(string jmbg, string bankPIN, BankType bankType)
        {
            switch (bankType)
            {
                case BankType.FirstBank:
                    BankResponse response = await FirstBankService.CheckStatus(jmbg, bankPIN);
                    return response;
                default:
                    throw new InvalidOperationException("Unsupported bank");
            }
        }

        public async Task<BankResponse> Deposit(string jmbg, string bankPIN, decimal amount, BankType bankType)
        {
            switch (bankType)
            {
                case BankType.FirstBank:
                    BankResponse response = await FirstBankService.Deposit(jmbg, bankPIN, amount);
                    return response;
                default:
                    throw new InvalidOperationException("Unsupported bank");
            }
        }

        public async Task<BankResponse> Withdraw(string jmbg, string bankPIN, decimal amount, BankType bankType)
        {
            switch (bankType)
            {
                case BankType.FirstBank:
                    BankResponse response = await FirstBankService.Withdraw(jmbg, bankPIN, amount);
                    return response;
                default:
                    throw new InvalidOperationException("Unsupported bank");
            }
        }
    }
}
