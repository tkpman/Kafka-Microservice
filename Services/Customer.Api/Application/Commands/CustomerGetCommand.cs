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
    public class CustomerGetCommand
        : IRequest<Application.Models.Customer>
    {
        public int CustomerId { get; set; }

        public CustomerGetCommand(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            this.CustomerId = id;
        }
    }

    public class CustomerGetCommandHandler
        : IRequestHandler<CustomerGetCommand, Application.Models.Customer>
    {
        //private readonly IUnitOfWork _unitOfWork;

        //public CustomerGetCommandHandler(IUnitOfWork unitOfWork)
        //{
        //    if (unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    this._unitOfWork = unitOfWork;
        //}

        public async Task<Models.Customer> Handle(CustomerGetCommand request, CancellationToken cancellationToken)
        {
            //var customer = this._unitOfWork.GetRepository.FirstOrDefault();
            //return customer;

            throw new NotImplementedException();
        }
    }
}
