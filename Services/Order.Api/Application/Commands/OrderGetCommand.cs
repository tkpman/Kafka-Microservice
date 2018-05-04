using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Order.Api.Application.Commands
{
    public class OrderGetCommand
        : IRequest<ICommandResult<Models.Order>>
    {
        public OrderGetCommand(int orderId)
        {
            this.OrderId = orderId;
        }

        public int OrderId { get; }
    }

    public class OrderGetCommandHandler
        : IRequestHandler<OrderGetCommand, ICommandResult<Models.Order>>
    {
        public async Task<ICommandResult<Models.Order>> Handle(
            OrderGetCommand request, 
            CancellationToken cancellationToken)
        {
            return CommandResult<Models.Order>.Success(new Models.Order());
        }
    }
}
