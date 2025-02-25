using Microsoft.AspNetCore.Mvc;
using SPC.API.Models;
using SPC.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class SupplierDrugOrderController : ControllerBase
{
    private readonly ISupplierDrugOrderService _supplierDrugOrderService;

    public SupplierDrugOrderController(ISupplierDrugOrderService supplierDrugOrderService)
    {
        _supplierDrugOrderService = supplierDrugOrderService;
    }

    // GET: api/SupplierDrugOrder
    [HttpGet("AllOrders")]
    public async Task<ActionResult<IEnumerable<SupplierDrugOrder>>> GetAll()
    {
        var orders = await _supplierDrugOrderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    // GET: api/SupplierDrugOrder/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierDrugOrder>> GetById(int id)
    {
        var order = await _supplierDrugOrderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    // POST: api/SupplierDrugOrder
    [HttpPost]
    public async Task<ActionResult<SupplierDrugOrder>> Create([FromBody] SupplierDrugOrder order)
    {
        if (order == null)
        {
            return BadRequest("Invalid order data.");
        }

        var createdOrder = await _supplierDrugOrderService.CreateOrderAsync(order);
        return CreatedAtAction(nameof(GetById), new { id = createdOrder.OrderId }, createdOrder);
    }

    // PUT: api/SupplierDrugOrder/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SupplierDrugOrder order)
    {
        if (id != order.OrderId)
        {
            return BadRequest("Order ID mismatch.");
        }

        var updated = await _supplierDrugOrderService.UpdateOrderAsync(order);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/SupplierDrugOrder/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _supplierDrugOrderService.DeleteOrderAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // GET: api/SupplierDrugOrder/supplier/{supplierId}
    [HttpGet("supplier/{supplierId}")]
    public async Task<ActionResult<IEnumerable<SupplierDrugOrder>>> GetBySupplierId(int supplierId)
    {
        var orders = await _supplierDrugOrderService.GetOrdersBySupplierIdAsync(supplierId);

        if (orders == null || !orders.Any())
        {
            return NotFound("No orders found for the given supplier.");
        }

        return Ok(orders);
    }

}
