using System;

namespace SPC.Web.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }  // Store hashed password
        public string Role { get; set; }  // Staff role (e.g., Admin, User)
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }  // Indicates if the staff is active
    }
}
