using System;
using System.Net.Http;

namespace ContosoOnlineOrders.NSwagGeneratedClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var httpClient = new HttpClient())
            {
                var apiClient = new ContosoOnlineOrders_ApiClient(
                    "http://localhost:5000",
                    httpClient
                );

#if OperationId
                /*------------------------------------------------------------
                With explicit operationId: Note how the API advertises itself
                and is easy to use and discover.
                ------------------------------------------------------------*/

                // create a product
                apiClient.CreateProductAsync("1.1", new CreateProductRequest
                {
                    Id = 1000,
                    InventoryCount = 0,
                    Name = "Test Product"
                });

                // update a product's inventory
                apiClient.UpdateProductInventoryAsync(1000, "1.1",
                    new InventoryUpdateRequest
                    {
                        CountToAdd = 50,
                        ProductId = 1000
                    });

                // get all products
                apiClient.GetProductsAsync("1.1");

                // get one product
                apiClient.GetProductAsync(1000, "1.1");

                // create a new order
                Guid orderId = Guid.NewGuid();

                apiClient.CreateOrderAsync("1.1", new Order
                {
                    Id = orderId,
                    Items = new CartItem[]
                    {
                        new CartItem { ProductId = 1000, Quantity = 10 }
                    }
                });

                // get one order
                apiClient.GetOrderAsync(orderId, "1.1");

                // get all orders
                apiClient.GetOrdersAsync("1.1");

                // check an order's inventory
                apiClient.CheckInventoryAsync(orderId, "1.1");

                // ship an order
                apiClient.ShipOrderAsync(orderId, "1.1");
#else
                /*------------------------------------------------------------
                Without explicit operationId: Note how the generated SDK
                code for the API isn't discoverable.

                If you were discovering the API's shape using IntelliSense alone,
                you'd have to look at the parameters to know how to call the API.
                ------------------------------------------------------------*/

                // create a product
                apiClient.ProductsAllAsync(new CreateProductRequest
                {
                    Id = 1000,
                    InventoryCount = 0,
                    Name = "Test Product"
                });

                // update a product's inventory
                apiClient.CheckInventory2Async(1000,                           
                new InventoryUpdateRequest
                    {
                        CountToAdd = 50,
                        ProductId = 1000
                    });

                // get all products
                apiClient.ProductsAsync();

                // get one product
                apiClient.Products2Async(1000);

                // create a new order
                Guid orderId = Guid.NewGuid();

                apiClient.OrdersAsync(new Order
                {
                    Id = orderId,
                    Items = new CartItem[]
                    {
                        new CartItem { ProductId = 1000, Quantity = 10 }
                    }
                });

                // get one order
                apiClient.Orders2Async(orderId);

                // get all orders
                apiClient.OrdersAllAsync();

                // check an order's inventory
                apiClient.CheckInventoryAsync(orderId);

                // ship an order
                apiClient.ShipAsync(orderId);
#endif
            }

            Console.WriteLine("Hello World!");
        }
    }
}
