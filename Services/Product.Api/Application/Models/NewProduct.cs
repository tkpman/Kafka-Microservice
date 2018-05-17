using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Product.Api.Application.Models
{
    public class NewProduct
    {
        public NewProduct() { }

        public string Name { get; set; }
        public string ProductId { get; set; }
        public int Price { get; set; }
        public int Amount { get; set; }
    }

    public class NewOrderValidator
        : AbstractValidator<NewProduct>
    {
        public NewOrderValidator()
        {
            RuleFor(x => x.Name).
                MinimumLength(1).
                WithMessage("Please enter a name");

            RuleFor(x => x.Amount).
                GreaterThan(1).
                WithMessage("Must be above 1");

            RuleFor(x => x.ProductId).
                MinimumLength(1).
                WithMessage("Please enter a ProductId");

            RuleFor(x => x.Price).
                GreaterThan(1).
                WithMessage("Must be above 1");

        }
    }
}
