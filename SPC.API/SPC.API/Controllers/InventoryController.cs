using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SPC.API.Models;
using SPC.API.Services;
using SPC.Web.Models;

namespace SPC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // POST: api/Inventory/add
        [HttpPost("add")]
        public async Task<ActionResult<Drug>> AddDrug([FromBody] Drug drug)
        {
            if (drug == null)
            {
                return BadRequest(new { message = "Received drug object is null" });
            }

            try
            {
                var addedDrug = await _inventoryService.AddDrug(drug);
                return CreatedAtAction(nameof(AddDrug), new { id = addedDrug.Id }, addedDrug);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }




        // PUT: api/Inventory/stock/{drugId}
        [HttpPut("stock/{drugId}")]
        public async Task<IActionResult> UpdateStock(int drugId, [FromBody] int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return BadRequest(new { message = "Quantity must be greater than zero" });
                }

                await _inventoryService.UpdateStock(drugId, quantity);

                return Ok(new { message = "Stock updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }






        // GET: api/Inventory/drugs?searchTerm={searchTerm}
        [HttpGet("drugs")]
        public async Task<ActionResult<IEnumerable<Drug>>> SearchDrugs([FromQuery] string searchTerm)
        {
            var drugs = await _inventoryService.SearchDrugs(searchTerm);
            return Ok(drugs);
        }

        // GET: api/Inventory/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Drug>>> GetAllDrugs()
        {
            var drugs = await _inventoryService.GetAllDrugs();
            return Ok(drugs);
        }

        // DELETE: api/Inventory/delete/{drugId}
        [HttpDelete("delete/{drugId}")]
        public async Task<IActionResult> DeleteDrug(int drugId)
        {
            try
            {
                await _inventoryService.DeleteDrug(drugId);
                return Ok(new { message = "Drug deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("plant/{manufacturingPlantId}")]
        public async Task<ActionResult<IEnumerable<Drug>>> GetDrugsByManufacturingPlant(string manufacturingPlantId)
        {
            try
            {
                var drugs = await _inventoryService.GetDrugsByManufacturingPlant(manufacturingPlantId);

                if (!drugs.Any())
                {
                    return NotFound(new { message = $"No drugs found for manufacturing plant ID: {manufacturingPlantId}" });
                }

                return Ok(drugs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the drugs" });
            }
        }

    }
}