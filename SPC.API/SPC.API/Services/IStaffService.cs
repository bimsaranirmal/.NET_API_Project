using SPC.API.Models;

namespace SPC.API.Services
{
    public interface IStaffService
    {
        Task<Staff> RegisterStaffAsync(Staff staff);
        Task<Staff> LoginAsync(string email, string password);
        Task<IEnumerable<Staff>> GetAllStaffAsync();
        Task<bool> DeleteStaffAsync(int id);
    }
}
