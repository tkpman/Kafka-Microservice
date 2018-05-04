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
    public class CustomerAddCommand : IRequest<Application.Models.Customer>
    {
        public Application.Models.Customer Customer { get; set; }

        public CustomerAddCommand(Application.Models.Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            this.Customer = customer;
        }
    }

    public class CustomerCreateCommandHandler : IRequestHandler<CustomerAddCommand, Application.Models.Customer>
    {
        //private readonly IUnitOfWork _unitOfWork;

        //public CustomerAddCommandHandler(IUnitOfWork unitOfWork)
        //{
        //    if (unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    this._unitOfWork = unitOfWork;
        //}

        public async Task<Models.Customer> Handle(CustomerAddCommand request, CancellationToken cancellationToken)
        {
            //var customer = new Application.Models.Customer(
            //    request.Customer.FirstName,
            //    request.Customer.LastName,
            //    request.Customer.Address,
            //    request.Customer.Email,
            //    request.Customer.PhoneNumber);

            //customer = this._unitOfWork.GetRepository.Add(customer);

            //var result = await this._unitOfWork.SaveChanges(cancellationToken);

            //return result ? customer : null;

            throw new NotImplementedException();
        }
    }
}
