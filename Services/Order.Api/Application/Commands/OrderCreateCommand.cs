using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MediatR;
using Order.Api.Application.Models;

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

        private NewOrder NewOrder { get; }
    }

    public class OrderCreateCommandHandler
        : IRequestHandler<OrderCreateCommand, ICommandResult<bool>>
    {
        public async Task<ICommandResult<bool>> Handle(
            OrderCreateCommand request,
            CancellationToken cancellationToken)
        {
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", "localhost:9092" }
            };

            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                var dr = producer.ProduceAsync("my-topic", null, "test message text").Result;
                Console.WriteLine($"Delivered '{dr.Value}' to: {dr.TopicPartitionOffset}");
            }

            return CommandResult<bool>.Success(true);
        }
    }
}
