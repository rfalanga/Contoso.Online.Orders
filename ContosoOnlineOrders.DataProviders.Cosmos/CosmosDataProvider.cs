using ContosoOnlineOrders.Abstractions;
using ContosoOnlineOrders.Abstractions.Models;
using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoOnlineOrders.DataProviders.Cosmos
{
    public class CosmosDataProvider : IStoreDataService
    {
        private readonly IRepository<ProductItem> _productRepository;
        private readonly IRepository<OrderItem> _orderRepository;

        public CosmosDataProvider(IRepositoryFactory factory)
        {
            _productRepository = factory.RepositoryOf<ProductItem>();
            _orderRepository = factory.RepositoryOf<OrderItem>();
        }

        public async Task<bool> CheckOrderInventoryAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id.ToString());

            foreach (var item in order.Items)
            {
                if(!await CheckProductInventoryAsync(item.ProductId, item.Quantity))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> CheckProductInventoryAsync(int id, int forAmount)
        {
            var product = await GetProductAsync(id);
            return product.InventoryCount >= forAmount;
        }

        public async Task<Order> CreateOrderAsync(Order order) =>
            await _orderRepository.CreateAsync(order);

        public async Task<Product> CreateProductAsync(Product product) =>
            await _productRepository.CreateAsync(product);

        public async Task<Order> GetOrderAsync(Guid id) =>
            await _orderRepository.GetAsync(id.ToString());

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var orders = await _orderRepository.GetAsync(_ => true);
            return orders.Select(orderItem => (Order)orderItem);
        }

        public async Task<Product> GetProductAsync(int id) =>
            await _productRepository.GetAsync(id.ToString());

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = await _productRepository.GetAsync(_ => true);
            return products.Select(productItem => (Product)productItem);
        }

        public async Task<bool> ShipOrderAsync(Guid id)
        {
            var orderItem = await _orderRepository.GetAsync(id.ToString());
            var products = await GetProductsAsync();

            // update all the inventories
            var updateTasks =
                orderItem.Items.Select(
                    item =>
                    {
                        var product = products.First(_ => _.Id == item.ProductId);
                        var productItem = product with
                        {
                            InventoryCount = product.InventoryCount - item.Quantity
                        };

                        return _productRepository.UpdateAsync(productItem).AsTask();
                    });

            await Task.WhenAll(updateTasks);

            orderItem.IsShipped = true;

            _ = await _orderRepository.UpdateAsync(orderItem);

            return true;
        }

        public async Task UpdateProductInventoryAsync(int id, int inventory)
        {
            var productItem = await _productRepository.GetAsync(id.ToString());
            productItem.InventoryCount = inventory;
            await _productRepository.UpdateAsync(productItem);
        }
    }
}
