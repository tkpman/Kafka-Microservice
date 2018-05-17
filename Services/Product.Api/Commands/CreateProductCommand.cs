using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Order.Api.Application.Commands;
using Product.Api.Application.Infrastructure;
using Product.Api.Application.Models;
using UnitOfWorks.Abstractions;

namespace Product.Api.Commands
{
    public class CreateProductCommand : IRequest<ICommandResult<Application.Models.Product>>
    {
        public CreateProductCommand(Application.Models.NewProduct newProduct)
        {
            if(newProduct == null)
                throw new ArgumentNullException(nameof(newProduct));

            this.NewProduct = newProduct;
        }

        public Application.Models.NewProduct NewProduct { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ICommandResult<Application.Models.Product>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this._unitOfWork = unitOfWork;
        }
        public async Task<ICommandResult<Application.Models.Product>> Handle(
            CreateProductCommand request, 
            CancellationToken cancellationToken)
        {

            var product = new Application.Entities.Product();

            product.Name = request.NewProduct.Name;
            product.Price = request.NewProduct.Price;
            product.ProductId = request.NewProduct.ProductId;
            product.Quantity = request.NewProduct.Amount;

            var repo = this._unitOfWork.GetRepository<Application.Entities.Product>();

            product = repo.Add(product);

            var result = await this._unitOfWork.SaveChanges(cancellationToken);

            if (!result.IsSuccessfull())            
                return CommandResult<Application.Models.Product>.Failure("Error when creating product");
            
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
