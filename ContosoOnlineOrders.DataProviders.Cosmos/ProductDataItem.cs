using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContosoOnlineOrders.DataProviders.Cosmos
{
    public class ProductItem : Item
    {
        public string Name { get; set; }
        public int InventoryCount { get; set; }
    }
}
