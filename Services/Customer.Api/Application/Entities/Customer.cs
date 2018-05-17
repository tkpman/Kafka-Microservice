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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public double Credit { get; set; }

        public List<OrderTransaction> OrderTransactions { get; set; }
    }
}
