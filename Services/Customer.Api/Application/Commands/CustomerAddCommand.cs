using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;
using Order.Api.Application.Commands;

namespace Customer.Api.Application.Commands
{
    public class CustomerAddCommand : IRequest<ICommandResult<Application.Models.Customer>>
    {
        public Application.Models.NewCustomer NewCustomer { get; set; }

        public CustomerAddCommand(Application.Models.NewCustomer newCustomer)
        {
            if (newCustomer == null)
                throw new ArgumentNullException(nameof(newCustomer));

            this.NewCustomer = newCustomer;
        }
    }

    public class CustomerCreateCommandHandler : IRequestHandler<CustomerAddCommand, ICommandResult<Application.Models.Customer>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerCreateCommandHandler(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this._unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<Application.Models.Customer>> Handle(CustomerAddCommand request, CancellationToken cancellationToken)
        {
            var customer = new Application.Entities.Customer();
            customer.FirstName = request.NewCustomer.FirstName;
            customer.LastName = request.NewCustomer.LastName;
            customer.Address = request.NewCustomer.Address;
            customer.Email = request.NewCustomer.Email;
            customer.PhoneNumber = request.NewCustomer.PhoneNumber;
            customer.Credit = request.NewCustomer.Credit;

            var repo = this._unitOfWork.GetRepository<Application.Entities.Customer>();

            customer = repo.Add(customer);

            var result = await this._unitOfWork.SaveChanges(cancellationToken);

            if (!result.IsSuccessfull())
                return CommandResult<Application.Models.Customer>.Failure("Error when creating Customer");

            var model = new Application.Models.Customer();
            model.Id = customer.Id;
            model.FirstName = customer.FirstName;
            model.LastName = customer.LastName;
            model.Address = customer.Address;
            model.Email = customer.Email;
            model.PhoneNumber = customer.PhoneNumber;
            model.Credit = customer.Credit;

            return CommandResult<Models.Customer>.Success(model);
        }
    }
}
