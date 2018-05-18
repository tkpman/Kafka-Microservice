using MediatR;
using Order.Api.Application.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWorks.Abstractions;

namespace Customer.Api.Application.Commands
{
    public class CustomerRemoveCommand : IRequest<ICommandResult<bool>>
    {
        public int CustomerId { get; set; }

        public CustomerRemoveCommand(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            this.CustomerId = id;
        }
    }

    public class CustomerRemoveCommandHandler : IRequestHandler<CustomerRemoveCommand, ICommandResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CustomerRemoveCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            this._unitOfWork = unitOfWork;
            this._mediator = mediator;
        }

        public async Task<ICommandResult<bool>> Handle(CustomerRemoveCommand request, CancellationToken cancellationToken)
        {
            var repo = this._unitOfWork.GetRepository<Application.Entities.Customer>();

            var customer = await repo.FirstOrDefault(x => x.Id == request.CustomerId);

            if (customer == null)
                return CommandResult<bool>.Failure("Something went wrong delete handler");


            repo.Remove(customer);

            var result = await this._unitOfWork.SaveChanges(cancellationToken);

            if (!result.IsSuccessfull())
                return CommandResult<bool>.Failure("coult not delete");

            return CommandResult<bool>.Success(true);
        }
    }
}
