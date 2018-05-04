using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Product.Api.Application.Models;

namespace Product.Api.Controllers
{
    [Route("api/[Controller]")]
    public class CommandController : Controller
    {
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            return Ok();
        }

        
        [HttpPost]
        public IActionResult CreateProduct([FromBody] NewProduct product)
        {
            return Ok();
        }

        
        [HttpPut]
        public IActionResult UpdateProduct([FromBody] Application.Models.Product product)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            return Ok();
        }
    }
}