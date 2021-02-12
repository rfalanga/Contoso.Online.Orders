using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoOnlineOrders.Abstractions.Models;
using Refit;

namespace ConsotoOnlineOrders.RefitClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Calling API using Refit");
            var client = RestService.For<IContosoOnlineOrdersApiClient>("http://localhost:5000");
            var products = await client.GetProducts();
            Console.WriteLine($"Currently {products.Count()} products in the store");
            Console.WriteLine("Creating new product");
            await client.CreateProduct(new CreateProductRequest
            {
              Id = 1000,
              InventoryCount = 10,
              Name = "Compact Disc"
            });
            products = await client.GetProducts();
            Console.WriteLine($"Currently {products.Count()} products in the store");
        }
    }
}
