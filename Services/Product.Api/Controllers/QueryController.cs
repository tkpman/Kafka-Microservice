using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Application.Commands;
using Product.Api.Commands;

namespace Product.Api.Controllers
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
        /// Gets all products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Application.Models.Product>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProducts()
        {
            var command = new GetProductsCommand();

            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);
            else
                return new BadRequestObjectResult("Something went wrong");
        }

        /// <summary>
        /// Gets a product from id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Application.Models.Product), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetProduct(int id)
        {
            if (id < 0)
                return new BadRequestObjectResult("Invalid variable");

            var command = new GetProductCommand(id);

            var result = await this._mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);

            else
                return new BadRequestObjectResult("Something went wrong");
        }
        
    }
}