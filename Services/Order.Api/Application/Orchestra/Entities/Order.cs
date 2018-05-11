using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Application.Orchestra.Entities
{
    public class Order
    {
        public string OrderId { get; set; }
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime Date { get; set; }
        public List<OrderProduct> Products { get; set; }

    }

    public class OrderProduct
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }

    }
}
