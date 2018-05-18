using MediatR;
using Order.Api.Application.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;

namespace Customer.Api.Application.Commands
{
    public class CustomerUpdateCommand : IRequest<ICommandResult<Application.Models.Customer>>
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
        : IRequestHandler<CustomerUpdateCommand, ICommandResult<Application.Models.Customer>>

    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerUpdateCommandHandler(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this._unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<Application.Models.Customer>> Handle(CustomerUpdateCommand request, CancellationToken cancellationToken)
        {
            var repo = this._unitOfWork.GetRepository<Application.Entities.Customer>();

            var customerToUpdate = await repo.FirstOrDefault(x => x.Id == request.Customer.Id);

            if (customerToUpdate == null)
                return CommandResult<Application.Models.Customer>.Failure("could not find customer to update");

            customerToUpdate.FirstName = request.Customer.FirstName;
            customerToUpdate.LastName = request.Customer.LastName;
            customerToUpdate.Address = request.Customer.Address;
            customerToUpdate.Email = request.Customer.Email;
            customerToUpdate.PhoneNumber = request.Customer.PhoneNumber;
            customerToUpdate.Credit = request.Customer.Credit;

            repo.Update(customerToUpdate);

            var result = await this._unitOfWork.SaveChanges(cancellationToken);

            if (!result.IsSuccessfull())
                return CommandResult<Application.Models.Customer>.Failure("Error when creating Customer");

            return CommandResult<Application.Models.Customer>.Success(request.Customer);

        }
    }
}
