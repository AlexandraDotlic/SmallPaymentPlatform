using Core.Domain.Entities;
using Core.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace ApplicationServices
{
    public class WalletService
    {
        private readonly ICoreUnitOfWork CoreUnitOfWork;

        public WalletService(ICoreUnitOfWork coreUnitOfWork)
        {
            CoreUnitOfWork = coreUnitOfWork;
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
            //TODO: provera da li je korisnik punoletan
            Wallet wallet = new Wallet(jmbg, firstName, lastName, (BankType)bankType, bankAccountNumber, bankPIN);
            await CoreUnitOfWork.WalletRepository.Insert(wallet);
            await CoreUnitOfWork.SaveChangesAsync();

            return wallet.PASS;
        }
    }
}
