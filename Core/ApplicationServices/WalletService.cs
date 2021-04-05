using Common.Utils;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.Internal.BankRoutinService.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ApplicationServices
{
    public class WalletService
    {
        private readonly ICoreUnitOfWork CoreUnitOfWork;
        private readonly IBankRoutingService BankRoutingService;
        private readonly decimal MaxDeposit;
        private readonly decimal MaxWithdraw;

        public WalletService(
            ICoreUnitOfWork coreUnitOfWork,
            IBankRoutingService bankRoutingService,
            IConfiguration configuration)
        {
            CoreUnitOfWork = coreUnitOfWork;
            BankRoutingService = bankRoutingService;
            MaxDeposit = decimal.Parse(configuration["MaxDeposit"]);
            MaxWithdraw = decimal.Parse(configuration["MaxWithdraw"]);
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
            if (amount < 0)
            {
                throw new InvalidOperationException("Amount must be greater than 0");
            }
            Wallet wallet = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                w => w.JMBG == jmbg && w.PASS == pass,
                w => w.Transactions
                );
            if (wallet == null)
            {
                throw new InvalidOperationException($"{nameof(Wallet)} with JMBG = {jmbg} and password = {pass} doesn't exist");
            }
            if (wallet.IsBlocked)
            {
                throw new InvalidOperationException($"{nameof(Deposit)} forbidden for blocked wallet");
            }
            await CoreUnitOfWork.BeginTransactionAsync();

            try
            {
                wallet.PayIn(amount, TransactionType.Deposit, MaxDeposit);
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
        public async Task Withdraw(string jmbg, string pass, decimal amount)
        {
            if (amount < 0)
            {
                throw new InvalidOperationException("Amount must be greater than 0");
            }
            Wallet wallet = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                w => w.JMBG == jmbg && w.PASS == pass,
                w => w.Transactions
                );
            if (wallet == null)
            {
                throw new InvalidOperationException($"{nameof(Wallet)} with JMBG = {jmbg} and password = {pass} doesn't exist");
            }
            if (wallet.IsBlocked)
            {
                throw new InvalidOperationException($"{nameof(Deposit)} forbidden for blocked wallet");
            }
            await CoreUnitOfWork.BeginTransactionAsync();

            try
            {
                wallet.PayOut(amount, TransactionType.Withdraw, MaxWithdraw);
                await CoreUnitOfWork.WalletRepository.Update(wallet);
                await CoreUnitOfWork.SaveChangesAsync();
                var withdrawResponse = await BankRoutingService.Deposit(jmbg, wallet.BankPIN, amount, wallet.Bank);
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
