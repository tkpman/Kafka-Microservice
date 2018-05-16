using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.Application.Entities
{
    public class OrderTransaction
    {
        public int Id { get; set; }

        public string OrderId { get; set; }

        public double Amount { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
