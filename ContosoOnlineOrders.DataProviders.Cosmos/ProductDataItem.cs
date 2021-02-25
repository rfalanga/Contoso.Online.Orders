using ContosoOnlineOrders.Abstractions.Models;
using Microsoft.Azure.CosmosRepository;

namespace ContosoOnlineOrders.DataProviders.Cosmos
{
    public class ProductItem : Item
    {
        public string Name { get; set; }
        public int InventoryCount { get; set; }

        public static implicit operator Product(ProductItem productItem) =>
            new(int.Parse(productItem.Id), productItem.Name, productItem.InventoryCount);

        public static implicit operator ProductItem(Product product) =>
            new()
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                InventoryCount = product.InventoryCount
            };
    }
}
