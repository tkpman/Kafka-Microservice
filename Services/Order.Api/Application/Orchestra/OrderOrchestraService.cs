using HostedServices;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Order.Api.Application.Orchestra.Infrastructure;

namespace Order.Api.Application.Orchestra
{
    public class OrderOrchestraService
        : JobHostingService
    {
        public OrderOrchestraService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.AddJob(new Jobs.OrderCreatedJob(
                serviceProvider.GetRequiredService<IConfiguration>()));
            this.AddJob(new Jobs.ReservedOrderJob(
                serviceProvider.GetRequiredService<IConfiguration>()));
            this.AddJob(new Jobs.ReservedCustomerCreditJob(
                serviceProvider.GetRequiredService<IConfiguration>()));
        }
    }
}
