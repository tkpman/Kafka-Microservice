using System.Linq;
using UnitOfWorks.EntityFrameworkCore.Tests.Seeds;
using UnitOfWorks.EntityFrameworkCore.Tests.Utils;
using Xunit;

namespace UnitOfWorks.EntityFrameworkCore.Tests
{
    public class UnitOfWorkTests
    {
        [Fact]
        public async void GetRepositoryReturnsRepository()
        {
            var options = InMemoryDbContextUtil
                .CreateInMemoryDatabaseOption(nameof(this.GetRepositoryReturnsRepository));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var unitOfWork = new UnitOfWork<InMemoryDbContext>(dbContext);
                var repository = unitOfWork.GetRepository<Customer>();
                Assert.NotNull(repository);
            }
        }

        [Fact]
        public async void SaveChanges()
        {
            var options = InMemoryDbContextUtil
                .CreateInMemoryDatabaseOption(nameof(this.GetRepositoryReturnsRepository));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var unitOfWork = new UnitOfWork<InMemoryDbContext>(dbContext);
                var firstCustomer = dbContext.Customers.First();
                dbContext.Customers.Remove(firstCustomer);
                await unitOfWork.SaveChanges();
                Assert.False(firstCustomer.Id == dbContext.Customers.First().Id);
            }
        }
    }
}
