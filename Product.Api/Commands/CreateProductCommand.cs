using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Order.Api.Application.Commands;
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
        //private readonly IUnitOfWork _unitOfWork;

        //public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        //{
        //    if(unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    this._unitOfWork = unitOfWork;
        //}
        public async Task<ICommandResult<Application.Models.Product>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Application.Models.Product();

            product.Description = request.NewProduct.Description;
            product.Amount = request.NewProduct.Amount;
            product.Name = request.NewProduct.Name;
            product.Id = 1;
            product.Price = request.NewProduct.Price;

            return CommandResult<Application.Models.Product>.Success(product);
        }
    }
}
