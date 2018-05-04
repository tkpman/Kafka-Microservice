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
    public class CustomerUpdateCommand : IRequest<Application.Models.Customer>
    {
        public Application.Models.Customer Customer { get; set; }

        public CustomerUpdateCommand(Application.Models.Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            this.Customer = customer;
        }
    }

    public class CustomerUpdateCommandHandler
        : IRequestHandler<CustomerUpdateCommand, Application.Models.Customer>

    {
        //private readonly IUnitOfWork _unitOfWork;

        //public CustomerUpdateCommandHandler(IUnitOfWork unitOfWork)
        //{
        //    if (unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    this._unitOfWork = unitOfWork;
        //}

        public async Task<Models.Customer> Handle(CustomerUpdateCommand request, CancellationToken cancellationToken)
        {
            //var updatedCustomer = new Application.Models.Customer(
            //    request.Customer.FirstName,
            //    request.Customer.LastName,
            //    request.Customer.Address,
            //    request.Customer.Email,
            //    request.Customer.PhoneNumber
            //    );

            //var customer = this._unitOfWork.GetRepository.Update(updatedCustomer);
            //var result = await this._unitOfWork.SaveChanges(cancellationToken);

            //return result ? customer : null;

            throw new NotImplementedException();
        }
    }
}
