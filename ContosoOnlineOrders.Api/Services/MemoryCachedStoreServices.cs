using ContosoOnlineOrders.Abstractions;
using ContosoOnlineOrders.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoOnlineOrders.Api.Services
{
    public class MemoryCachedStoreServices : IStoreDataService
    {
        const string MEMCACHE_KEY_ORDERS = "orders";
        const string MEMCACHE_KEY_PRODUCTS = "products";

        private readonly IMemoryCache _memoryCache;

        public MemoryCachedStoreServices(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public async Task<bool> CheckOrderInventoryAsync(Guid id)
        {
            var order = (await GetOrderAsync(id)) ?? throw new ArgumentException($"No order found for Order ID {id}.");

            foreach (var cartItem in order.Items)
            {
                var result = await CheckProductInventoryAsync(cartItem.ProductId, cartItem.Quantity);
                if (!result)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> CheckProductInventoryAsync(int id, int forAmount) =>
            (await GetProductAsync(id)).InventoryCount >= forAmount;

        public async Task<Order> CreateOrderAsync(Order order)
        {
            var orders = await GetOrdersAsync();
            if (orders.Any(x => x.Id == order.Id))
            {
                throw new ArgumentException($"An order already exists with the Order ID {order.Id}.");
            }
            else
            {
                _memoryCache.Set(MEMCACHE_KEY_ORDERS, orders.Concat(new[] { order }).ToList());
            }

            return order;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var products = await GetProductsAsync();
            if (products.Any(x => x.Id == product.Id))
            {
                throw new ArgumentException($"Product with matching product ID {product.Id} already exists.");
            }
            else
            {
                _memoryCache.Set(MEMCACHE_KEY_PRODUCTS, products.Concat(new[] { product }).ToList());
            }

            return product;
        }

        public async Task<Order> GetOrderAsync(Guid id)
        {
            var orders = await GetOrdersAsync();
            return orders.FirstOrDefault(order => order.Id == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync() =>
            await _memoryCache.GetOrCreateAsync(
                MEMCACHE_KEY_ORDERS, _ => Task.FromResult(new List<Order>()));

        public async Task<Product> GetProductAsync(int id)
        {
            var products = await GetProductsAsync();
            return products.FirstOrDefault(product => product.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync() =>
            await _memoryCache.GetOrCreateAsync(
                MEMCACHE_KEY_PRODUCTS, _ => Task.FromResult(new List<Product>()));

        public async Task<bool> ShipOrderAsync(Guid id)
        {
            try
            {
                var orders = await GetOrdersAsync();
                var order = orders.FirstOrDefault(_ => _.Id == id);
                if (order is not null)
                {
                    order.IsShipped = true;
                    _memoryCache.Set(MEMCACHE_KEY_ORDERS, orders.ToList());
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task UpdateProductInventoryAsync(int id, int inventory)
        {
            var products = await GetProductsAsync();
            var existingProduct = products.FirstOrDefault(x => x.Id == id);
            if (existingProduct != null)
            {
                var otherProducts = products.Where(_ => _.Id != id).ToList();
                otherProducts.Add(new Product(id, existingProduct.Name, inventory));
                _memoryCache.Set(MEMCACHE_KEY_PRODUCTS, otherProducts.ToList());
            }
        }
    }
}