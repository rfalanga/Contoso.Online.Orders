using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoOnlineOrders.Abstractions.Models;
using Refit;

namespace ConsotoOnlineOrders.RefitClient
{
    public interface IContosoOnlineOrdersApiClient
    {
        [Get("/products")]
        Task<IEnumerable<Product>> GetProducts();

        [Post("/products")]
        Task CreateProduct([Body] CreateProductRequest request);
    }
}
