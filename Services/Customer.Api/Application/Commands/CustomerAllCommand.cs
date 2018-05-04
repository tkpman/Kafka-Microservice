using Customer.Api.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;

namespace Customer.Api.Application.Commands
{
    public class CustomerAllCommand
        : IRequest<List<Application.Models.Customer>>
    {
        public CustomerAllCommand() { }
    }

    public class CustomerAllCommandHandler
        : IRequestHandler<CustomerAllCommand, List<Application.Models.Customer>>
    {
        //private readonly IUnitOfWork _unitOfWork;

        //public CustomerAllCommandHandler(IUnitOfWork unitOfWork)
        //{
        //    if (unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    this._unitOfWork = unitOfWork;
        //}

        async Task<List<Models.Customer>> IRequestHandler<CustomerAllCommand, List<Models.Customer>>.Handle(CustomerAllCommand request, CancellationToken cancellationToken)
        {
            //var customers = await this._unitOfWork.GetRepository.All<Application.Models.Customer>();
            //return customers.ToList();

            throw new NotImplementedException();
        }
    }
}
