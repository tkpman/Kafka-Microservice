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
    public class ReserveOrderJob
        : IJob
    {
        private readonly IConfiguration _configuration;

        private readonly Dictionary<string, object> _consumerConfig;

        private readonly Dictionary<string, object> _producerConfig;

        public ReserveOrderJob(
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

        private void OnReserveOrder(object o, Message<string, ReserveOrder> e, Producer<string, ReservedOrder> producer)
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

                double totalPrice = 0;

                // Very infiencent way of reserving items in database, should only be used
                // for this project, for an easy way. It is only here for just working
                // example.
                foreach (var product in order.Products)
                {
                    var productInDatabase = productRepository
                        .FirstOrDefault(x => x.ProductId == product.ProductId).Result;

                    if (productInDatabase.Quantity - product.Quantity < 0)
                    {
                        var reservedItemOutOfStock = new ReservedOrder()
                        {
                            OrderId = order.OrderId,
                            Status = "outofstock"
                        };

                        var reserveItemOutOfStockResult = producer.ProduceAsync(
                            "reserved-order",
                            Guid.NewGuid().ToString() + DateTime.Now,
                            reservedItemOutOfStock).Result;

                        return;
                    }

                    productInDatabase.Quantity -= product.Quantity;

                    productRepository.Update(productInDatabase);

                    totalPrice += productInDatabase.Price;
                }

                var result = unitOfWork.SaveChanges().Result;

                if (!result.IsSuccessfull())
                    throw new Exception();

                var reservedItem = new ReservedOrder()
                {
                    OrderId = order.OrderId,
                    Status = "success",
                    Total = totalPrice
                };

                var reserveItemResult = producer.ProduceAsync(
                    "reserved-order",
                    Guid.NewGuid().ToString() + DateTime.Now,
                    reservedItem).Result;
            }
        }

        private void OnError(object o, Error e) { }

        private void OnConsumeError(object o, Message e) { }

        public void Run(CancellationToken cancellationToken)
        {
            using (var producer = new Producer<string, ReservedOrder>(this._producerConfig, new AvroSerializer<string>(), new AvroSerializer<ReservedOrder>()))
            using (var consumer = new Consumer<string, ReserveOrder>(this._consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<ReserveOrder>()))
            {
                // Add Event listeners.
                consumer.OnMessage += (o, e) => OnReserveOrder(o, e, producer);
                consumer.OnError += OnError;
                consumer.OnConsumeError += OnConsumeError;

                // Subscribe to the ReserveItems topic.
                consumer.Subscribe("reserve-order");

                // Poll messages from Kafka aslong as no cancellation request
                // is sent.
                while (!cancellationToken.IsCancellationRequested)
                    consumer.Poll(100);
            }
        }
    }
}
