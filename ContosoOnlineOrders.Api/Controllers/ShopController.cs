using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ContosoOnlineOrders.Abstractions.Models;
using ContosoOnlineOrders.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoOnlineOrders.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
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
        [HttpGet("/products/page/{page}", Name = nameof(GetProductsPage))]
#else
        [HttpGet("/products")]
#endif
        [MapToApiVersion("1.1")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsPage([FromRoute] int page = 0)
        {
            var pageSize = 5;
            var productsPage = StoreServices.GetProducts().Skip(page * pageSize).Take(pageSize);
            return await Task.FromResult(Ok(productsPage));
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