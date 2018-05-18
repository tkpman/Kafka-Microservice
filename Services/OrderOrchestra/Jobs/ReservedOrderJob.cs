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
using UnitOfWorks.Abstractions;
using UnitOfWorks.EntityFrameworkCore;

namespace OrderOrchestra.Jobs
{
    public class ReservedOrderJob
        : IJob
    {
        private readonly IConfiguration _configuration;

        private readonly Dictionary<string, object> _consumerConfig;

        private readonly Dictionary<string, object> _producerConfig;

        public ReservedOrderJob(IConfiguration configuration)
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

        private void OnReservedOrder(
            object o, Message<string, 
            ReservedOrder> e, 
            Producer<string, ReserveCustomerCredit> reserveCustomerCreditProducer, 
            Producer<string, OrderFailed> orderFailedProducer,
            Producer<string, OrderState> orderStateProducer)
        {
            using (var orderOrchestraDbContext = new OrderOrchestraDbContext())
            {

                IUnitOfWork unitOfWork = new UnitOfWork<Infrastructure.OrderOrchestraDbContext>(orderOrchestraDbContext);
                IRepository<Entities.Order> orderRepository = unitOfWork.GetRepository<Entities.Order>();

                var specification = new Specification<Entities.Order>();
                specification.Include(x => x.Products);
                // Get the Order from the repository.
                var order = orderRepository.FirstOrDefault(x => x.OrderId == e.Value.OrderId, specification).Result;

                if (order == null)
                    throw new Exception();

                if (e.Value.Status.Equals("success"))
                {
                    order.Status = Entities.Order.OrderStatus.WaitingForPayment;
                    order.Total = e.Value.Total;

                }
                else if (e.Value.Status.Equals("outofstock"))
                {
                    order.Status = Entities.Order.OrderStatus.Failed;
                }

                // Save the order entity to the repository.
                orderRepository.Update(order);
                var result = unitOfWork.SaveChanges().Result;

                // If save fails, throw an exception.
                if (!result.IsSuccessfull())
                    throw new Exception();

                var orderState = new OrderState()
                {
                    id = order.OrderId,
                    customerId = order.CustomerId,
                    date = order.Date.ToString(),
                    status = order.Status.ToString(),
                    total = order.Total,
                    products = order.Products.Select(x => new OrderProduct()
                    {
                        id = x.ProductId,
                        Quantity = x.Quantity
                    }).ToList()
                };

                var orderStateResult = orderStateProducer.ProduceAsync(
                    "order-state",
                    Guid.NewGuid().ToString(),
                    orderState).Result;

                if (e.Value.Status.Equals("success"))
                {
                    var reserveCustomerCredit = new ReserveCustomerCredit()
                    {
                        OrderId = e.Value.OrderId,
                        Amount = e.Value.Total,
                        CustomerId = order.CustomerId
                    };

                    var r = reserveCustomerCreditProducer.ProduceAsync(
                        "reserve-customer-credit", 
                        Guid.NewGuid().ToString(), 
                        reserveCustomerCredit).Result;
                }
                else if (e.Value.Status.Equals("outofstock"))
                {
                    var orderFailed = new OrderFailed()
                    {
                        OrderId = e.Value.OrderId
                    };

                    var r = orderFailedProducer.ProduceAsync(
                        "order-failed", Guid.NewGuid().ToString(), orderFailed).Result;
                }
            }
        }

        private void OnError(object o, Error e) { }

        private void OnConsumeError(object o, Message e) { }

        public void Run(CancellationToken cancellationToken)
        {
            using (var orderStateProducer = new Producer<string, OrderState>(this._producerConfig, new AvroSerializer<string>(), new AvroSerializer<OrderState>()))
            using (var reserveCustomerCreditProducer = new Producer<string, ReserveCustomerCredit>(this._producerConfig, new AvroSerializer<string>(), new AvroSerializer<ReserveCustomerCredit>()))
            using (var orderFailedProducer = new Producer<string, OrderFailed>(this._producerConfig, new AvroSerializer<string>(), new AvroSerializer<OrderFailed>()))
            using (var consumer = new Consumer<string, ReservedOrder>(this._consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<ReservedOrder>()))
            {
                // Add Event listeners.
                consumer.OnMessage += (o, e) => OnReservedOrder(o, e, reserveCustomerCreditProducer, orderFailedProducer, orderStateProducer);
                consumer.OnError += OnError;
                consumer.OnConsumeError += OnConsumeError;

                // Subscribe to the ReservedItem topic.
                consumer.Subscribe("reserved-order");

                // Poll messages from Kafka aslong as no cancellation is
                // requested.
                while (!cancellationToken.IsCancellationRequested)
                    consumer.Poll(100);
            }
        }
    }
}
