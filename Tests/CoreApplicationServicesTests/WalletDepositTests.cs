using ApplicationServices;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.Internal.BankRoutinService.Implementations;
using Core.Domain.Services.Internal.BankRoutinService.Interface;
using Core.Infrastructure.DataAccess.EfCoreDataAccess;
using EfCoreDataAccess;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockBankService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.CoreApplicationServicesTests
{
    [TestClass]
    public class WalletDepositTests
    {
        private ICoreUnitOfWork CoreUnitOfWork;
        private EfCoreDbContext DbContext;
        private IBankRoutingService BankRoutingService;
        private IConfiguration Configuration;

        [TestInitialize]
        public async Task Setup()
        {
            var dbContextFactory = new SampleDbContextFactory();
            DbContext = dbContextFactory.CreateDbContext(new string[] { });
            CoreUnitOfWork = new EfCoreUnitOfWork(DbContext);
            var firstBankService = new FirstBankService();
            BankRoutingService = new BankRoutingService(firstBankService);

            var inMemorySettings = new Dictionary<string, string> {
                {"MaxDeposit", "1000000"},
            };

            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [TestCleanup()]
        public async Task Cleanup()
        {
            CoreUnitOfWork.ClearTracker();
            Wallet wallet = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                    wallet => wallet.JMBG == "2904992785075",
                    wallet => wallet.Transactions
                );

            if (wallet != null)
            {
                await CoreUnitOfWork.WalletRepository.Delete(wallet);
                await CoreUnitOfWork.SaveChangesAsync();
            }
            await DbContext.DisposeAsync();
            DbContext = null;
        }

        [TestMethod]
        public async Task SuccessWalletDepositTest()
        {
            
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874","1234");

                //Act
                await walletService.Deposit(jmbg, password, 1000m);

                //Assert
                Wallet wallet = await CoreUnitOfWork.WalletRepository.GetById(jmbg);

                Assert.AreEqual(1000m, wallet.Balance, "Balance must be 1000");
                Assert.AreNotEqual(0, wallet.Transactions.Count(), "Transaction count must be different than 0");
                Assert.AreEqual(TransactionType.Deposit, wallet.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.Deposit).Type);
                Assert.AreEqual(1000m, wallet.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.Deposit).Amount, $"Transaction amount must be 10000.");

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

    }
}
