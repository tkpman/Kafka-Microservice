using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UnitOfWorks.EntityFrameworkCore.Tests.Seeds
{
    public class Customer
    {
        public int Age { get; set; }

        public Address Address { get; set; }

        public string FirstName { get; set; }

        public int Id { get; set; }

        public string LastName { get; set; }

        public static void Seed(DbContext dbContext)
        {
            // Add customers to the db context.
            dbContext.Set<Customer>().AddRange(GetCustomers());
            dbContext.SaveChanges();
        }

        private static List<Customer> GetCustomers()
        {
            var customers = new List<Customer>();

            customers.Add(new Customer
            {
                FirstName = "Frank",
                LastName = "Wong",
                Age = 22,
                Address = new Address
                {
                    PostalCode = 6000,
                    Street = "Street One",
                    Townn = "Town One"
                }
            });

            customers.Add(new Customer
            {
                FirstName = "Kim",
                LastName = "Jensen",
                Age = 22,
                Address = new Address
                {
                    PostalCode = 6100,
                    Street = "Street Two",
                    Townn = "Town Two"
                }
            });

            customers.Add(new Customer
            {
                FirstName = "Mads",
                LastName = "Densen",
                Age = 25,
                Address = new Address
                {
                    PostalCode = 6200,
                    Street = "Street Three",
                    Townn = "Town Three"
                }
            });

            return customers;
        }
    }
}
