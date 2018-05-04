using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Order.Api.Application.Models;

namespace Order.Api.Application.Commands
{
    public class OrderAllCommand
        : IRequest<ICommandResult<List<Models.Order>>>
    {
        public OrderAllCommand()
        { }
    }

    public class OrderAllCommandHandler
        : IRequestHandler<OrderAllCommand, ICommandResult<List<Models.Order>>>
    {
        public async Task<ICommandResult<List<Models.Order>>> Handle(
            OrderAllCommand request, 
            CancellationToken cancellationToken)
        {
            return CommandResult<List<Models.Order>>.Success(new List<Models.Order>());
        }
    }
}
