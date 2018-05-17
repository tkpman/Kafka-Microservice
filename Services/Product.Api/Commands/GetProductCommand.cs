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
        //private readonly IUnitOfWork _UnitOfWork;

        //public GetPruductCommandHndler(IUnitOfWork unitOfWork)
        //{
        //    if(unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    this._UnitOfWork = unitOfWork;
        //}

        public async Task<ICommandResult<Application.Models.Product>> Handle(
            GetProductCommand request, 
            CancellationToken cancellationToken)
        {
            Application.Models.Product product = new Application.Models.Product();

            product.ProductId = "Test Discription";
            product.Amount = 99999;
            product.Id = 0;
            product.Name = "Test product";
            product.Price = 99999;

            return CommandResult<Application.Models.Product>.Success(product);
        }
    }
}
