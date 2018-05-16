using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Customer.Api.Application.Entities;
using Customer.Api.Application.Infrastructure;
using HostedServices;
using Microsoft.Extensions.Configuration;
using Order.Api.Application.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;
using UnitOfWorks.EntityFrameworkCore;

namespace Customer.Api.Application.Background.Jobs
{
    public class ReserveCustomerCreditJob
        : IJob
    {
        private readonly IConfiguration _configuration;

        private readonly Dictionary<string, object> _consumerConfig;

        private readonly Dictionary<string, object> _producerConfig;

        public ReserveCustomerCreditJob(IConfiguration configuration)
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

        private void OnReserveCustomerCredit(object o, Message<string, ReserveCustomerCredit> e, Producer<string, ReservedCustomerCredit> producer)
        {
            using (var customerDbContext = new CustomerDbContext())
            {
                IUnitOfWork unitOfWork = new UnitOfWork<CustomerDbContext>(customerDbContext);
                IRepository<Entities.Customer> customerRepository = unitOfWork.GetRepository<Entities.Customer>();
                IRepository<Entities.OrderTransaction> orderTransactionRepository = unitOfWork.GetRepository<Entities.OrderTransaction>();

                var customer = customerRepository
                    .FirstOrDefault(x => x.CustomerId == e.Value.OrderId).Result;

                if (customer != null)
                {
                    if (customer.Credit - e.Value.Amount >= 0)
                    {
                        try
                        {
                            using (var transaction = customerDbContext.Database.BeginTransaction())
                            {
                                var orderTransaction = new OrderTransaction()
                                {
                                    CustomerId = customer.Id,
                                    Amount = e.Value.Amount,
                                    OrderId = e.Value.OrderId
                                };

                                orderTransactionRepository.Add(orderTransaction);

                                customer.Credit -= e.Value.Amount;
                                customerRepository.Update(customer);

                                var result = unitOfWork.SaveChanges().Result;

                                if (result.IsSuccessfull())
                                {
                                    transaction.Commit();

                                    var responseOk = new ReservedCustomerCredit()
                                    {
                                        OrderId = e.Value.OrderId,
                                        Status = "success"
                                    };

                                    producer.ProduceAsync(
                                        "reserved-customer-credit",
                                        Guid.NewGuid().ToString(),
                                        responseOk);

                                    return;
                                }
                            }

                        } catch(Exception)
                        {

                        }
                    }
                }

                var responseError = new ReservedCustomerCredit()
                {
                    OrderId = e.Value.OrderId,
                    Status = "failed"
                };

                producer.ProduceAsync(
                    "reserved-customer-credit",
                    Guid.NewGuid().ToString(),
                    responseError);
            }
        }

        private void OnError(object o, Error e) { }

        private void OnConsumeError(object o, Message e) { }

        public void Run(CancellationToken cancellationToken)
        {
            using (var producer = new Producer<string, ReservedCustomerCredit>(this._producerConfig, new AvroSerializer<string>(), new AvroSerializer<ReservedCustomerCredit>()))
            using (var consumer = new Consumer<string, ReserveCustomerCredit>(this._consumerConfig, new AvroDeserializer<string>(), new AvroDeserializer<ReserveCustomerCredit>()))
            {
                // Add Event listeners.
                consumer.OnMessage += (o, e) => OnReserveCustomerCredit(o, e, producer);
                consumer.OnError += OnError;
                consumer.OnConsumeError += OnConsumeError;

                // Subscribe to the ReserveItems topic.
                consumer.Subscribe("reserve-customer-credit");

                // Poll messages from Kafka aslong as no cancellation request
                // is sent.
                while (!cancellationToken.IsCancellationRequested)
                    consumer.Poll(100);
            }
        }
    }
}
