using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessing.Application.Configuration
{
    public class OrderRulesOptions
    {
        public int MaxItemQuantity { get; set; }
        public int MaxItemsPerOrder { get; set; }
    }
}
