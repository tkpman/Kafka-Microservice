using HostedServices;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderOrchestra
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
