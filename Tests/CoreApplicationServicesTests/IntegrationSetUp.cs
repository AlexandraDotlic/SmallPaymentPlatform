using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests.CoreApplicationServicesTests
{
    [TestClass]
    public class IntegrationSetUp
    {
        [AssemblyInitialize()]
        public static async Task AssemblyInit(TestContext context)
        {
            var dbContextFactory = new SampleDbContextFactory();
            using (var dbContext = dbContextFactory.CreateDbContext(new string[] { }))
            {
                await dbContext.Database.EnsureCreatedAsync();

            }
        }
            [AssemblyCleanup()]
        public static async Task AssemblyCleanup()
        {
            var dbContextFactory = new SampleDbContextFactory();
            using (var dbContext = dbContextFactory.CreateDbContext(new string[] { }))
            {
                await dbContext.Database.EnsureDeletedAsync();
            }
        }
    }

}
