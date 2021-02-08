using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ContosoOnlineOrders.Abstractions.Models;
using ContosoOnlineOrders.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoOnlineOrders.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
#if ProducesConsumes
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
#endif
    public class ShopController : ControllerBase
    {
        public IStoreServices StoreServices { get; }

        public ShopController(IStoreServices storeServices)
        {
            StoreServices = storeServices;
        }

#if OperationId
        [HttpPost("/orders", Name = nameof(CreateOrder))]
#else
        [HttpPost("/orders")]
#endif
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            ActionResult<Order> result = Conflict();

            try
            {
                StoreServices.CreateOrder(order);
                result = Created($"/orders/{order.Id}", order);
            }
            catch
            {
                result = Conflict();
            }

            return await Task.FromResult(result);
        }

#if OperationId
        [HttpGet("/products", Name = nameof(GetProducts))]
#else
        [HttpGet("/products")]
#endif
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await Task.FromResult(Ok(StoreServices.GetProducts()));
        }

#if OperationId
        [HttpGet("/products/{id}", Name = nameof(GetProduct))]
#else
        [HttpGet("/products/{id}")]
#endif
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = StoreServices.GetProduct(id);
            ActionResult<Product> result = NotFound();

            if(product != null)
            {
                result = Ok(product);
            }

            return await Task.FromResult(result);
        }
    }
}