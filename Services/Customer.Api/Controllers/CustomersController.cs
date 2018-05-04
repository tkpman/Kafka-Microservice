using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customer.Api.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers
{
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
        /// Gets a specific Customer by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetCustomer(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var command = new CustomerGetCommand(id);
            var result = this._mediator.Send(command);

            if (result != null)
                return new OkObjectResult(result);

            return new BadRequestResult();
        }

        // Get Request for List of all Customers.
        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var command = new CustomerAllCommand();
            var result = this._mediator.Send(command);

            if (result != null)
                return new OkObjectResult(result);

            return new BadRequestResult();
        }

        // Creates a new Customer.
        [HttpPost]
        public IActionResult CreateCustomer([FromBody] Application.Models.Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var command = new CustomerAddCommand(customer);
            var result = this._mediator.Send(command);

            if (result != null)
                return new OkObjectResult(result);

            return new BadRequestResult();
        }

        // Updates a Customer.
        [HttpPut]
        public IActionResult UpdateCustomer([FromBody] Application.Models.Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var command = new CustomerUpdateCommand(customer);
            var result = this._mediator.Send(command);

            if (result != null)
                return new OkObjectResult(result);

            return new BadRequestResult();
        }

        // Deletes a Customer by specified id.
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var command = new CustomerRemoveCommand(id);
            var result = this._mediator.Send(command);

            if (result != null)
                return new OkObjectResult(result);

            return new BadRequestResult();
        }
    }
}