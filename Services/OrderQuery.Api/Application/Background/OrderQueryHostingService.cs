using HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace OrderQuery.Api.Application.Background
{
    public class OrderQueryHostingService
        : JobHostingService
    {
        public OrderQueryHostingService(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
            this.AddJob(new Jobs.OrderStateJob(
                serviceProvider.GetRequiredService<IConfiguration>()));
        }
    }
}
