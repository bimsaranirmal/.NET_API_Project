using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SPC.API.Models;
using SPC.API.Services;
using SPC.Web.Models;

namespace SPC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;

        public PharmacyController(IPharmacyService pharmacyService)
        {
            _pharmacyService = pharmacyService;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterPharmacy([FromBody] Pharmacy pharmacy)
        {
            if (pharmacy == null)
            {
                return BadRequest(new { message = "Pharmacy data is null" });
            }

            try
            {
                // Hash the password before saving
                pharmacy.Password = HashPassword(pharmacy.Password);

                var registeredPharmacy = await _pharmacyService.RegisterPharmacyAsync(pharmacy);
                return CreatedAtAction(nameof(RegisterPharmacy), new { id = registeredPharmacy.Id }, registeredPharmacy);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });  // Return 409 Conflict for duplicate entries
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
                var pharmacy = await _pharmacyService.LoginAsync(email, password);
                if (pharmacy == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                // Ensure Pharmacy ID is included in the response
                return Ok(new LoginResponse
                {
                    Message = "Login successful",
                    Pharmacy = new Pharmacy
                    {
                        Id = pharmacy.Id,  // Ensure the Pharmacy ID is properly sent
                        PharmacyName = pharmacy.PharmacyName,
                        Email = pharmacy.Email,
                        RegistrationNumber = pharmacy.RegistrationNumber,
                        ContactNumber = pharmacy.ContactNumber
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpGet("all")]
        public async Task<IActionResult> GetAllPharmacies()
        {
            try
            {
                var pharmacies = await _pharmacyService.GetAllPharmaciesAsync();
                return Ok(pharmacies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPharmacies([FromQuery] string query)
        {
            try
            {
                var pharmacies = await _pharmacyService.GetAllPharmaciesAsync();
                var filtered = pharmacies.Where(p =>
                    p.PharmacyName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.RegistrationNumber.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return Ok(filtered);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePharmacy(int id)
        {
            try
            {
                var result = await _pharmacyService.DeletePharmacyAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Pharmacy not found." });
                }
                return Ok(new { message = "Pharmacy deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("check-pharmacy/{pharmacyId}")]
        public async Task<IActionResult> CheckPharmacyId(int pharmacyId)
        {
            var pharmacy = await _pharmacyService.GetPharmacyById(pharmacyId);

            if (pharmacy == null)
            {
                return NotFound(new { message = "Pharmacy ID not found." });
            }

            return Ok(new { message = "Pharmacy ID is valid." });
        }


    }
}
