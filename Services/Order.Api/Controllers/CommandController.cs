using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Application.Commands;
using Order.Api.Application.Models;

namespace Order.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
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
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] NewOrder newOrder)
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
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update([FromBody] UpdatedOrder updatedOrder)
        {
            var command = new OrderUpdateCommand(updatedOrder);

            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkResult();

            return new BadRequestResult();
        }
    }
}