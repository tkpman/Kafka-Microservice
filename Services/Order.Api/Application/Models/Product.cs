using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Application.Models
{
    public class Product
    {
        /// <summary>
        /// Id of the proudct.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Quantity of the product.
        /// </summary>
        public int Quantity { get; set; }
    }
}
