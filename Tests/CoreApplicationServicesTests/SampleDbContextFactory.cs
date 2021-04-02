using Core.Infrastructure.DataAccess.EfCoreDataAccess;
using EfCoreDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.CoreApplicationServicesTests
{
    public class SampleDbContextFactory : IDesignTimeDbContextFactory<EfCoreDbContext>
    {
        public EfCoreDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<EfCoreDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=test;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

            return new EfCoreDbContext(options);
        }
    }

}
