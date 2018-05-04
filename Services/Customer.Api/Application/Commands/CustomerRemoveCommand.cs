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
    public class CustomerRemoveCommand : IRequest<Application.Models.Customer>
    {
        public int CustomerId { get; set; }

        public CustomerRemoveCommand(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            this.CustomerId = id;
        }
    }

    public class CustomerRemoveCommandHandler : IRequestHandler<CustomerRemoveCommand, Application.Models.Customer>
    {
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly IMediator _mediator;

        //public CustomerRemoveCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        //{
        //    if (unitOfWork == null)
        //        throw new ArgumentNullException(nameof(unitOfWork));

        //    if (mediator == null)
        //        throw new ArgumentNullException(nameof(mediator));

        //    this._unitOfWork = unitOfWork;
        //    this._mediator = mediator;
        //}

        public async Task<Models.Customer> Handle(CustomerRemoveCommand request, CancellationToken cancellationToken)
        {
            //var command = new CustomerGetCommand(request.CustomerId);
            //var customerToDelete = await this._mediator.Send(command);

            //if (customerToDelete == null)
            //    return null;

            //var customer = this._unitOfWork.GetRepository.remove(customerToDelete);
            //var result = await this._unitOfWork.SaveChanges(cancellationToken);

            //return result ? customer : null;

            throw new NotImplementedException();
        }
    }
}
