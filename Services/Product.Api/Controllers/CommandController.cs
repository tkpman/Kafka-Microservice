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

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var command = new GetProductsCommand();

            var result = await this._Mediator.Send(command);

            if(result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);
            else
                return new BadRequestObjectResult("Something went wrong");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            if (id < 0)
                return new BadRequestObjectResult("Invalid variable");

            var command = new GetProductCommand(id);

            var result = await this._Mediator.Send(command);

            if (result.Status == CommandResultStatus.Success)
                return new OkObjectResult(result.Result);

            else 
                return new BadRequestObjectResult("Something went wrong");           
        }

        
        [HttpPost]
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

        
        [HttpPut]
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

        [HttpDelete("{id}")]
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