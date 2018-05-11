using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Order.Api.Application.Models
{
    public class NewOrder
    {
        /// <summary>
        /// Id of the customer which order the product.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// List of products the user wants to order.
        /// </summary>
        public List<Product> Products { get; set; }
    }

    public class NewOrderValidator
        : AbstractValidator<NewOrder>
    {
        public NewOrderValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty();

            RuleFor(x => x.Products)
                .Must(x => x != null && x.Any())
                .WithMessage("Order has to contain at least one product.");
        }
    }
}
