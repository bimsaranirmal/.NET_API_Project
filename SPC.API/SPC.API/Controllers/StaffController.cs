using Microsoft.AspNetCore.Mvc;
using SPC.API.Models;
using SPC.API.Services;
using System.Security.Cryptography;
using System.Text;

namespace SPC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterStaff([FromBody] Staff staff)
        {
            if (staff == null)
            {
                return BadRequest(new { message = "Staff data is null" });
            }

            try
            {
                var registeredStaff = await _staffService.RegisterStaffAsync(staff);
                return CreatedAtAction(nameof(RegisterStaff), new { id = registeredStaff.Id }, registeredStaff);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = "Email already exists." }); // 409 Conflict
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPost("login")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login([FromBody] Dictionary<string, string> credentials)
        {
            if (!credentials.TryGetValue("email", out string email) || !credentials.TryGetValue("password", out string password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            try
            {
                var staff = await _staffService.LoginAsync(email, password);
                if (staff == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                return Ok(new { message = "Login successful", staff });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllSuppliers()
        {
            var suppliers = await _staffService.GetAllStaffAsync();
            return Ok(suppliers);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            try
            {
                var result = await _staffService.DeleteStaffAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Staff not found." });
                }
                return Ok(new { message = "Staff deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
