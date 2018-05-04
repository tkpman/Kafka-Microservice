using Microsoft.EntityFrameworkCore;
using UnitOfWorks.EntityFrameworkCore.Tests.Seeds;

namespace UnitOfWorks.EntityFrameworkCore.Tests.Utils
{
    public class InMemoryDbContext
        : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options)
            : base(options) { }
    }
}
