using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Customer.Api.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Application.Commands;

namespace Customer.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/Query")]
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
        /// Gets a specific Customer by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Application.Models.Customer), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCustomer(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var command = new CustomerGetCommand(id);
            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);

            return new BadRequestObjectResult(result.Error);
        }

        /// <summary>
        /// Get Request for List of all Customers.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Application.Models.Customer>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCustomers()
        {
            var command = new CustomerAllCommand();
            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);

            return new BadRequestObjectResult(result.Error);
        }
    }
}