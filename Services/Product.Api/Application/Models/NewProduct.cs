using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Application.Models
{
    public class NewProduct
    {
        public NewProduct() { }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Amount { get; set; }
    }
}
