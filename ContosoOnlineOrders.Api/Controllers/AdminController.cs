using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ContosoOnlineOrders.Abstractions;
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
    public class AdminController : ControllerBase
    {
        public IStoreDataService StoreServices { get; }

        public AdminController(IStoreDataService storeServices)
        {
            StoreServices = storeServices;
        }

#if OperationId
        [HttpGet("/orders", Name = nameof(GetOrders))]
#else
        [HttpGet("/orders")]
#endif
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await Task.FromResult(Ok(StoreServices.GetOrders()));
        }

#if OperationId
        [HttpGet("/orders/{id}", Name = nameof(GetOrder))]
#else
        [HttpGet("/orders/{id}")]
#endif
        public async Task <ActionResult<Order>> GetOrder([FromRoute] Guid id)
        {
            var order = StoreServices.GetOrder(id);
            ActionResult<Order> result = NotFound();

            if(order != null)
            {
                result = Ok(order);
            }

            return await Task.FromResult(result);
        }

#if OperationId
        [HttpGet("/orders/{id}/checkInventory", Name = nameof(CheckInventory))]
#else
        [HttpGet("/orders/{id}/checkInventory")]
#endif
        public async Task<ActionResult> CheckInventory([FromRoute] Guid id)
        {
            ActionResult result = NotFound();

            try
            {
                var inventory = StoreServices.CheckOrderInventory(id);
                if (inventory)
                {
                    result = Ok();
                }
            }
            catch
            {
                result = Conflict();
            }

            return await Task.FromResult(result);
        }

#if OperationId
        [HttpGet("/orders/{id}/ship", Name = nameof(ShipOrder))]
#else
        [HttpGet("/orders/{id}/ship")]
#endif
        public async Task<ActionResult> ShipOrder([FromRoute] Guid id)
        {
            var shipResult = StoreServices.ShipOrder(id);
            ActionResult result = NotFound();

            if(shipResult)
            {
                result = Ok();
            }

            return await Task.FromResult(result);
        }

#if OperationId
        [HttpPut("/products/{id}/checkInventory", Name = nameof(UpdateProductInventory))]
#else
        [HttpPut("/products/{id}/checkInventory")]
#endif
        public async Task<ActionResult> UpdateProductInventory([FromRoute] int id, 
            [FromBody] InventoryUpdateRequest request)
        {
            ActionResult result = NotFound();

            try
            {
                StoreServices.UpdateProductInventory(id, request.countToAdd);
                result = Ok();
            }
            catch
            {
                result = NotFound();
            }

            return await Task.FromResult(result);
        }

#if OperationId
        [HttpPost("/products", Name = nameof(CreateProduct))]
#else
        [HttpPost("/products")]
#endif
        public async Task<ActionResult<Product>> CreateProduct(
            [FromBody] CreateProductRequest request)
        {
            ActionResult<Product> result = NotFound();

            try
            {
                var newProduct = new Product(request.Id, request.Name, request.InventoryCount);
                StoreServices.CreateProduct(newProduct);
                result = Created($"/products/{request.Id}", newProduct);
            }
            catch
            {
                result = Conflict();
            }

            return await Task.FromResult(result);
        }
    }
}