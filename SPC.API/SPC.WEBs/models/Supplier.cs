using System;

namespace SPC.Web.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsApproved { get; set; }
        public string Password { get; set; }
    }
}

