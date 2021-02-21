using Azure.Data.Tables;
using ContosoOnlineOrders.Abstractions;
using ContosoOnlineOrders.Abstractions.Models;
using System;
using System.Collections.Generic;

namespace ContosoOnlineOrders.DataServices.AzureTables
{
    public class AzureTablesDataService : IStoreDataService
    {
        private const string PRODUCTS_TABLE_NAME = "product";
        private const string ORDERS_TABLE_NAME = "order";
        private const string DEFAULT_PARTITION_KEY = "default";

        private TableServiceClient TableServiceClient { get; set; }
        private TableClient TableClient { get; set; }

        public AzureTablesDataService()
        {
        }

        public static AzureTablesDataService Create(string connectionString)
        {
            var result = new AzureTablesDataService();
            result.TableServiceClient = new TableServiceClient(connectionString);
            result.Setup();

            return result;
        }

        internal void Setup()
        {
            var orderCreateResult = TableServiceClient.CreateTableIfNotExists(ORDERS_TABLE_NAME);
            var productCreateResult = TableServiceClient.CreateTableIfNotExists(PRODUCTS_TABLE_NAME);

            // if the product table was just created, seed it with data
            if(productCreateResult != null)
            {
                var shells = new TableEntity(DEFAULT_PARTITION_KEY, "1000") { {"Name", "Taco Shells" }, {"InventoryCount", 10 } };
                var peppers = new TableEntity(DEFAULT_PARTITION_KEY, "1001") { { "Name", "Jalapeno Peppers" }, { "InventoryCount", 10 } };
                var cheese = new TableEntity(DEFAULT_PARTITION_KEY, "1002") { { "Name", "Cheddar Cheese" }, { "InventoryCount", 10 } };
               
                var productTableClient = TableServiceClient.GetTableClient(PRODUCTS_TABLE_NAME);
                
                productTableClient.AddEntity(shells);
                productTableClient.AddEntity(peppers);
                productTableClient.AddEntity(cheese);
            }
        }

        public bool CheckOrderInventory(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool CheckProductInventory(int id, int forAmount)
        {
            throw new NotImplementedException();
        }

        public void CreateOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public void CreateProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Order GetOrder(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetOrders()
        {
            throw new NotImplementedException();
        }

        public Product GetProduct(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetProducts()
        {
            throw new NotImplementedException();
        }

        public bool ShipOrder(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UpdateProductInventory(int id, int inventory)
        {
            throw new NotImplementedException();
        }
    }
}
