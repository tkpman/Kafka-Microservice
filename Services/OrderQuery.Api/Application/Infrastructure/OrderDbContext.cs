using Microsoft.EntityFrameworkCore;
using OrderQuery.Api.Application.Infrastructure.EntityTypeConfigurations;

namespace OrderQuery.Api.Application.Infrastructure
{
    public class OrderDbContext
        : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base() { }

        public OrderDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseInMemoryDatabase("Order");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProductEntityTypeConfiguration());
        }
    }
}
