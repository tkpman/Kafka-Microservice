using System.Linq;
using UnitOfWorks.Abstractions;
using UnitOfWorks.EntityFrameworkCore.Tests.Seeds;
using UnitOfWorks.EntityFrameworkCore.Tests.Utils;
using Xunit;

namespace UnitOfWorks.EntityFrameworkCore.Tests
{
    public class RepositoryTests
    {
        [Fact]
        public async void AddReturnsEntity()
        {
            var options = InMemoryDbContextUtil.CreateInMemoryDatabaseOption(
                nameof(this.AddReturnsEntity));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var repository = new Repository<Customer>(dbContext);
                var customer = new Customer();

                Assert.Equal(customer, repository.Add(customer));
                dbContext.SaveChanges();
                Assert.Equal(customer, dbContext.Customers.Last());
            }
        }

        [Fact]
        public async void AllReturnsEntities()
        {
            var options = InMemoryDbContextUtil.CreateInMemoryDatabaseOption(
                nameof(this.AllReturnsEntities));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var numberOfCustomers = dbContext.Customers.Count();
                var repository = new Repository<Customer>(dbContext);
                var customers = await repository.All();
                Assert.Equal(numberOfCustomers, customers.Count);
            }
        }

        [Fact]
        public async void AllReturnsEntitiesWithSubEntities()
        {
            var options = InMemoryDbContextUtil.CreateInMemoryDatabaseOption(
                nameof(this.AllReturnsEntitiesWithSubEntities));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var numberOfCustomers = dbContext.Customers.Count();
                var repository = new Repository<Customer>(dbContext);
                var specification = new Specification<Customer>();
                specification.Include(x => x.Address);
                var customers = await repository.All(
                    specification
                    );

                Assert.Equal(numberOfCustomers, customers.Count);
                Assert.NotNull(customers.ElementAt(0).Address);
            }
        }

        [Fact]
        public async void FirstOrDefaultReturnsEntity()
        {
            var options = InMemoryDbContextUtil.CreateInMemoryDatabaseOption(
                nameof(this.FirstOrDefaultReturnsEntity));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var repository = new Repository<Customer>(dbContext);
                var idOfFirstCustomer = dbContext.Customers.First();
                var customer = await repository.FirstOrDefault(x => x.Id == idOfFirstCustomer.Id);
                Assert.NotNull(customer);
                Assert.Equal(idOfFirstCustomer.Id, customer.Id);
            }
        }

        [Fact]
        public async void FirstOrDefaultReturnsNull()
        {
            var options = InMemoryDbContextUtil.CreateInMemoryDatabaseOption(
                nameof(this.FirstOrDefaultReturnsNull));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var repository = new Repository<Customer>(dbContext);
                var customer = await repository.FirstOrDefault(x => x.Id == 10000);
                Assert.Null(customer);
            }
        }

        [Fact]
        public async void RemoveReturnsEntity()
        {
            var options = InMemoryDbContextUtil.CreateInMemoryDatabaseOption(
                nameof(this.RemoveReturnsEntity));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var repository = new Repository<Customer>(dbContext);
                var customerToRemove = dbContext.Customers.First();

                Assert.Equal(customerToRemove, repository.Remove(customerToRemove));
                dbContext.SaveChanges();
                Assert.Null(dbContext.Customers.FirstOrDefault(x => x.Id == customerToRemove.Id));
            }
        }

        [Fact]
        public async void UpdateReturnsEntity()
        {
            var options = InMemoryDbContextUtil.CreateInMemoryDatabaseOption(
                nameof(this.UpdateReturnsEntity));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var repository = new Repository<Customer>(dbContext);
                var customerToUpdate = dbContext.Customers.First();
                customerToUpdate.Age = 1000;
                Assert.Equal(customerToUpdate, repository.Update(customerToUpdate));
                dbContext.SaveChanges();
                var customer = dbContext.Customers.FirstOrDefault(x => x.Id == customerToUpdate.Id);
                Assert.NotNull(customer);
                Assert.Equal(1000, customer.Age);
            }
        }

        [Fact]
        public async void WhereReturnsEntities()
        {
            var options = InMemoryDbContextUtil.CreateInMemoryDatabaseOption(
                nameof(this.WhereReturnsEntities));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var repository = new Repository<Customer>(dbContext);
                var countOfCustomersWithAge22 = dbContext.Customers.Count(x => x.Age == 22);
                var customers = await repository.Where(x => x.Age == 22);
                Assert.Equal(countOfCustomersWithAge22, customers.Count);
            }
        }

        [Fact]
        public async void WhereReturnsEntitiesWithSubEntities()
        {
            var options = InMemoryDbContextUtil.CreateInMemoryDatabaseOption(
                nameof(this.WhereReturnsEntitiesWithSubEntities));

            using (var dbContext = new InMemoryDbContext(options))
                Customer.Seed(dbContext);

            using (var dbContext = new InMemoryDbContext(options))
            {
                var repository = new Repository<Customer>(dbContext);
                var countOfCustomersWithAge22 = dbContext.Customers.Count(x => x.Age == 22);
                var specification = new Specification<Customer>();
                specification.Include(x => x.Address);
                var customers = await repository.Where(
                    x => x.Age == 22,
                    specification);
                Assert.Equal(countOfCustomersWithAge22, customers.Count);
                Assert.NotNull(customers.ElementAt(0).Address);
            }
        }
    }
}
