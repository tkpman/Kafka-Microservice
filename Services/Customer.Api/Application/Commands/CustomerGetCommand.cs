using Customer.Api.Application.Models;
using MediatR;
using Order.Api.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;

namespace Customer.Api.Application.Commands
{
    public class CustomerGetCommand
        : IRequest<CommandResult<Application.Models.Customer>>
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
        : IRequestHandler<CustomerGetCommand, CommandResult<Application.Models.Customer>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerGetCommandHandler(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this._unitOfWork = unitOfWork;
        }

        public async Task<CommandResult<Models.Customer>> Handle(CustomerGetCommand request, CancellationToken cancellationToken)
        {
            var customer = await this._unitOfWork
                .GetRepository<Entities.Customer>()
                .FirstOrDefault(x => x.Id == request.CustomerId);

            if (customer == null)
                return CommandResult<Models.Customer>.Failure(
                    $"Failed to find customer with id {request.CustomerId}");

            var model = new Models.Customer()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                Credit = customer.Credit,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber
            };

            return CommandResult<Models.Customer>.Success(model);
        }
    }
}
