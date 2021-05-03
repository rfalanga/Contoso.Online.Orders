using ContosoOnlineOrders.Abstractions;
using ContosoOnlineOrders.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;
using System;
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
    public class AdminController : ControllerBase
    {
        private readonly IStoreDataService _storeServices;

        public AdminController(IStoreDataService storeServices) => _storeServices = storeServices;

        [HttpGet("/orders", Name = nameof(GetOrders))]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders() =>
            Ok(await _storeServices.GetOrdersAsync());

        [HttpGet("/orders/{id}", Name = nameof(GetOrder))]
        public async Task<ActionResult<Order>> GetOrder([FromRoute] Guid id)
        {
            ActionResult<Order> result = NotFound();

            var order = await _storeServices.GetOrderAsync(id);
            if (order != null)
            {
                result = Ok(order);
            }

            return result;
        }

        [HttpGet("/orders/{id}/checkInventory", Name = nameof(CheckInventory))]
        public async Task<ActionResult> CheckInventory([FromRoute] Guid id)
        {
            ActionResult result = NotFound();

            try
            {
                var inventory = await _storeServices.CheckOrderInventoryAsync(id);
                if (inventory)
                {
                    result = Ok();
                }
            }
            catch
            {
                result = Conflict();
            }

            return result;
        }

        [HttpGet("/orders/{id}/ship", Name = nameof(ShipOrder))]
        public async Task<ActionResult> ShipOrder([FromRoute] Guid id)
        {
            ActionResult result = NotFound();

            var shipResult = await _storeServices.ShipOrderAsync(id);
            if (shipResult)
            {
                result = Ok();
            }

            return result;
        }

        [HttpPut("/products/{id}/checkInventory", Name = nameof(UpdateProductInventory))]
        public async Task<ActionResult> UpdateProductInventory(
            [FromRoute] int id,
            [FromBody] InventoryUpdateRequest request)
        {
            ActionResult result;

            try
            {
                await _storeServices.UpdateProductInventoryAsync(id, request.countToAdd);
                result = Ok();
            }
            catch
            {
                result = NotFound();
            }

            return result;
        }

        [HttpPost("/products", Name = nameof(CreateProduct))]
        public async Task<ActionResult<Product>> CreateProduct(
            [FromBody] CreateProductRequest request)
        {
            ActionResult<Product> result;

            try
            {
                Product newProduct = new(request.Id, request.Name, request.InventoryCount);
                await _storeServices.CreateProductAsync(newProduct);
                result = Created($"/products/{request.Id}", newProduct);
            }
            catch
            {
                result = Conflict();
            }

            return result;
        }

        [HttpGet("/low-inventory", Name = nameof(GetLowInventoryProducts))]
        [MapToApiVersion("1.2")]
        public async Task<ActionResult<Product>> GetLowInventoryProducts()
        {
            var products = await _storeServices.GetProductsAsync();
            return Ok(products?.Where(_ => _.InventoryCount <= 5) ?? Array.Empty<Product>());
        }
    }
}