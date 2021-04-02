using Common.Utils;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.Internal.BankRoutinService.Interface;
using System;
using System.Threading.Tasks;

namespace ApplicationServices
{
    public class WalletService
    {
        private readonly ICoreUnitOfWork CoreUnitOfWork;
        private readonly IBankRoutingService BankRoutingService;

        public WalletService(
            ICoreUnitOfWork coreUnitOfWork,
            IBankRoutingService bankRoutingService)
        {
            CoreUnitOfWork = coreUnitOfWork;
            BankRoutingService = bankRoutingService;
        }

        public async Task<string> CreateWallet(
            string jmbg,
            string firstName, 
            string lastName, 
            short bankType, 
            string bankAccountNumber,
            string bankPIN
            )
        {
            //provera ka servisu banke da li uopste moze da se kreira wallet
            var response = await BankRoutingService.CheckStatus(jmbg, bankPIN, (BankType)bankType);

            if (!response.IsSuccess)
            {
                throw new InvalidOperationException($"Creating wallet for JMBG= {jmbg} and PIN= {bankPIN} not allowed");
            }
            //provera da li je korisnik punoletan
            if(JMBGParser.CalculatePersonsYearsFromJMBG(jmbg) < 18)
            {
                throw new InvalidOperationException($"Creating wallet for JMBG= {jmbg} and PIN= {bankPIN} not allowed because person is under 18");

            }

            Wallet wallet = new Wallet(jmbg, firstName, lastName, (BankType)bankType, bankAccountNumber, bankPIN);
            await CoreUnitOfWork.WalletRepository.Insert(wallet);
            await CoreUnitOfWork.SaveChangesAsync();

            return wallet.PASS;
        }

        public async Task Deposit(string jmbg, string pass, decimal amount)
        {
            if(amount < 0)
            {
                throw new InvalidOperationException("Amount must be greater than 0");
            }
            Wallet wallet = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                w => w.JMBG == jmbg && w.PASS == pass,
                w => w.Transactions
                );
            if(wallet == null)
            {
                throw new InvalidOperationException($"{nameof(Wallet)} with JMBG = {jmbg} and password = {pass} doesn't exist");
            }
            //todo: maximum deposit
            //todo: no deposit for blocked wallet
            decimal maxDeposit = 0;
            await CoreUnitOfWork.BeginTransactionAsync();

            try
            {
                wallet.PayIn(amount, TransactionType.Deposit, wallet.Bank.ToString(), maxDeposit);
                await CoreUnitOfWork.WalletRepository.Update(wallet);
                await CoreUnitOfWork.SaveChangesAsync();
                var withdrawResponse = await BankRoutingService.Withdraw(jmbg, wallet.BankPIN, amount, wallet.Bank);
                if (!withdrawResponse.IsSuccess)
                {
                    throw new InvalidOperationException(withdrawResponse.ErrorMessage);
                }

                await CoreUnitOfWork.CommitTransactionAsync();
            }
            catch (InvalidOperationException ex)
            {
                await CoreUnitOfWork.RollbackTransactionAsync();
                throw ex;
            }

        }
    }
}
