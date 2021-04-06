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
using System.Text;
using System.Threading.Tasks;

namespace Tests.CoreApplicationServicesTests
{
    [TestClass]
    public class ChangeWalletPassTests
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
        public async Task SuccessWalletChangePassTest()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                string newPass = "1245aa";
                //Act
                await walletService.ChangePass(jmbg, password, newPass);

                //Assert
                Wallet wallet = await CoreUnitOfWork.WalletRepository.GetById(jmbg);

                Assert.AreEqual(true, wallet.IsPassValid(newPass));

            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task FailWalletChangePassTest()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                string newPass = "1245";
                //Act

                //Assert
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.ChangePass(jmbg, password, newPass));


            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task FailWalletChangePassTest2()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                string newPass = "1245aa";
                //Act

                //Assert
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.ChangePass(jmbg, newPass, newPass));


            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task FailWalletChangePassTest3()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                //Act

                //Assert
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await walletService.ChangePass(jmbg, password, null));


            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

        [TestMethod]
        public async Task FailWalletChangePassTest4()
        {
            try
            {
                string jmbg = "2904992785075";
                //Arrange
                var walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
                string password = await walletService.CreateWallet(jmbg, "TestIme", "TestPrezime", (short)BankType.FirstBank, "360123456789999874", "1234");

                string newPass = "1245aa";
                //Act

                //Assert
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await walletService.ChangePass(null, password, newPass));


            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected error: " + ex.Message);
            }
        }

    }
}
