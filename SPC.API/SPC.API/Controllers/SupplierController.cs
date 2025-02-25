using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SPC.API.Models;
using SPC.API.Services;

namespace SPC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Supplier>> RegisterSupplier(Supplier supplier)
        {
            if (supplier == null)
            {
                return BadRequest("Supplier data is null");
            }

            if (string.IsNullOrWhiteSpace(supplier.Password))
            {
                return BadRequest("Password is required");
            }

            try
            {
                // Hash the password before saving it
                supplier.Password = HashPassword(supplier.Password);

                var result = await _supplierService.RegisterSupplier(supplier);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message }); // Handle duplicate email/registration number/mobile number error
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }



        // Password hashing method
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
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
                var supplier = await _supplierService.LoginAsync(email, password);
                if (supplier == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                // Match the pharmacy login response structure
                return Ok(new LoginResponseSupplier
                {
                    
                    Supplier = new Supplier
                    {
                        Id = supplier.Id,
                        Name = supplier.Name,
                        Email = supplier.Email,
                        RegistrationNumber = supplier.RegistrationNumber,
                        ContactNumber = supplier.ContactNumber,
                        Address = supplier.Address,
                        RegistrationDate = supplier.RegistrationDate,
                        IsApproved = supplier.IsApproved
                    },
                    Message = "Login successful",
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message }); // Return error for unapproved users
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        // Password verification method
        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var inputHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
                return Convert.ToBase64String(inputHash) == hashedPassword;
            }
        }

        // Get all suppliers
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliers();
            return Ok(suppliers);
        }

        // Search suppliers by name, registration number, or email
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Supplier>>> SearchSuppliers([FromQuery] string searchTerm)
        {
            var suppliers = await _supplierService.SearchSuppliers(searchTerm);
            return Ok(suppliers);
        }

        // Delete a supplier by ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var result = await _supplierService.DeleteSupplier(id);
            if (!result)
            {
                return NotFound("Supplier not found");
            }
            return Ok("Supplier deleted successfully");
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveSupplier(int id)
        {
            var result = await _supplierService.UpdateSupplierApproval(id, true);
            if (!result)
            {
                return NotFound(new { message = "Supplier not found or already approved." });
            }
            return Ok(new { message = "Supplier approved successfully." });
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectSupplier(int id)
        {
            var result = await _supplierService.UpdateSupplierApproval(id, false);
            if (!result)
            {
                return NotFound(new { message = "Supplier not found or already rejected." });
            }
            return Ok(new { message = "Supplier rejected successfully." });
        }


    }
}
