using ApplicationServices;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services.Internal.BankRoutinService.Implementations;
using Core.Domain.Services.Internal.BankRoutinService.Interface;
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

        [TestInitialize]
        public void Setup()
        {
            var dbContextFactory = new SampleDbContextFactory();
            DbContext = dbContextFactory.CreateDbContext(new string[] { });
            CoreUnitOfWork = new EfCoreUnitOfWork(DbContext);

            var inMemoryCollection = new Dictionary<string, string> {
                {"MaxDeposit", "1000000"},
                {"MaxWithdraw", "1000000"},

            };

            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryCollection)
                .Build();
            var firstBankService = new FirstBankService();
            BankRoutingService = new BankRoutingService(firstBankService);
        }

        [TestCleanup()]
        public async Task Cleanup()
        {
            await DbContext.DisposeAsync();
            CoreUnitOfWork = null;
        }
        [TestMethod]
        public async Task TestCreateWallet()
        {

            WalletService walletService = new WalletService(CoreUnitOfWork, BankRoutingService, Configuration);

            string walletPass = await walletService.CreateWallet("1203977780011", "Pera", "Peric", 1, "360123456", "1234");

            Wallet wallet = await CoreUnitOfWork.WalletRepository.GetFirstOrDefaultWithIncludes(w => w.PASS == walletPass);

            Assert.AreEqual(walletPass.Length, 6);
            Assert.AreNotEqual(null, wallet, "Wallet can't be null");
            Assert.AreEqual("1203977780011", wallet.JMBG, "JMBG must be equal");
            Assert.AreEqual("Pera", wallet.FirstName);
            Assert.AreEqual("Peric", wallet.LastName);
            Assert.AreEqual(1, (short)wallet.Bank);
            Assert.AreEqual("360123456", wallet.BankAccountNumber);
            Assert.AreEqual("1234", wallet.BankPIN);
            Assert.AreEqual(walletPass, wallet.PASS);



        }
    }
}
