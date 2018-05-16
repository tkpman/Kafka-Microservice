using HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.Application.Background
{
    public class CustomerHostedService
        : JobHostingService
    {
        public CustomerHostedService(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
            this.AddJob(new Jobs.ReserveCustomerCreditJob(
                serviceProvider.GetRequiredService<IConfiguration>()));
        }
    }
}
