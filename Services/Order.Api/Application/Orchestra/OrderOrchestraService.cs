using HostedServices;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Order.Api.Application.Orchestra
{
    public class OrderOrchestraServiceBeta
        : JobHostingService
    {
        public OrderOrchestraServiceBeta(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.AddJob(new Jobs.OrderCreatedJob(
                serviceProvider.GetRequiredService<IConfiguration>()));
        }
    }
}
