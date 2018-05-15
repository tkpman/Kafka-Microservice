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

        public OrderCreatedJob(IConfiguration configuration)
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

            using (var consumer = new Consumer<string, OrderCreated>(consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<OrderCreated>()))
            {
                IUnitOfWork unitOfWork = new UnitOfWork<Infrastructure.OrderOrchestraDbContext>(new OrderOrchestraDbContext());
                IRepository<Entities.Order> orderRepository = unitOfWork.GetRepository<Entities.Order>();

                consumer.OnMessage += (o, e) =>
                {
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

                    orderRepository.Add(order);

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

                consumer.Subscribe("order-created");

                while (!cancellationToken.IsCancellationRequested)
                {
                    consumer.Poll(100);
                }

            }

        }
    }
}
