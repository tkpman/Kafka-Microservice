using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Application.Models
{
    public class Product
    {
        public Product() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductId { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
    }

    public class ProductValidator
        : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Id).
                GreaterThan(0).
                WithMessage("Id must be greather than 0");

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
