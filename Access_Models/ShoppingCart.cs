﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Access_Models
{
    public class ShoppingCart
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public ShoppingCart()
        {
            Quantity = 1;
        }
    }
}
