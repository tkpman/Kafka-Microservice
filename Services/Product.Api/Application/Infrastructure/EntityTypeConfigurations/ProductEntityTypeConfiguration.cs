using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Api.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Application.Infrastructure.EntityTypeConfigurations
{
    public class ProductEntityTypeConfiguration
        : IEntityTypeConfiguration<Entities.Product>
    {
        public void Configure(EntityTypeBuilder<Entities.Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity).IsConcurrencyToken(true);

        }
    }
}
