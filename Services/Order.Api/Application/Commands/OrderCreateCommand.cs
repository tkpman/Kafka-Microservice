using System;
using System.Threading;
using System.Threading.Tasks;
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
            return CommandResult<bool>.Success(true);
        }
    }
}
