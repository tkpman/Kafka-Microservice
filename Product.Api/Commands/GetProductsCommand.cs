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
    public class GetProductsCommand : 
        IRequest<ICommandResult<List<Application.Models.Product>>>
    {
    }

    public class GetProductsCommandHandler : 
        IRequestHandler<GetProductsCommand, 
        ICommandResult<List<Application.Models.Product>>>
    {
        //private readonly IUnitOfWork _unitOfWork;

        //public GetProductsCommandHandler(IUnitOfWork unitOfWork)
        //{
        //    if(unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    this._unitOfWork = unitOfWork;
        //}

        public async Task<ICommandResult<List<Application.Models.Product>>> Handle(GetProductsCommand request, CancellationToken cancellationToken)
        {
            var products = new List<Application.Models.Product>();

            Application.Models.Product product = new Application.Models.Product();

            product.Description = "Test Discription";
            product.Amount = 99999;
            product.Id = 0;
            product.Name = "Test product";
            product.Price = 99999;

            Application.Models.Product product1 = new Application.Models.Product();

            product1.Description = "Test Discription";
            product1.Amount = 99999;
            product1.Id = 0;
            product1.Name = "Test product";
            product1.Price = 99999;

            products.Add(product);
            products.Add(product1);

            return CommandResult<List<Application.Models.Product>>.Success(products);
        }
    }
}
