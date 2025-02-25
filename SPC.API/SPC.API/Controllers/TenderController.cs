using Microsoft.AspNetCore.Mvc;
using SPC.API.Models;
using SPC.API.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderController : ControllerBase
    {
        private readonly ITenderService _tenderService;

        public TenderController(ITenderService tenderService)
        {
            _tenderService = tenderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tender>>> GetAllTenders()
        {
            var tenders = await _tenderService.GetAllTenders();
            return Ok(tenders);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Tender>>> SearchTenders(string searchTerm)
        {
            var tenders = await _tenderService.SearchTenders(searchTerm);
            return Ok(tenders);
        }

        [HttpGet("supplier/{supplierId:int}")]
        public async Task<ActionResult<IEnumerable<Tender>>> GetTendersBySupplierId(int supplierId)

        {
            var tenders = await _tenderService.GetTendersBySupplierId(supplierId);
            if (tenders == null || !tenders.Any())
            {
                return NotFound($"No tenders found for Supplier ID {supplierId}.");
            }
            return Ok(tenders);
        }

        [HttpPost("submit")]
        public async Task<ActionResult> SubmitTender([FromBody] Tender tender)
        {
            if (tender == null)
                return BadRequest("Tender data is required.");

            var success = await _tenderService.SubmitTender(tender);
            if (success)
                return Ok(new { message = "Tender submitted successfully." });

            return StatusCode(500, "Error submitting tender.");
        }


        [HttpPut("approve/{tenderId}")]
        public async Task<ActionResult> ApproveTender(int tenderId)
        {
            var success = await _tenderService.ApproveTender(tenderId);
            if (success)
                return Ok(new { message = "Tender approved successfully." });

            return NotFound("Tender not found.");
        }

        [HttpPut("reject/{tenderId}")]
        public async Task<ActionResult> RejectTender(int tenderId)
        {
            var success = await _tenderService.RejectTender(tenderId);
            if (success)
                return Ok(new { message = "Tender rejected successfully." });

            return NotFound("Tender not found.");
        }

        [HttpDelete("delete/{tenderId}")]
        public async Task<ActionResult> DeleteTender(int tenderId)
        {
            var success = await _tenderService.DeleteTender(tenderId);
            if (success)
                return Ok(new { message = "Tender deleted successfully." });
            return NotFound("Tender not found.");
        }

        [HttpPut("done/{tenderId}")]
        public async Task<ActionResult> MarkTenderAsDone(int tenderId)
        {
            var success = await _tenderService.MarkTenderAsDone(tenderId);
            if (success)
                return Ok(new { message = "Tender marked as done successfully." });
            return NotFound($"Tender with ID {tenderId} not found.");
        }
    }
}
