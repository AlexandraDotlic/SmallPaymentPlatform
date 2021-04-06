﻿using Common.Utils;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.Internal.BankRoutinService.Interface;
using Core.Domain.Services.Internal.FeeService.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ApplicationServices
{
    public class WalletService
    {
        private readonly ICoreUnitOfWork CoreUnitOfWork;
        private readonly IBankRoutingService BankRoutingService;
        private readonly IFeeService FeeService;
        private readonly decimal MaxDeposit;
        private readonly decimal MaxWithdraw;
        private readonly int DaysAfterWalletCreationWithNoFee;
        private readonly bool IsFirstTransferFreeInMonth;
        private readonly decimal FixedFee;
        private readonly int PercentageFee;
        private readonly decimal FeeLimit;
        private readonly string AdminPASS;

        public WalletService(
            ICoreUnitOfWork coreUnitOfWork,
            IBankRoutingService bankRoutingService,
            IFeeService feeService,
            IConfiguration configuration)
        {
            CoreUnitOfWork = coreUnitOfWork;
            BankRoutingService = bankRoutingService;
            FeeService = feeService;
            MaxDeposit = decimal.Parse(configuration["MaxDeposit"]);
            MaxWithdraw = decimal.Parse(configuration["MaxWithdraw"]);
            DaysAfterWalletCreationWithNoFee = Int32.Parse(configuration["DaysAfterWalletCreationWithNoFee"]);
            IsFirstTransferFreeInMonth = bool.Parse(configuration["IsFirstTransferFreeInMonth"]);
            FixedFee = decimal.Parse(configuration["FixedFee"]);
            PercentageFee = Int32.Parse(configuration["PercentageFee"]);
            FeeLimit = decimal.Parse(configuration["FeeLimit"]);
            AdminPASS = configuration["AdminPASS"];
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
                var depositResponse = await BankRoutingService.Deposit(jmbg, wallet.BankPIN, amount, wallet.Bank);
                if (!depositResponse.IsSuccess)
                {
                    throw new InvalidOperationException(depositResponse.ErrorMessage);
                }

                await CoreUnitOfWork.CommitTransactionAsync();
            }
            catch (InvalidOperationException ex)
            {
                await CoreUnitOfWork.RollbackTransactionAsync();
                throw ex;
            }

        }

        public async Task Transfer(string sourceWalletJmbg, string sourceWalletPass, decimal amount, string destinationWalletJmbg)
        {
            if (amount < 0)
            {
                throw new InvalidOperationException("Amount must be greater than 0");
            }
            Wallet sourceWallet = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                w => w.JMBG == sourceWalletJmbg && w.PASS == sourceWalletPass,
                w => w.Transactions
                );
            if (sourceWallet == null)
            {
                throw new InvalidOperationException($"{nameof(Wallet)} with JMBG = {sourceWalletJmbg} and password = {sourceWalletPass} doesn't exist");
            }
            if (sourceWallet.IsBlocked)
            {
                throw new InvalidOperationException($"{nameof(Transfer)} from blocked wallet #{sourceWallet.JMBG} is forbidden");
            }

            Wallet destinationWallet = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                w => w.JMBG == destinationWalletJmbg,
                w => w.Transactions
                );
            if (destinationWallet == null)
            {
                throw new InvalidOperationException($"{nameof(Wallet)} with JMBG = {sourceWalletJmbg} doesn't exist");
            }
            if (destinationWallet.IsBlocked)
            {
                throw new InvalidOperationException($"{nameof(Transfer)} to blocked wallet #{destinationWallet.JMBG} is forbidden");
            }

            decimal transferFee = await FeeService.CalculateTransferFee(
                sourceWallet, 
                amount,
                DaysAfterWalletCreationWithNoFee,
                IsFirstTransferFreeInMonth,
                FixedFee,
                PercentageFee,
                FeeLimit);
            await CoreUnitOfWork.BeginTransactionAsync();

            try
            {
                sourceWallet.PayOut(amount, TransactionType.TransferPayOut, MaxWithdraw);
                if(transferFee > 0)
                {
                    sourceWallet.PayOut(transferFee, TransactionType.FeePayOut, MaxWithdraw);
                    await CoreUnitOfWork.WalletRepository.Update(sourceWallet);
                    await CoreUnitOfWork.SaveChangesAsync();
                }
                destinationWallet.PayIn(amount, TransactionType.TransferPayIn, MaxDeposit);

                await CoreUnitOfWork.WalletRepository.Update(sourceWallet);
                await CoreUnitOfWork.SaveChangesAsync();

                await CoreUnitOfWork.WalletRepository.Update(destinationWallet);
                await CoreUnitOfWork.SaveChangesAsync();

                await CoreUnitOfWork.CommitTransactionAsync();
            }
            catch (InvalidOperationException ex)
            {
                await CoreUnitOfWork.RollbackTransactionAsync();
                throw ex;
            }

        }

        public async Task<decimal> CalculateTransferFee(string jmbg, string pass, decimal amount)
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
                throw new InvalidOperationException($"{nameof(CalculateTransferFee)} forbidden for blocked wallet");
            }
            decimal fee = await FeeService.CalculateTransferFee(wallet, amount, DaysAfterWalletCreationWithNoFee, IsFirstTransferFreeInMonth, FixedFee, PercentageFee, FeeLimit);

            return fee;
        }

        public async Task BlockWallet(string jmbg, string adminPass)
        {
            if (string.IsNullOrEmpty(jmbg))
            {
                throw new ArgumentNullException($"{nameof(jmbg)}");
            }
            if (string.IsNullOrEmpty(adminPass))
            {
                throw new ArgumentNullException($"{nameof(adminPass)}");
            }
            if (adminPass != AdminPASS)
            {
                throw new ArgumentException($"Operation {nameof(BlockWallet)}  is forbidden: Invalid AdminPASS");
            }
            Wallet wallet = await CoreUnitOfWork.WalletRepository.GetById(jmbg);
            if (wallet == null)
            {
                throw new InvalidOperationException($"{nameof(Wallet)} with JMBG = {jmbg} doesn't exist");
            }

            wallet.Block();
            await CoreUnitOfWork.WalletRepository.Update(wallet);
            await CoreUnitOfWork.SaveChangesAsync();
 
        }
        public async Task UnblockWallet(string jmbg, string adminPass)
        {
            if (string.IsNullOrEmpty(jmbg))
            {
                throw new ArgumentNullException($"{nameof(jmbg)}");
            }
            if (string.IsNullOrEmpty(adminPass))
            {
                throw new ArgumentNullException($"{nameof(adminPass)}");
            }
            if (adminPass != AdminPASS)
            {
                throw new ArgumentException($"Operation {nameof(UnblockWallet)} is forbidden: Invalid AdminPASS");
            }
            Wallet wallet = await CoreUnitOfWork.WalletRepository.GetById(jmbg);
            if (wallet == null)
            {
                throw new InvalidOperationException($"{nameof(Wallet)} with JMBG = {jmbg} doesn't exist");
            }

            wallet.Unblock();
            await CoreUnitOfWork.WalletRepository.Update(wallet);
            await CoreUnitOfWork.SaveChangesAsync();

        }

    }
}
