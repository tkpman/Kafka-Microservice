using HostedServices;
using Product.Api.Application.Async.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Product.Api.Application.Infrastructure;

namespace Product.Api.Application.Async
{
    public class ProductHostedService
        : JobHostingService
    {
        public ProductHostedService(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
            this.AddJob(new ReserveOrderJob(
                serviceProvider.GetRequiredService<IConfiguration>()));
        }
    }
}
