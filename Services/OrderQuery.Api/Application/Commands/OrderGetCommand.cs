using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using UnitOfWorks.Abstractions;

namespace OrderQuery.Api.Application.Commands
{
    public class OrderGetCommand
        : IRequest<ICommandResult<Models.Order>>
    {
        public OrderGetCommand(string orderId)
        {
            this.OrderId = orderId;
        }

        public string OrderId { get; }
    }

    public class OrderGetCommandHandler
        : IRequestHandler<OrderGetCommand, ICommandResult<Models.Order>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderGetCommandHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<Models.Order>> Handle(
            OrderGetCommand request, 
            CancellationToken cancellationToken)
        {
            var order = await this._unitOfWork
                .GetRepository<Entities.Order>()
                .FirstOrDefault(x => x.OrderId == request.OrderId);

            if (order == null)
                return CommandResult<Models.Order>.Failure("Failed to get order.");

            var model = new Models.Order()
            {
                CustomerId = order.CustomerId,
                Date = order.Date,
                OrderId = order.OrderId,
                Total = order.Total,
                Status = order.Status.ToString(),
                Products = order.Products.Select(p => new Models.Product()
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            };

            return CommandResult<Models.Order>.Success(model);
        }
    }
}
