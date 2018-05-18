using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderOrchestra.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderOrchestra.Infrastructure.EntityConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Entities.Order>
    {
        public void Configure(EntityTypeBuilder<Entities.Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId).IsRequired();

            builder.HasMany(x => x.Products).WithOne(x => x.Order);
        }
    }

    public class OrderProductEntityTypeConfiguration : IEntityTypeConfiguration<Entities.OrderProduct>
    {
        public void Configure(EntityTypeBuilder<OrderProduct> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
