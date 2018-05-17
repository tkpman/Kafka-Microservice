using Customer.Api.Application.Infrastructure;
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
    public class CustomerAllCommand
        : IRequest<CommandResult<List<Application.Models.Customer>>>
    {
        public CustomerAllCommand() { }
    }

    public class CustomerAllCommandHandler
        : IRequestHandler<CustomerAllCommand, CommandResult<List<Application.Models.Customer>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerAllCommandHandler(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this._unitOfWork = unitOfWork;
        }

        public async Task<CommandResult<List<Models.Customer>>> Handle(
            CustomerAllCommand request, 
            CancellationToken cancellationToken)
        {
            var customers = (await this._unitOfWork
                .GetRepository<Entities.Customer>()
                .All()).Select(x => new Models.Customer()
                {
                    Address = x.Address,
                    Credit = x.Credit,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber
                });

            return CommandResult<List<Models.Customer>>.Success(customers.ToList());
        }
    }
}
