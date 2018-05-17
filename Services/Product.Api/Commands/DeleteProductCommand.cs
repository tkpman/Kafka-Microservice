using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Order.Api.Application.Commands;
using UnitOfWorks.Abstractions;

namespace Product.Api.Commands
{
    public class DeleteProductCommand : IRequest<ICommandResult<bool>>
    {
        public DeleteProductCommand(int id)
        {
            if(id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            this.Id = id;
        }

        public int Id { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ICommandResult<bool>>
    {
        private readonly IUnitOfWork _UnitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this._UnitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var repo = this._UnitOfWork.GetRepository<Application.Entities.Product>();

            var product = await repo.FirstOrDefault(x => x.Id == request.Id);

            if (product == null)
                return CommandResult<bool>.Failure("Something went wrong delete handler");


            repo.Remove(product);

            var result = await this._UnitOfWork.SaveChanges(cancellationToken);

            if (!result.IsSuccessfull())
                return CommandResult<bool>.Failure("coult not delete");

            return CommandResult<bool>.Success(true);
        }
    }
}
