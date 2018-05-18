using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderQuery.Api.Application.Models
{
    public class Order
    {
        public string OrderId { get; set; }

        public string CustomerId { get; set; }

        public string Date { get; set; }

        public double Total { get; set; }

        public List<Product> Products { get; set; }
    }
}
