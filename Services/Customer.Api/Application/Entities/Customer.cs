using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.Api.Application.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string CustomerId { get; set; }

        public string Name { get; set; }

        public double Credit { get; set; }

        public List<OrderTransaction> OrderTransactions { get; set; }
    }
}
