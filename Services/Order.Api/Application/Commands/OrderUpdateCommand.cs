using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Order.Api.Application.Models;

namespace Order.Api.Application.Commands
{
    public class OrderUpdateCommand
        : IRequest<ICommandResult<bool>>
    {
        public OrderUpdateCommand(UpdatedOrder updatedOrder)
        {
            if (updatedOrder == null)
                throw new ArgumentNullException(nameof(updatedOrder));

            this.UpdatedOrder = updatedOrder;
        }

        public UpdatedOrder UpdatedOrder { get; }
    }

    public class OrderUpdatedCommandHandler
        : IRequestHandler<OrderUpdateCommand, ICommandResult<bool>>
    {
        public async Task<ICommandResult<bool>> Handle(
            OrderUpdateCommand request, 
            CancellationToken cancellationToken)
        {
            return CommandResult<bool>.Success(true);
        }
    }
}
