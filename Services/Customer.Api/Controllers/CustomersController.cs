using System;
using System.Threading.Tasks;
using Customer.Api.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/Customers")]
    public class CustomersController : Controller
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            this._mediator = mediator;
        }

        /// <summary>
        /// Creates a Customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCustomer([FromBody] Application.Models.NewCustomer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var command = new CustomerAddCommand(customer);
            var result = await this._mediator.Send(command);

            if (result != null)
                return new OkObjectResult(result.Result);

            return new BadRequestResult();
        }

        /// <summary>
        /// Updates a Customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCustomer([FromBody] Application.Models.Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var command = new CustomerUpdateCommand(customer);
            var result = await this._mediator.Send(command);

            if (result != null)
                return new OkObjectResult(result.Result);

            return new BadRequestResult();
        }

        /// <summary>
        /// Deletes a Customer by specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var command = new CustomerRemoveCommand(id);
            var result = await this._mediator.Send(command);

            if (result != null)
                return new OkObjectResult(result.Result);

            return new BadRequestResult();
        }
    }
}