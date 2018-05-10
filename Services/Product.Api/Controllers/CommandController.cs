using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Order.Api.Application.Commands;
using Product.Api.Application.Models;
using Product.Api.Commands;

namespace Product.Api.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class CommandController : Controller
    {
        private readonly IMediator _Mediator;

        public CommandController(IMediator mediator)
        {
            if(mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            this._Mediator = mediator;
        }
        
        /// <summary>
        /// Creates a product based on input.
        /// </summary>
        /// <param name="product">The product to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProduct([FromBody] NewProduct product)
        {
            if(product == null)
                throw new ArgumentNullException(nameof(product));

            var command = new CreateProductCommand(product);

            var result = await this._Mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);
            else
                return new BadRequestObjectResult("Something went wrong");
        }

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="product">Product to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProduct([FromBody] Application.Models.Product product)
        {
            if(product == null)
                throw new ArgumentNullException(nameof(product));

            var command = new UpdateProductCommand(product);

            var result = await this._Mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);
            else
                return new BadRequestObjectResult("Something went wrong");
        }

        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <param name="id">id of product to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if(id < 0)
                return new BadRequestObjectResult("Invalid input");

            var command = new DeleteProductCommand(id);

            var result = await this._Mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);
            else
                return new BadRequestObjectResult("Something went wrong");
        }
    }
}