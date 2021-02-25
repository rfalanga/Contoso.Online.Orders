using ContosoOnlineOrders.Abstractions;
using ContosoOnlineOrders.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ContosoOnlineOrders.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiVersion("1.2")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class ShopController : ControllerBase
    {
        private readonly IStoreDataService _storeServices;

        public ShopController(IStoreDataService storeServices) => _storeServices = storeServices;

        [HttpPost("/orders", Name = nameof(CreateOrder))]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            ActionResult<Order> result;

            try
            {
                _ = await _storeServices.CreateOrderAsync(order);
                result = Created($"/orders/{order.Id}", order);
            }
            catch
            {
                result = Conflict();
            }

            return result;
        }

        [HttpGet("/products", Name = nameof(GetProducts))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() =>
            Ok(await _storeServices.GetProductsAsync());

        [HttpGet("/products/page/{page}", Name = nameof(GetProductsPage))]
        [MapToApiVersion("1.1")]
        [MapToApiVersion("1.2")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsPage([FromRoute] int page = 0)
        {
            var pageSize = 5;
            var products = await _storeServices.GetProductsAsync();
            return Ok(products.Skip(page * pageSize).Take(pageSize));
        }

        [HttpGet("/products/{id}", Name = nameof(GetProduct))]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            ActionResult<Product> result = NotFound();

            var product = await _storeServices.GetProductAsync(id);
            if (product != null)
            {
                result = Ok(product);
            }

            return result;
        }
    }
}