using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SPC.API.Models;
using SPC.API.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SPC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierDrugController : ControllerBase
    {
        private readonly ISupplierDrugService _supplierDrugService;

        public SupplierDrugController(ISupplierDrugService supplierDrugService)
        {
            _supplierDrugService = supplierDrugService;
        }

        // POST: api/SupplierDrug/add
        [HttpPost("add")]
        public async Task<ActionResult<SupplierDrug>> AddSupplierDrug([FromBody] SupplierDrug supplierDrug)
        {
            if (supplierDrug == null)
            {
                return BadRequest(new { message = "Received drug object is null" });
            }

            try
            {
                var addedDrug = await _supplierDrugService.AddSupplierDrug(supplierDrug);
                return CreatedAtAction(nameof(GetDrugById), new { id = addedDrug.Id }, addedDrug);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message }); // Handles supplier not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the drug.", error = ex.Message });
            }
        }

        // GET: api/SupplierDrug/supplier/{supplierId}
        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<SupplierDrug>>> GetDrugsBySupplier(int supplierId)
        {
            var drugs = await _supplierDrugService.GetDrugsBySupplier(supplierId);

            if (drugs == null || !drugs.Any())
            {
                return NotFound(new { message = $"No drugs found for supplier ID: {supplierId}" });
            }

            return Ok(drugs);
        }


        // GET: api/SupplierDrug/drugs?searchTerm={searchTerm}
        [HttpGet("drugSearch")]
        public async Task<ActionResult<IEnumerable<SupplierDrug>>> SearchDrugs([FromQuery] string searchTerm)
        {
            var drugs = await _supplierDrugService.SearchDrugs(searchTerm);

            if (drugs == null || !drugs.Any())
            {
                return NotFound(new { message = "No matching drugs found" });
            }

            return Ok(drugs);
        }

        // GET: api/SupplierDrug/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDrug>> GetDrugById(int id)
        {
            var drug = await _supplierDrugService.GetDrugById(id);

            if (drug == null)
            {
                return NotFound(new { message = "Drug not found" });
            }

            return Ok(drug);
        }

        
        // GET: api/Inventory/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Drug>>> GetAllDrugs()
        {
            var drugs = await _supplierDrugService.GetAllDrugs();
            return Ok(drugs);
        }



        // PUT: api/Inventory/stock/{drugId}
        [HttpPut("stock/{id}")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return BadRequest(new { message = "Quantity must be greater than zero" });
                }

                await _supplierDrugService.UpdateStock(id, quantity);

                return Ok(new { message = "Stock updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/SupplierDrug/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSupplierDrug(int id)
        {
            try
            {
                bool deleted = await _supplierDrugService.DeleteSupplierDrug(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Drug not found" });
                }

                return Ok(new { message = "Drug deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the drug.", error = ex.Message });
            }
        }

    }
}
