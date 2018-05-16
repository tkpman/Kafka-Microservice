using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MediatR;
using Order.Api.Application.Events;
using Order.Api.Application.Models;
using System.Linq;

namespace Order.Api.Application.Commands
{
    public class OrderCreateCommand
        : IRequest<ICommandResult<bool>>
    {
        public OrderCreateCommand(NewOrder newOrder)
        {
            if (newOrder == null)
                throw new ArgumentNullException(nameof(newOrder));

            this.NewOrder = newOrder;
        }

        public NewOrder NewOrder { get; }
    }

    public class OrderCreateCommandHandler
        : IRequestHandler<OrderCreateCommand, ICommandResult<bool>>
    {
        public async Task<ICommandResult<bool>> Handle(
            OrderCreateCommand request,
            CancellationToken cancellationToken)
        {
            //var config = new Dictionary<string, object>
            //{
            //    { "bootstrap.servers", "192.168.1.63:9092" }
            //};

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", "192.168.1.63:9092" },
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                { "schema.registry.url", "192.168.1.63:8081" },
                // optional schema registry client properties:
                { "schema.registry.connection.timeout.ms", 5000 },
                { "schema.registry.max.cached.schemas", 10 },
                // optional avro serializer properties:
                { "avro.serializer.buffer.bytes", 50 },
                { "avro.serializer.auto.register.schemas", true }
            };


            //using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            using (var producer = new Producer<string, OrderCreated>(config, new AvroSerializer<string>(), new AvroSerializer<OrderCreated>()))
            {
                var orderCreated = new OrderCreated()
                {
                    id = Guid.NewGuid().ToString(),
                    customerId = request.NewOrder.CustomerId,
                    date = DateTime.Now.ToString(),
                    products = request.NewOrder.Products.Select(x => new OrderProduct()
                    {
                        id = x.Id,
                        Quantity = x.Quantity
                    }).ToList()
                };

                var dr = await producer.ProduceAsync("order-created", orderCreated.id, orderCreated);
                Console.WriteLine($"Delivered '{dr.Value}' to: {dr.TopicPartitionOffset}");
            }

            return CommandResult<bool>.Success(true);
        }
    }
}
