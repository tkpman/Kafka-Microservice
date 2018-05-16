using Customer.Api.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.Application.Infrastructure.EntityTypeConfigurations
{
    public class CustomerEntityTypeConfiguration
        : IEntityTypeConfiguration<Entities.Customer>
    {
        public void Configure(EntityTypeBuilder<Entities.Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Credit).IsConcurrencyToken(true);

            builder.HasMany(x => x.OrderTransactions).WithOne(x => x.Customer).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
