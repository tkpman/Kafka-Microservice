using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.CodeAnalysis.CSharp;
using Order.Api.Application.Commands;
using UnitOfWorks.Abstractions;

namespace Product.Api.Commands
{
    public class GetProductCommand : IRequest<ICommandResult<Application.Models.Product>>
    {
        public GetProductCommand(int id)
        {
            if(id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            this.Id = id;
        }
        public int Id { get; set; }
    }

    public class GetPruductCommandHndler : 
        IRequestHandler<GetProductCommand, 
        ICommandResult<Application.Models.Product>>
    {
        private readonly IUnitOfWork _UnitOfWork;

        public GetPruductCommandHndler(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this._UnitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<Application.Models.Product>> Handle(
            GetProductCommand request, 
            CancellationToken cancellationToken)
        {
            var repo = this._UnitOfWork.GetRepository<Application.Entities.Product>();

            var product = await repo.FirstOrDefault(x => x.Id == request.Id);

            if (product == null)
                return CommandResult<Application.Models.Product>.Failure("Could not find product");

            var model = new Application.Models.Product();
            model.Name = product.Name;
            model.Amount = product.Quantity;
            model.Id = product.Id;
            model.Price = product.Price;
            model.ProductId = product.ProductId;

            return CommandResult<Application.Models.Product>.Success(model);
        }
    }
}
