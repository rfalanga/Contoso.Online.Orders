using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoOnlineOrders.Abstractions.Models;

namespace ContosoOnlineOrders.Abstractions
{
    public interface IStoreDataService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int id);
        Task UpdateProductInventoryAsync(int id, int inventory);
        Task<Product> CreateProductAsync(Product product);
        Task<bool> CheckProductInventoryAsync(int id, int forAmount);
        Task<Order> CreateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<bool> CheckOrderInventoryAsync(Guid id);
        Task<Order> GetOrderAsync(Guid id);
        Task<bool> ShipOrderAsync(Guid id);
    }
}