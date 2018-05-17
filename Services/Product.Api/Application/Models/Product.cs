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
}
