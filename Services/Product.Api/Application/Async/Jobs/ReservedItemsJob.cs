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
using UnitOfWorks.Abstractions;
using UnitOfWorks.EntityFrameworkCore;

namespace Product.Api.Application.Async.Jobs
{
    public class ReserveItemsJob
        : IJob
    {
        private readonly IConfiguration _configuration;

        public ReserveItemsJob(
            IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public void Run(CancellationToken cancellationToken)
        {
            var consumerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", this._configuration["Kafka:BootstrapServers"] },
                { "group.id", Guid.NewGuid() },
                { "schema.registry.url", this._configuration["Kafka:SchemaRegistryUrl"] }
            };

            var producerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", this._configuration["Kafka:BootstrapServers"] },
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                { "schema.registry.url", this._configuration["Kafka:SchemaRegistryUrl"] },
                // optional schema registry client properties:
                { "schema.registry.connection.timeout.ms", 5000 },
                { "schema.registry.max.cached.schemas", 10 },
                // optional avro serializer properties:
                { "avro.serializer.buffer.bytes", 50 },
                { "avro.serializer.auto.register.schemas", true }
            };

            using (var producer = new Producer<string, ReservedItem>(producerConfig, new AvroSerializer<string>(), new AvroSerializer<ReservedItem>()))
            using (var consumer = new Consumer<string, ReserveItems>(consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<ReserveItems>()))
            {
                consumer.OnMessage += (o, e) =>
                {
                    using (var productDbContext = new ProductDbContext())
                    {
                        IUnitOfWork unitOfWork = new UnitOfWork<ProductDbContext>(productDbContext);
                        IRepository<Entities.Order> orderRepository = unitOfWork.GetRepository<Entities.Order>();
                        IRepository<Entities.Product> productRepository = unitOfWork.GetRepository<Entities.Product>();

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
                        foreach (var product in order.Products)
                        {
                            var productInDatabase = productRepository
                                .FirstOrDefault(x => x.ProductId == product.ProductId).Result;

                            if (productInDatabase.Quantity - product.Quantity < 0)
                            {
                                var reservedItemOutOfStock = new ReservedItem()
                                {
                                    OrderId = order.OrderId,
                                    Status = "outofstock"
                                };

                                var reserveItemOutOfStockResult = producer.ProduceAsync(
                                    "reserved-item",
                                    Guid.NewGuid().ToString() + DateTime.Now,
                                    reservedItemOutOfStock).Result;

                                return;
                            }

                            productInDatabase.Quantity -= product.Quantity;

                            productRepository.Update(productInDatabase);
                        }

                        var result = unitOfWork.SaveChanges().Result;

                        if (!result.IsSuccessfull())
                            throw new Exception();

                        var reservedItem = new ReservedItem()
                        {
                            OrderId = order.OrderId,
                            Status = "success"
                        };

                        var reserveItemResult = producer.ProduceAsync(
                            "reserved-item",
                            Guid.NewGuid().ToString() + DateTime.Now,
                            reservedItem).Result;
                    }
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
            }
        }
    }
}
