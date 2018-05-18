using Microsoft.EntityFrameworkCore;
using OrderQuery.Api.Application.Infrastructure.EntityTypeConfigurations;

namespace OrderQuery.Api.Application.Infrastructure
{
    public class OrderDbContext
        : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base() { }

        public OrderDbContext() : base() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseInMemoryDatabase("Order.Query");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProductEntityTypeConfiguration());
        }
    }
}
