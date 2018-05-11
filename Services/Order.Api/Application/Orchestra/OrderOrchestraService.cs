using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Hosting;
using Order.Api.Application.Events;
using Order.Api.Application.Orchestra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;
using UnitOfWorks.EntityFrameworkCore;

namespace Order.Api.Application.Orchestra
{
    public class OrderOrchestraService : IHostedService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._cancellationTokenSource = new CancellationTokenSource();

            this._task = this.OrderCreatedSubscribe(this._cancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._cancellationTokenSource.Cancel();
            return Task.WhenAny(this._task, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        public Task OrderCreatedSubscribe(CancellationToken cancellationToken)
        {
            var consumerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", "192.168.1.63:9092" },
                { "group.id", Guid.NewGuid() },
                { "schema.registry.url", "192.168.1.63:8081" }
            };

            using (var consumer = new Consumer<string, OrderCreated>(consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<OrderCreated>()))
            {
                IUnitOfWork unitOfWork = new UnitOfWork<OrderOrchestraDbContext>(new OrderOrchestraDbContext());
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



                    Console.WriteLine($"user key name: {e.Key}, user value favorite color: {e.Value.customerId}");
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

            return Task.CompletedTask;
        }
    }
}
