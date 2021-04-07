using ApplicationServices;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.Internal.BankRoutinService.Implementations;
using Core.Domain.Services.Internal.BankRoutinService.Interface;
using Core.Domain.Services.Internal.FeeService.Implementations;
using Core.Domain.Services.Internal.FeeService.Interface;
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
    public class GetWalletTransactionsbyDateTests
    {
        private ICoreUnitOfWork CoreUnitOfWork;
        private EfCoreDbContext DbContext;
        private IBankRoutingService BankRoutingService;
        private IConfiguration Configuration;
        private IFeeService FeeService;

        [TestInitialize]
        public async Task Setup()
        {
            var dbContextFactory = new SampleDbContextFactory();
            DbContext = dbContextFactory.CreateDbContext(new string[] { });
            CoreUnitOfWork = new EfCoreUnitOfWork(DbContext);
            var firstBankService = new FirstBankService();
            BankRoutingService = new BankRoutingService(firstBankService);
            FeeService = new FeeService();

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
        public async Task SuccessGetWalletTransactionsByDateTest()
        {
            
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874","1234");
                await walletService.Deposit(jmbg, password, 1000m);
                //Act
                var walletTransactionsDTO = await walletService.GetWalletTransactionsByDate(jmbg, password, DateTime.Now);

                //Assert
                Wallet wallet = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.JMBG == jmbg,
                    w => w.Transactions.Where(t => t.TransactionDateTime.Date == DateTime.Now.Date));

                Assert.AreEqual(walletTransactionsDTO.JMBG, wallet.JMBG);
                Assert.AreEqual(walletTransactionsDTO.Balance, wallet.Balance);
                Assert.AreEqual(walletTransactionsDTO.Transactions.Count, 1);
                Assert.AreEqual(walletTransactionsDTO.Transactions.Count, wallet.Transactions.Count);
                Assert.AreEqual(walletTransactionsDTO.Transactions.First().Type, TransactionType.Deposit);
                Assert.AreEqual(walletTransactionsDTO.Transactions.First().Amount, 1000m);




            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task FailGetWalletTransactionsByDateTest1()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                //Assert
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.GetWalletTransactionsByDate(jmbg, "1234", DateTime.Now), "Invalid password");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }

        }

        [TestMethod]
        public async Task FailGetWalletTransactionsByDateTest2()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                //Assert
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await walletService.GetWalletTransactionsByDate(null, password, DateTime.Now));
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }

        }

        [TestMethod]
        public async Task FailGetWalletTransactionsByDateTest3()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                //Assert
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await walletService.GetWalletTransactionsByDate(jmbg, null, DateTime.Now));
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }

        }

    }
}
