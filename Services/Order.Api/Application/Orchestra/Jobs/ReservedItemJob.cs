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
    public class ReservedItemJob
        : IJob
    {
        private readonly IConfiguration _configuration;

        public ReservedItemJob(IConfiguration configuration)
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

            using (var consumer = new Consumer<string, ReservedItem>(consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<ReservedItem>()))
            {
                consumer.OnMessage += (o, e) =>
                {
                    using (var orderOrchestraDbContext = new OrderOrchestraDbContext())
                    {

                        IUnitOfWork unitOfWork = new UnitOfWork<Infrastructure.OrderOrchestraDbContext>(orderOrchestraDbContext);
                        IRepository<Entities.Order> orderRepository = unitOfWork.GetRepository<Entities.Order>();

                        // Get the Order from the repository.
                        var order = orderRepository.FirstOrDefault(x => x.OrderId == e.Value.OrderId).Result;

                        if (order == null)
                            throw new Exception();

                        if (e.Value.Status.Equals("success"))
                        {
                            order.Status = Entities.Order.OrderStatus.Success;

                        }
                        else if (e.Value.Equals("outofstock"))
                        {
                            order.Status = Entities.Order.OrderStatus.Failed;
                        }

                        // Save the order entity to the repository.
                        orderRepository.Update(order);
                        var result = unitOfWork.SaveChanges().Result;

                        // If save fails, throw an exception.
                        if (!result.IsSuccessfull())
                            throw new Exception();
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

                consumer.Subscribe("reserved-item");

                while (!cancellationToken.IsCancellationRequested)
                {
                    consumer.Poll(100);
                }
            }
        }
    }
}
