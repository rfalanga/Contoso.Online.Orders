using System;
using System.Collections.Generic;

namespace ContosoOnlineOrders.Abstractions.Models
{
    public record Order(Guid Id, List<CartItem> Items)
    {
        public bool IsShipped { get; set; } = false;
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}