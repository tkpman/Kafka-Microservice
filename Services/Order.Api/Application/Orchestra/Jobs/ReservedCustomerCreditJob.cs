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
    public class ReservedCustomerCreditJob
        : IJob
    {
        private readonly IConfiguration _configuration;

        private readonly Dictionary<string, object> _consumerConfig;

        private readonly Dictionary<string, object> _producerConfig;

        public ReservedCustomerCreditJob(IConfiguration configuration)
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

        private void OnReservedCustomerCredit(
            object o, 
            Message<string, ReservedCustomerCredit> e,
            Producer<string, OrderFailed> producer)
        {
            using (var orderOrchestraDbContext = new OrderOrchestraDbContext())
            {
                IUnitOfWork unitOfWork = new UnitOfWork<Infrastructure.OrderOrchestraDbContext>(orderOrchestraDbContext);
                IRepository<Entities.Order> orderRepository = unitOfWork.GetRepository<Entities.Order>();

                var order = orderRepository.FirstOrDefault(x => x.OrderId == e.Value.OrderId).Result;
                
                if (e.Value.Status.Equals("success"))
                {
                    order.Status = Entities.Order.OrderStatus.Success;
                }
                else
                {
                    order.Status = Entities.Order.OrderStatus.Failed;
                }

                var result = unitOfWork.SaveChanges().Result;

                if (!result.IsSuccessfull())
                    throw new Exception();

                if (e.Value.Status.Equals("success"))
                {
                    // TODO:
                }
                else
                {
                    var orderFailed = new OrderFailed()
                    {
                        OrderId = e.Value.OrderId
                    };

                    var p = producer.ProduceAsync(
                        "order-failed", 
                        Guid.NewGuid().ToString(), 
                        orderFailed).Result;
                }
            }
        }

        private void OnError(object o, Error e) { }

        private void OnConsumeError(object o, Message e) { }

        public void Run(CancellationToken cancellationToken)
        {
            using (var producer = new Producer<string, OrderFailed>(this._producerConfig, new AvroSerializer<string>(), new AvroSerializer<OrderFailed>()))
            using (var consumer = new Consumer<string, ReservedCustomerCredit>(this._consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<ReservedCustomerCredit>()))
            {
                // Add Event listeners.
                consumer.OnMessage += (o, e) => OnReservedCustomerCredit(o, e, producer);
                consumer.OnError += OnError;
                consumer.OnConsumeError += OnConsumeError;

                // Subscribe to the ReservedItem topic.
                consumer.Subscribe("reserved-customer-credit");

                // Poll messages from Kafka aslong as no cancellation is
                // requested.
                while (!cancellationToken.IsCancellationRequested)
                    consumer.Poll(100);
            }
        }
    }
}
