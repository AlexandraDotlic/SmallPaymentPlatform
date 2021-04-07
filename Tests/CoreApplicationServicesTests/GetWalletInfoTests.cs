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
    public class GetWalletInfoTests
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
                { "PercentageFee", "1" },
                {"AdminPASS", "admin!" },

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
        public async Task SuccessGetWalletInfoTest()
        {
            
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874","1234");

                //Act
                var walletInfoDTO = await walletService.GetWalletInfo(jmbg, password);

                //Assert

                Assert.AreEqual(walletInfoDTO.JMBG, jmbg);
                Assert.AreEqual(walletInfoDTO.FirstName, "TestIme");
                Assert.AreEqual(walletInfoDTO.LastName, "TestPrezime");
                Assert.AreEqual(walletInfoDTO.Bank, BankType.FirstBank);
                Assert.AreEqual(walletInfoDTO.BankAccountNumber, "360123456789999874");
                Assert.AreEqual(walletInfoDTO.Balance, 0m);
                Assert.AreEqual(walletInfoDTO.IsBlocked, false);


            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task SuccessGetWalletInfoTest2()
        {

            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");
                await walletService.BlockWallet(jmbg, Configuration["AdminPASS"]);
                //Act
                var walletInfoDTO = await walletService.GetWalletInfo(jmbg, password);

                //Assert

                Assert.AreEqual(walletInfoDTO.JMBG, jmbg);
                Assert.AreEqual(walletInfoDTO.FirstName, "TestIme");
                Assert.AreEqual(walletInfoDTO.LastName, "TestPrezime");
                Assert.AreEqual(walletInfoDTO.Bank, BankType.FirstBank);
                Assert.AreEqual(walletInfoDTO.BankAccountNumber, "360123456789999874");
                Assert.AreEqual(walletInfoDTO.Balance, 0m);
                Assert.AreEqual(walletInfoDTO.IsBlocked, true);


            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }
        [TestMethod]
        public async Task FailGetWalletInfoTest1()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                //Assert
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.GetWalletInfo(jmbg, "1234"), "Invalid password");
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }

        }

        [TestMethod]
        public async Task FailGetWalletInfoTest2()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                //Assert
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await walletService.GetWalletInfo(null, password));
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }

        }

        [TestMethod]
        public async Task FailGetWalletInfoTest3()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                //Assert
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await walletService.GetWalletInfo(jmbg, null));
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }

        }

    }
}
