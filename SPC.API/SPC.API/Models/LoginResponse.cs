using SPC.Web.Models;

namespace SPC.API.Models
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public Pharmacy Pharmacy { get; set; }
    }
}
