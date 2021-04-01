﻿using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.External.BankService;
using System;
using System.Threading.Tasks;

namespace ApplicationServices
{
    public class WalletService
    {
        private readonly ICoreUnitOfWork CoreUnitOfWork;
        private readonly IBankService BankService;

        public WalletService(
            ICoreUnitOfWork coreUnitOfWork,
            IBankService bankService)
        {
            CoreUnitOfWork = coreUnitOfWork;
            BankService = bankService;
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
            //TODO: provera ka servisu banke da li uopste moze da se kreira wallet
            bool doesBankAccountExist = await BankService.CheckStatus(jmbg, bankPIN);

            if (!doesBankAccountExist)
            {
                throw new InvalidOperationException($"Creating wallet for JMBG= {jmbg} and PIN= {bankPIN} not allowed");
            }
            //TODO: provera da li je korisnik punoletan
            Wallet wallet = new Wallet(jmbg, firstName, lastName, (BankType)bankType, bankAccountNumber, bankPIN);
            await CoreUnitOfWork.WalletRepository.Insert(wallet);
            await CoreUnitOfWork.SaveChangesAsync();

            return wallet.PASS;
        }
    }
}
