using ContosoOnlineOrders.Abstractions.Models;
using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;

namespace ContosoOnlineOrders.DataProviders.Cosmos
{
    public class OrderItem : Item
    {
        public bool IsShipped { get; set; }
        public DateTime OrderDate { get; set; }
        public List<CartItem> Items { get; set; } = new();

        public static implicit operator Order(OrderItem orderItem) =>
            new(Guid.Parse(orderItem.Id), orderItem.Items);

        public static implicit operator OrderItem(Order order) =>
            new()
            {
                Id = order.Id.ToString(),
                IsShipped = order.IsShipped,
                Items = order.Items,
                OrderDate = DateTime.Now
            };
    }
}
