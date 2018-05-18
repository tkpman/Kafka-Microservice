using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderOrchestra
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load configuration.
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            // Build service provider with services.
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var orderOrchestraService = new OrderOrchestraService(serviceProvider);
            orderOrchestraService.StartAsync(cancellationTokenSource.Token);

            var exit = false;

            Console.CancelKeyPress += (o, e) => {
                orderOrchestraService.StopAsync(cancellationTokenSource.Token).Wait();
                exit = true;
            };

            while (!exit)
                Thread.Sleep(1000);
        }
    }
}
