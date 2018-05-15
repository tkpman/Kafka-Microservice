using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using HostedServices;
using Microsoft.Extensions.Configuration;
using Order.Api.Application.Events;
using Order.Api.Application.Orchestra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;
using UnitOfWorks.EntityFrameworkCore;

namespace Order.Api.Application.Orchestra.Jobs
{
    public class OrderCreatedJob
        : IJob
    {
        private readonly IConfiguration _configuration;
        private readonly OrderOrchestraDbContext _orderOrchestraDbContext;

        public OrderCreatedJob(
            IConfiguration configuration)
        {
            this._configuration = configuration;
            this._orderOrchestraDbContext = new OrderOrchestraDbContext();
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

            using (var producer = new Producer<string, ReserveItems>(producerConfig, new AvroSerializer<string>(), new AvroSerializer<ReserveItems>()))
            using (var consumer = new Consumer<string, OrderCreated>(consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<OrderCreated>()))
            {
                IUnitOfWork unitOfWork = new UnitOfWork<Infrastructure.OrderOrchestraDbContext>(this._orderOrchestraDbContext);
                IRepository<Entities.Order> orderRepository = unitOfWork.GetRepository<Entities.Order>();

                consumer.OnMessage += (o, e) =>
                {
                    // Converts OrderCreated to an order entity.
                    var order = new Entities.Order()
                    {
                        OrderId = e.Value.id,
                        CustomerId = e.Value.customerId,
                        Date = DateTime.Parse(e.Value.date),
                        Products = e.Value.products.Select(x => new Entities.OrderProduct()
                        {
                            ProductId = x.id,
                            Quantity = x.Quantity
                        }).ToList()
                    };

                    // Save the order entity to the repository.
                    orderRepository.Add(order);
                    var result = unitOfWork.SaveChanges().Result;

                    // If save fails, throw an exception.
                    if (!result.IsSuccessfull())
                        throw new Exception();

                    var reserveItems = new ReserveItems()
                    {
                        OrderId = order.OrderId,
                        products = order.Products.Select(x => new ReserveItemProduct()
                        {
                            id = x.ProductId,
                            Quantity = x.Quantity
                        }).ToList()
                    };

                    var reserveItemsResult = producer.ProduceAsync(
                        "reserve-items", 
                        Guid.NewGuid().ToString() + DateTime.Now, 
                        reserveItems).Result;
                };

                consumer.OnError += (_, e) =>
                {
                    Console.WriteLine("Error: " + e.Reason);
                };

                consumer.OnConsumeError += (_, e) =>
                {
                    Console.WriteLine("Consume error: " + e.Error.Reason);
                };

                consumer.Subscribe("order-created");

                while (!cancellationToken.IsCancellationRequested)
                {
                    consumer.Poll(100);
                }

                this._orderOrchestraDbContext.Dispose();
            }

        }
    }
}
