using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using UnitOfWorks.Abstractions;

namespace OrderQuery.Api.Application.Commands
{
    public class OrderAllCommand
        : IRequest<ICommandResult<List<Models.Order>>>
    {
        public OrderAllCommand()
        { }
    }

    public class OrderAllCommandHandler
        : IRequestHandler<OrderAllCommand, ICommandResult<List<Models.Order>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderAllCommandHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<List<Models.Order>>> Handle(
            OrderAllCommand request, 
            CancellationToken cancellationToken)
        {
            var orders = await this._unitOfWork
                .GetRepository<Entities.Order>()
                .All();

            var models = orders.Select(x => new Models.Order()
            {
                CustomerId = x.CustomerId,
                Date = x.Date,
                OrderId = x.OrderId,
                Total = x.Total,
                Products = x.Products.Select(p => new Models.Product()
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            }).ToList();

            return CommandResult<List<Models.Order>>.Success(models);
        }
    }
}
