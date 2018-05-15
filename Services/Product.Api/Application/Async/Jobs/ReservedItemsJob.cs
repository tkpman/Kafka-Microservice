using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using HostedServices;
using Microsoft.Extensions.Configuration;
using Order.Api.Application.Events;
using Product.Api.Application.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;
using UnitOfWorks.EntityFrameworkCore;

namespace Product.Api.Application.Async.Jobs
{
    public class ReserveItemsJob
        : IJob
    {
        private readonly IConfiguration _configuration;
        private readonly ProductDbContext _productDbContext;

        public ReserveItemsJob(
            IConfiguration configuration)
        {
            this._configuration = configuration;
            this._productDbContext = new ProductDbContext();
        }

        public void Run(CancellationToken cancellationToken)
        {
            var consumerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", this._configuration["Kafka:BootstrapServers"] },
                { "group.id", Guid.NewGuid() },
                { "schema.registry.url", this._configuration["Kafka:SchemaRegistryUrl"] }
            };

            using (var consumer = new Consumer<string, ReserveItems>(consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<ReserveItems>()))
            {
                IUnitOfWork unitOfWork = new UnitOfWork<ProductDbContext>(this._productDbContext);
                IRepository<Entities.Order> orderRepository = unitOfWork.GetRepository<Entities.Order>();
                IRepository<Entities.Product> productRepository = unitOfWork.GetRepository<Entities.Product>();

                consumer.OnMessage += (o, e) =>
                {
                    var order = new Entities.Order()
                    {
                        OrderId = e.Value.OrderId,
                        Products = e.Value.products.Select(x => new Entities.OrderProduct()
                        {
                            ProductId = x.id,
                            Quantity = x.Quantity
                        }).ToList()
                    };

                    orderRepository.Add(order);
                    
                    // Very infiencent way of reserving items in database, should only be used
                    // for this project, for an easy way. It is only here for just working
                    // example.
                    foreach(var product in order.Products)
                    {
                        var productInDatabase = productRepository
                            .FirstOrDefault(x => x.ProductId == product.ProductId).Result;

                        if (productInDatabase.Quantity - product.Quantity < 0)
                            throw new Exception();

                        productInDatabase.Quantity -= product.Quantity;

                        productRepository.Update(productInDatabase);
                    }

                    var result = unitOfWork.SaveChanges().Result;

                    if (!result.IsSuccessfull())
                        throw new Exception();
                };

                consumer.OnError += (_, e) =>
                {
                    Console.WriteLine("Error: " + e.Reason);
                };

                consumer.OnConsumeError += (_, e) =>
                {
                    Console.WriteLine("Consume error: " + e.Error.Reason);
                };

                consumer.Subscribe("reserve-items");

                while (!cancellationToken.IsCancellationRequested)
                {
                    consumer.Poll(100);
                }

                this._productDbContext.Dispose();
            }
        }
    }
}
