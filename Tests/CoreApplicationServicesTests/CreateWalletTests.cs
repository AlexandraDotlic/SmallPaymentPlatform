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
    public class CreateWalletTests
    {
        private ICoreUnitOfWork CoreUnitOfWork;
        private EfCoreDbContext DbContext;
        private IBankRoutingService BankRoutingService;
        private IConfiguration Configuration;
        private IFeeService FeeService;

        [TestInitialize]
        public void Setup()
        {
            var dbContextFactory = new SampleDbContextFactory();
            DbContext = dbContextFactory.CreateDbContext(new string[] { });
            CoreUnitOfWork = new EfCoreUnitOfWork(DbContext);

            var inMemoryCollection = new Dictionary<string, string> {
                {"MaxDeposit", "1000000" },
                {"MaxWithdraw", "100000"},
                { "DaysAfterWalletCreationWithNoFee","7"},
                { "IsFirstTransferFreeInMonth", "True" },
                { "FixedFee","100" },
                { "FeeLimit", "10000" },
                { "PercentageFee", "1" }

            };

            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryCollection)
                .Build();
            var firstBankService = new FirstBankService();
            BankRoutingService = new BankRoutingService(firstBankService);
            FeeService = new FeeService();

        }

        [TestCleanup()]
        public async Task Cleanup()
        {
            CoreUnitOfWork.ClearTracker();
            Wallet wallet = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                    wallet => wallet.JMBG == "1203977780011",
                    wallet => wallet.Transactions
                );

            if (wallet != null)
            {
                await CoreUnitOfWork.WalletRepository.Delete(wallet);
                await CoreUnitOfWork.SaveChangesAsync();
            }

            Wallet wallet2 = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(
                   wallet => wallet.JMBG == "1203008780011",
                   wallet => wallet.Transactions
               );

            if (wallet2 != null)
            {
                await CoreUnitOfWork.WalletRepository.Delete(wallet);
                await CoreUnitOfWork.SaveChangesAsync();
            }
            await DbContext.DisposeAsync();
            DbContext = null;
        }

        [TestMethod]
        public async Task SuccessCreateWalletTest()
        {

            WalletService walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);

            string walletPass = await walletService.CreateWallet("1203977780011", "Pera", "Peric", 1, "360123456", "1234");

            Wallet wallet = await CoreUnitOfWork.WalletRepository.GetById("1203977780011");

            Assert.AreEqual(walletPass.Length, 6);
            Assert.AreNotEqual(null, wallet, "Wallet can't be null");
            Assert.AreEqual("1203977780011", wallet.JMBG, "JMBG must be equal");
            Assert.AreEqual("Pera", wallet.FirstName);
            Assert.AreEqual("Peric", wallet.LastName);
            Assert.AreEqual(1, (short)wallet.Bank);
            Assert.AreEqual("360123456", wallet.BankAccountNumber);
            Assert.AreEqual("1234", wallet.BankPIN);
            Assert.AreEqual(true, wallet.IsPassValid(walletPass));
        }
        [TestMethod]
        public async Task FailCreateWalletTest1()
        {

            WalletService walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
           
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.CreateWallet("1203977780011", "Pera", "Peric", 1, "360123456", null), $"Invalid bank PIN");

        }
        [TestMethod]
        public async Task FailCreateWalletTest2()
        {

            WalletService walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.CreateWallet("1203977780011", "Pera", "Peric", 1, "360123456", "0000"), $"Invalid bank PIN");

        }
        [TestMethod]
        public async Task FailCreateWalletTest3()
        {

            WalletService walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.CreateWallet("1203008780011", "Mika", "Peric", 1, "360123456", "1234"), $"User must be at least 18 years old");

        }

        [TestMethod]
        public async Task FailCreateWalletTest4()
        {

            WalletService walletService = new WalletService(CoreUnitOfWork, BankRoutingService, FeeService, Configuration);
            string walletPass = await walletService.CreateWallet("1203977780011", "Pera", "Peric", 1, "360123456", "1234");

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await walletService.CreateWallet("1203977780011", "Pera", "Peric", 1, "360123456", "1234"), $"Wallet already exists");

        }
    }
}
