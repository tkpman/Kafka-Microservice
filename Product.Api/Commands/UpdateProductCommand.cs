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
        //private readonly IUnitOfWork _UnitOfWork;

        //public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        //{
        //    if(unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    this._UnitOfWork = unitOfWork;
        //}

        public async Task<ICommandResult<Application.Models.Product>> Handle(
            UpdateProductCommand request, 
            CancellationToken cancellationToken)
        {
            return CommandResult<Application.Models.Product>.Success(request.Product);
        }
    }
}
