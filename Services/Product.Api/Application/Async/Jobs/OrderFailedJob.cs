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
    public class OrderFailedJob
        : IJob
    {
        private readonly IConfiguration _configuration;

        private Dictionary<string, object> _consumerConfig;

        private Dictionary<string, object> _producerConfig;

        public OrderFailedJob(IConfiguration configuration)
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

        private void OnOrderFailed(object o, Message<string, OrderFailed> e)
        {
            using (var productDbContext = new ProductDbContext())
            {
                IUnitOfWork unitOfWork = new UnitOfWork<ProductDbContext>(productDbContext);
                IRepository<Entities.Order> orderRepository = unitOfWork.GetRepository<Entities.Order>();
                IRepository<Entities.Product> productRepository = unitOfWork.GetRepository<Entities.Product>();

                // Get the order from the database and include all the OrderProducts.
                var orderSpecification = new Specification<Entities.Order>();
                orderSpecification.Include(x => x.Products);
                var order = orderRepository.FirstOrDefault(x => x.OrderId == e.Value.OrderId, orderSpecification).Result;

                // If order do exist.
                if (order != null)
                {
                    foreach(var orderProduct in order.Products)
                    {
                        var product = productRepository
                            .FirstOrDefault(x => x.ProductId == orderProduct.ProductId).Result;

                        if (product == null)
                            continue;

                        product.Quantity += orderProduct.Quantity;

                        productRepository.Update(product);
                    }

                    orderRepository.Remove(order);

                    var result = unitOfWork.SaveChanges().Result;

                    if (!result.IsSuccessfull())
                        throw new Exception();
                }
            }
        }

        private void OnError(object o, Error e) { }

        private void OnConsumeError(object o, Message e) { }

        public void Run(CancellationToken cancellationToken)
        {
            using (var consumer = new Consumer<string, OrderFailed>(this._consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<OrderFailed>()))
            {
                // Attach Event handlers to the consumer.
                consumer.OnMessage += OnOrderFailed;
                consumer.OnError += OnError;
                consumer.OnConsumeError += OnConsumeError;

                // Subscribe for the Ordercreated Topic.
                consumer.Subscribe("order-failed");

                // Poll messages from Kafka, as long as no cancellation request
                // is send.
                while (!cancellationToken.IsCancellationRequested)
                    consumer.Poll(100);
            }
        }
    }
}
