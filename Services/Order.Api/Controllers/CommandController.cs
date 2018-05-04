using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Application.Commands;
using Order.Api.Application.Models;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    public class CommandController : Controller
    {
        private readonly IMediator _mediator;

        public CommandController(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            this._mediator = mediator;
        }

        /// <summary>
        /// Create's the given order.
        /// </summary>
        /// <param name="newOrder">Order to create / place.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(NewOrder newOrder)
        {
            var command = new OrderCreateCommand(newOrder);

            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new CreatedResult("", "");

            return new BadRequestResult();
        }

        /// <summary>
        /// Removes the given order.
        /// </summary>
        /// <param name="orderId">Id of the order to remove.</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Remove(int orderId)
        {
            var command = new OrderRemoveCommand(orderId);

            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkResult();

            return new BadRequestResult();
        }

        /// <summary>
        /// Update's the given order.
        /// </summary>
        /// <param name="updatedOrder">Order to update.</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update(UpdatedOrder updatedOrder)
        {
            var command = new OrderUpdateCommand(updatedOrder);

            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkResult();

            return new BadRequestResult();
        }
    }
}