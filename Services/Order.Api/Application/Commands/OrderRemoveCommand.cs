using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Order.Api.Application.Commands
{
    public class OrderRemoveCommand
        : IRequest<ICommandResult<bool>>
    {
        public OrderRemoveCommand(int orderId)
        {
            this.OrderId = orderId;
        }

        public int OrderId { get; }
    }

    public class OrderRemoveCommandHandler
        : IRequestHandler<OrderRemoveCommand, ICommandResult<bool>>
    {
        public async Task<ICommandResult<bool>> Handle(
            OrderRemoveCommand request, 
            CancellationToken cancellationToken)
        {
            return CommandResult<bool>.Success(true);
        }
    }
}
