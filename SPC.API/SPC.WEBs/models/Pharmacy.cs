// Models/Pharmacy.cs
using System.ComponentModel.DataAnnotations;

namespace SPC.Web.Models
{
    public class Pharmacy
    {
        public int Id { get; set; }  // Pharmacy ID (auto-generated)

        [Required]
        public string PharmacyName { get; set; }  // Name of the pharmacy

        [Required]
        public string Location { get; set; }  // Location of the pharmacy

        [Required]
        [Phone]
        public string ContactNumber { get; set; }  // Pharmacy contact number

        [Required]
        [EmailAddress]
        public string Email { get; set; }  // Pharmacy email address

        [Required]
        [MaxLength(50)]
        public string RegistrationNumber { get; set; }  // Pharmacy registration number

        [Required]
        [MinLength(8)]  // Password should have at least 8 characters
        public string Password { get; set; }
    }
}
