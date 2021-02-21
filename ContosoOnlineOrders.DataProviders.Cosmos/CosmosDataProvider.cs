using ContosoOnlineOrders.Abstractions;
using ContosoOnlineOrders.Abstractions.Models;
using Microsoft.Azure.CosmosRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContosoOnlineOrders.DataProviders.Cosmos
{
    public class CosmosDataProvider : IStoreDataService
    {
        public CosmosDataProvider(IRepository<ProductItem> productRepository,
            IRepository<OrderItem> orderRepository)
        {
            ProductRepository = productRepository;
            OrderRepository = orderRepository;
        }

        public IRepository<ProductItem> ProductRepository { get; }
        public IRepository<OrderItem> OrderRepository { get; }

        public bool CheckOrderInventory(Guid id)
        {
            var order = GetOrder(id);

            foreach (var item in order.Items)
            {
                if(!CheckProductInventory(item.ProductId, item.Quantity))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckProductInventory(int id, int forAmount)
        {
            var product = GetProduct(id);
            return product.InventoryCount >= forAmount;
        }

        public void CreateOrder(Order order)
        {
            OrderRepository.CreateAsync(new OrderItem
            {
                OrderDate = DateTime.Now,
                Id = order.Id.ToString(),
                IsShipped = false,
                Items = order.Items
            });
        }

        public void CreateProduct(Product product)
        {
            ProductRepository.CreateAsync(new ProductItem
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                InventoryCount = product.InventoryCount
            });
        }

        public Order GetOrder(Guid id)
        {
            return GetOrders().First(_ => _.Id == id);
        }

        public IEnumerable<Order> GetOrders()
        {
            return OrderRepository.GetAsync(_ => true).Result
                .Select(_ => new Order(Guid.Parse(_.Id), _.Items));
        }

        public Product GetProduct(int id)
        {
            return GetProducts().First(_ => _.Id == id);
        }

        public IEnumerable<Product> GetProducts()
        {
            return ProductRepository.GetAsync(_ => true).Result
                .Select(_ => new Product(int.Parse(_.Id), _.Name, _.InventoryCount));
        }

        public bool ShipOrder(Guid id)
        {
            var orderItem = OrderRepository.GetAsync(_ => _.Id == id.ToString()).Result.First();
            var products = GetProducts();

            // update all the inventories
            foreach (var item in orderItem.Items)
            {
                UpdateProductInventory(item.ProductId, products.First(_ => _.Id == item.ProductId).InventoryCount - item.Quantity);
            }
            orderItem.IsShipped = true;
            OrderRepository.UpdateAsync(orderItem);
            return true;
        }

        public void UpdateProductInventory(int id, int inventory)
        {
            var productItem = ProductRepository.GetAsync(_ => _.Id == id.ToString()).Result.First();
            productItem.InventoryCount = inventory;
            ProductRepository.UpdateAsync(productItem);
        }
    }
}
