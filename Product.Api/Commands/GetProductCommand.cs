using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using UnitOfWorks.Abstractions;

namespace Product.Api.Commands
{
    public class GetProductCommand : IRequest<Application.Models.Product>
    {
        public GetProductCommand(Application.Models.Product product)
        {
            if(product == null)
                throw new ArgumentNullException(nameof(product));

            this.Product = product;
        }
        public Application.Models.Product Product { get; set; }
    }

    public class GetPruductCommandHndler : IRequestHandler<GetProductCommand, Application.Models.Product>
    {
        //private readonly IUnitOfWork _UnitOfWork;

        //public GetPruductCommandHndler(IUnitOfWork unitOfWork)
        //{
        //    if(unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    this._UnitOfWork = unitOfWork;
        //}

        public Task<Application.Models.Product> Handle(GetProductCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
