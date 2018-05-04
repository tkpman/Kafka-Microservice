using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Application.Commands;

namespace Order.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class QueryController : Controller
    {
        private readonly IMediator _mediator;

        public QueryController(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            this._mediator = mediator;
        }

        /// <summary>
        /// Get's the order with the given id.
        /// </summary>
        /// <param name="orderId">Id of the order to get.</param>
        /// <returns></returns>
        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(Application.Models.Order), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get(int orderId)
        {
            var command = new OrderGetCommand(orderId);

            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);

            return new BadRequestResult();
        }

        /// <summary>
        /// Get's all the orders.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Application.Models.Order>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> All()
        {
            var command = new OrderAllCommand();

            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);

            return new BadRequestResult();
        }
    }
}