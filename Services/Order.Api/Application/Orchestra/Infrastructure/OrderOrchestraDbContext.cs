using Microsoft.EntityFrameworkCore;
using Order.Api.Application.Orchestra.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Application.Orchestra.Infrastructure
{
    public class OrderOrchestraDbContext : DbContext
    {
        public OrderOrchestraDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseInMemoryDatabase("OrderOrchestra");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProductEntityTypeConfiguration());
        }
    }
}
