using ContosoOnlineOrders.Abstractions.Models;
using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContosoOnlineOrders.DataProviders.Cosmos
{
    public class OrderItem : Item
    {
        public bool IsShipped { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
