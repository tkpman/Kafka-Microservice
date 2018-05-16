using Customer.Api.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.Application.Infrastructure.EntityTypeConfigurations
{
    public class OrderTransactionEntityTypeConfiguration
        : IEntityTypeConfiguration<Entities.OrderTransaction>
    {
        public void Configure(EntityTypeBuilder<OrderTransaction> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
