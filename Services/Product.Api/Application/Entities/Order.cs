﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Application.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderId { get; set; }

        public List<OrderProduct> Products { get; set; }
    }

    public class OrderProduct
    {
        public int Id { get; set; }

        public string ProductId { get; set; }

        public int Quantity { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }
    }
}
