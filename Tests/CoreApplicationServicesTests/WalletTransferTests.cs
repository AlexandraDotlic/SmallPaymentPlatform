using ApplicationServices;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.Internal.BankRoutinService.Implementations;
using Core.Domain.Services.Internal.BankRoutinService.Interface;
using Core.Domain.Services.Internal.FeeService.Implementations;
using Core.Domain.Services.Internal.FeeService.Interface;
using Core.Infrastructure.DataAccess.EfCoreDataAccess;
using EfCoreDataAccess;
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
    public class WalletTransferTests
    {
        private ICoreUnitOfWork CoreUnitOfWork;
        private EfCoreDbContext DbContext;
        private IBankRoutingService BankRoutingService;
        private IFeeService FeeService;
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
                {"MaxDeposit", "1000000" },
                {"MaxWithdraw", "100000"},
                { "DaysAfterWalletCreationWithNoFee","7"},
                { "IsFirstTransferFreeInMonth", "True" },
                { "FixedFee","100" },
                { "FeeLimit", "10000" },
                { "PercentageFee", "1" }
            };

            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            FeeService = new FeeService();

        }

        [TestCleanup()]
        public async Task Cleanup()
        {
            CoreUnitOfWork.ClearTracker();
            Wallet wallet1 = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                    wallet => wallet.JMBG == "2904992785075",
                    wallet => wallet.Transactions
                );

            if (wallet1 != null)
            {
                await CoreUnitOfWork.WalletRepository.Delete(wallet1);
                await CoreUnitOfWork.SaveChangesAsync();
            }
            Wallet wallet2 = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                    wallet => wallet.JMBG == "2904990785034",
                    wallet => wallet.Transactions
                );

            if (wallet2 != null)
            {
                await CoreUnitOfWork.WalletRepository.Delete(wallet2);
                await CoreUnitOfWork.SaveChangesAsync();
            }
            await DbContext.DisposeAsync();
            DbContext = null;
        }

        [TestMethod]
        public async Task SuccessWalletTransferTest()
        {

            try
            {
                string jmbg1 = "2904992785075";
                string jmbg2 = "2904990785034";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password1 = await walletService.CreateWallet(jmbg1, "TestIme1", "TestPrezime1", (short)BankType.FirstBank, "360123456789999874", "1234");
                string password2 = await walletService.CreateWallet(jmbg2, "TestIme2", "TestPrezime2", (short)BankType.FirstBank, "360123456789999889", "1224");

                await walletService.Deposit(jmbg1, password1, 2000m);
                
                //Act

                await walletService.Transfer(jmbg1, password1, 500m, jmbg2);

                //Assert
                Wallet wallet1 = await CoreUnitOfWork.WalletRepository.GetById(jmbg1);
                Wallet wallet2 = await CoreUnitOfWork.WalletRepository.GetById(jmbg2);

                Assert.AreEqual(1500m, wallet1.Balance, "Balance must be 1500");
                Assert.AreEqual(500m, wallet2.Balance, "Balance must be 500");

                Assert.AreNotEqual(0, wallet1.Transactions.Count(), "Transaction count must be different than 0");
                Assert.AreNotEqual(0, wallet2.Transactions.Count(), "Transaction count must be different than 0");

                Assert.AreEqual(TransactionType.TransferPayOut, wallet1.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.TransferPayOut).Type);
                Assert.AreEqual(TransactionType.TransferPayIn, wallet2.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.TransferPayIn).Type);

                Assert.AreEqual(500m, wallet1.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.TransferPayOut).Amount, $"Transaction amount must be 500.");

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task SuccessWalletTransferWithFeeTest()
        {

            try
            {
                Configuration["IsFirstTransferFreeInMonth"] = "False";
                Configuration["DaysAfterWalletCreationWithNoFee"] = "0";
                string jmbg1 = "2904992785075";
                string jmbg2 = "2904990785034";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password1 = await walletService.CreateWallet(jmbg1, "TestIme1", "TestPrezime1", (short)BankType.FirstBank, "360123456789999874", "1234");
                string password2 = await walletService.CreateWallet(jmbg2, "TestIme2", "TestPrezime2", (short)BankType.FirstBank, "360123456789999889", "1224");

                await walletService.Deposit(jmbg1, password1, 2000m);

                //Act

                await walletService.Transfer(jmbg1, password1, 500m, jmbg2);

                //Assert
                Wallet wallet1 = await CoreUnitOfWork.WalletRepository.GetById(jmbg1);
                Wallet wallet2 = await CoreUnitOfWork.WalletRepository.GetById(jmbg2);

                Assert.AreEqual(1400m, wallet1.Balance, "Balance must be 1400");
                Assert.AreEqual(500m, wallet2.Balance, "Balance must be 500");

                Assert.AreNotEqual(0, wallet1.Transactions.Count(), "Transaction count must be different than 0");
                Assert.AreNotEqual(0, wallet2.Transactions.Count(), "Transaction count must be different than 0");

                Assert.AreEqual(TransactionType.TransferPayOut, wallet1.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.TransferPayOut).Type);
                Assert.AreEqual(TransactionType.TransferPayIn, wallet2.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.TransferPayIn).Type);

                Assert.AreEqual(TransactionType.FeePayOut, wallet1.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.FeePayOut).Type);
                Assert.AreEqual(100m, wallet1.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.FeePayOut).Amount, $"Transaction transfer fee amount must be 100.");

                Assert.AreEqual(500m, wallet1.Transactions.FirstOrDefault(transaction => transaction.Type == TransactionType.TransferPayOut).Amount, $"Transaction amount must be 500.");

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task FailWalletTransferTest1()
        {
            try
            {
                string jmbg1 = "2904992785075";
                string jmbg2 = "2904990785034";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password1 = await walletService.CreateWallet(jmbg1, "TestIme1", "TestPrezime1", (short)BankType.FirstBank, "360123456789999874", "1234");
                string password2 = await walletService.CreateWallet(jmbg2, "TestIme2", "TestPrezime2", (short)BankType.FirstBank, "360123456789999889", "1224");
                await walletService.Deposit(jmbg1, password1, 2000m);

                //Assert
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.Transfer(jmbg1, password1, 2000000m, jmbg2), $"Exceeded monthly withdraw and deposit limit ({Configuration["MaxDeposit"]} RSD).");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }

        }


        [TestMethod]
        public async Task FailWalletTransferTest2()
        {
            try
            {
                string jmbg1 = "2904992785075";
                string jmbg2 = "2904990785034";
                string pass = "abcdef";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);

                //Act
                //Assert
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.Transfer(jmbg1, pass, 1000m, jmbg2), $"Wallet #{jmbg2} doesn't exist");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task FailWalletTransferTest3()
        {
            try
            {
                string jmbg1 = "2904992785075";
                string jmbg2 = "2904990785034";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password1 = await walletService.CreateWallet(jmbg1, "TestIme1", "TestPrezime1", (short)BankType.FirstBank, "360123456789999874", "1234");
                string password2 = await walletService.CreateWallet(jmbg2, "TestIme2", "TestPrezime2", (short)BankType.FirstBank, "360123456789999889", "1224");

                //Assert
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.Transfer(jmbg1, password1, 1000m, jmbg2), $"Not enough funds on wallet #{jmbg1}.");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }

        }
    }
}
