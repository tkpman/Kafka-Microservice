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
    public class GetProductsCommand : 
        IRequest<ICommandResult<List<Application.Models.Product>>>
    {
    }

    public class GetProductsCommandHandler : 
        IRequestHandler<GetProductsCommand, 
        ICommandResult<List<Application.Models.Product>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductsCommandHandler(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            this._unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult<List<Application.Models.Product>>> Handle(
            GetProductsCommand request, 
            CancellationToken cancellationToken)
        {
            var repo = this._unitOfWork.GetRepository<Application.Entities.Product>();

            var result = await repo.All();


            List<Application.Models.Product> models = new List<Application.Models.Product>();
            foreach (var product in result)
            {
                var model = new Application.Models.Product();

                model.Name = product.Name;
                model.Amount = product.Quantity;
                model.Id = product.Id;
                model.Price = product.Price;
                model.ProductId = product.ProductId;

                models.Add(model);
            }

            return CommandResult<List<Application.Models.Product>>.Success(models);
        }
    }
}
