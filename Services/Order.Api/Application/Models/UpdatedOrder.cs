using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Order.Api.Application.Models
{
    public class UpdatedOrder
    {
        /// <summary>
        /// List of products the user wants to order.
        /// </summary>
        public List<Product> Products { get; set; }
    }

    public class UpdatedOrderValidator
        : AbstractValidator<NewOrder>
    {
        public UpdatedOrderValidator()
        {
            RuleFor(x => x.Products)
                .NotNull()
                .WithMessage("Order has to contain at least one product.");

            RuleFor(x => x.Products)
                .Must(x => x != null && x.Any())
                .WithMessage("Order has to contain at least one product.");
        }
    }
}
