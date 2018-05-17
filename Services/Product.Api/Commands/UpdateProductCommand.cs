using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Order.Api.Application.Commands;
using UnitOfWorks.Abstractions;

namespace Product.Api.Commands
{
    public class UpdateProductCommand : IRequest<ICommandResult<Application.Models.Product>>
    {
        public UpdateProductCommand(Application.Models.Product product)
        {
            if(product == null)
                throw new ArgumentNullException(nameof(product));

            this.Product = product;
        }

        public Application.Models.Product Product { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ICommandResult<Application.Models.Product>>
    {
        private readonly IUnitOfWork _UnitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this._UnitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<Application.Models.Product>> Handle(
            UpdateProductCommand request, 
            CancellationToken cancellationToken)
        {
            var repo = this._UnitOfWork.GetRepository<Application.Entities.Product>();

            var productToUpdate = await repo.FirstOrDefault(x => x.Id == request.Product.Id);

            if (productToUpdate == null)
                return CommandResult<Application.Models.Product>.Failure("could not find product to update");

            productToUpdate.Name = request.Product.Name;
            productToUpdate.Price = request.Product.Price;
            productToUpdate.ProductId = request.Product.ProductId;
            productToUpdate.Quantity = request.Product.Amount;

            repo.Update(productToUpdate);

            var result = await this._UnitOfWork.SaveChanges(cancellationToken);

            if (!result.IsSuccessfull())
                return CommandResult<Application.Models.Product>.Failure("could not update");
            
            return CommandResult<Application.Models.Product>.Success(request.Product);
        }
    }
}
