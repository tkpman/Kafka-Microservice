using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using HostedServices;
using Microsoft.Extensions.Configuration;
using Order.Api.Application.Events;
using OrderOrchestra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;
using UnitOfWorks.EntityFrameworkCore;

namespace OrderOrchestra.Jobs
{
    public class OrderCreatedJob
        : IJob
    {
        private readonly IConfiguration _configuration;

        private Dictionary<string, object> _consumerConfig;

        private Dictionary<string, object> _producerConfig;

        public OrderCreatedJob(
            IConfiguration configuration)
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

        private void OnCreated(object o, Message<string, OrderCreated> e, Producer<string, ReserveOrder> producer)
        {
            using (var orderOrchestraDbContext = new OrderOrchestraDbContext())
            {

                IUnitOfWork unitOfWork = new UnitOfWork<Infrastructure.OrderOrchestraDbContext>(orderOrchestraDbContext);
                IRepository<Entities.Order> orderRepository = unitOfWork.GetRepository<Entities.Order>();

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

                var reserveItems = new ReserveOrder()
                {
                    OrderId = order.OrderId,
                    products = order.Products.Select(x => new ReserveItemProduct()
                    {
                        id = x.ProductId,
                        Quantity = x.Quantity
                    }).ToList()
                };

                var reserveItemsResult = producer.ProduceAsync(
                    "reserve-order",
                    Guid.NewGuid().ToString() + DateTime.Now,
                    reserveItems).Result;
            }
        }

        private void OnError(object o, Error e) { }

        private void OnConsumeError(object o, Message e) { }

        public void Run(CancellationToken cancellationToken)
        {
            using (var producer = new Producer<string, ReserveOrder>(this._producerConfig, new AvroSerializer<string>(), new AvroSerializer<ReserveOrder>()))
            using (var consumer = new Consumer<string, OrderCreated>(this._consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<OrderCreated>()))
            {
                // Attach Event handlers to the consumer.
                consumer.OnMessage += (o, e) => OnCreated(o, e, producer);
                consumer.OnError +=  OnError;
                consumer.OnConsumeError += OnConsumeError;

                // Subscribe for the Ordercreated Topic.
                consumer.Subscribe("order-created");

                // Poll messages from Kafka, as long as no cancellation request
                // is send.
                while (!cancellationToken.IsCancellationRequested)
                    consumer.Poll(100);
            }

        }
    }
}
