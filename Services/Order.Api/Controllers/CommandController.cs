using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Application.Models;

namespace Order.Api.Controllers
{
    [Route("api/{controller}")]
    public class CommandController : Controller
    {
        private readonly IMediator _mediator;

        public CommandController(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            this._mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewOrder newOrder)
        {
            return new CreatedResult("", "");
        }
    }
}