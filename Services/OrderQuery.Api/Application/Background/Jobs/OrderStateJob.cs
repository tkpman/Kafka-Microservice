using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using HostedServices;
using Microsoft.Extensions.Configuration;
using OrderQuery.Api.Application.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.EntityFrameworkCore;

namespace OrderQuery.Api.Application.Background.Jobs
{
    public class OrderStateJob
        : IJob
    {

        private readonly IConfiguration _configuration;

        private Dictionary<string, object> _consumerConfig;

        private Dictionary<string, object> _producerConfig;

        public OrderStateJob(IConfiguration configuration)
        {
            this._configuration = configuration;

            this._consumerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", this._configuration["Kafka:BootstrapServers"] },
                { "group.id", Guid.NewGuid() },
                { "schema.registry.url", this._configuration["Kafka:SchemaRegistryUrl"] }
            };

            this._producerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", this._configuration["Kafka:BootstrapServers"] },
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                { "schema.registry.url", this._configuration["Kafka:SchemaRegistryUrl"] },
                // optional schema registry client properties:
                { "schema.registry.connection.timeout.ms", int.Parse(this._configuration["Kafka:SchemaRegistryConnectionTimeoutMS"]) },
                { "schema.registry.max.cached.schemas", int.Parse(this._configuration["Kafka:SchemaRegistryMaxCachedSchemas"]) },
                // optional avro serializer properties:
                { "avro.serializer.buffer.bytes", int.Parse(this._configuration["Kafka:AvroSerializerBufferBytes"]) },
                { "avro.serializer.auto.register.schemas", bool.Parse(this._configuration["Kafka:AvroSerializerAutoRegisterSchemas"]) }
            };
        }

        private void OnOrder(object o, Message<string, Order.Api.Application.Events.OrderState> e)
        {
            using (var orderDbContext = new OrderDbContext())
            {
                var unitOfWork = new UnitOfWork<OrderDbContext>(orderDbContext);
                var orderRepository = unitOfWork.GetRepository<Entities.Order>();

                var order = (orderRepository.FirstOrDefault(x => x.OrderId == e.Value.id).Result);
                
                if (order == null)
                {
                    var newOrder = new Entities.Order()
                    {
                        CustomerId = e.Value.customerId,
                        Date = e.Value.date,
                        OrderId = e.Value.id,
                        Total = e.Value.total,
                        Products = e.Value.products.Select(x => new Entities.OrderProduct()
                        {
                            ProductId = x.id,
                            Quantity = x.Quantity
                        }).ToList()
                    };

                    orderRepository.Add(newOrder);
                } else
                {
                    if (e.Value.status.Equals("WaitingForReservation"))
                        order.Status = Entities.Order.OrderStatus.WaitingForReservation;
                    else if (e.Value.status.Equals("WaitingForPayment"))
                        order.Status = Entities.Order.OrderStatus.WaitingForPayment;
                    else if (e.Value.status.Equals("Failed"))
                        order.Status = Entities.Order.OrderStatus.Failed;
                    else if (e.Value.status.Equals("Success"))
                        order.Status = Entities.Order.OrderStatus.Success;

                    orderRepository.Update(order);
                }

                var result = unitOfWork.SaveChanges().Result;

                if (!result.IsSuccessfull())
                    throw new Exception();
                
            }
        }

        private void OnError(object o, Error e) { }

        private void OnConsumeError(object o, Message e) { }

        public void Run(CancellationToken cancellationToken)
        {
            using (var consumer = new Consumer<string, Order.Api.Application.Events.OrderState>(this._consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<Order.Api.Application.Events.OrderState>()))
            {
                // Attach Event handlers to the consumer.
                consumer.OnMessage += (o, e) => OnOrder(o, e);
                consumer.OnError += OnError;
                consumer.OnConsumeError += OnConsumeError;

                // Subscribe for the Order Topic.
                consumer.Subscribe("order-state");

                // Poll messages from Kafka, as long as no cancellation request
                // is send.
                while (!cancellationToken.IsCancellationRequested)
                    consumer.Poll(100);
            }
        }
    }
}
