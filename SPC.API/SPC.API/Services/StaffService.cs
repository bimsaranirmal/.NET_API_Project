using Microsoft.EntityFrameworkCore;
using SPC.API.Data;
using SPC.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace SPC.API.Services
{
    public class StaffService : IStaffService
    {
        private readonly ApplicationDbContext _context;

        public StaffService(ApplicationDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public async Task<Staff> RegisterStaffAsync(Staff staff)
        {
            // Check if the email already exists
            var existingStaff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == staff.Email);
            if (existingStaff != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            staff.Password = HashPassword(staff.Password);
            staff.RegistrationDate = DateTime.UtcNow;
            staff.IsActive = true;

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
            return staff;
        }


        public async Task<Staff> LoginAsync(string email, string password)
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email);
            if (staff == null || staff.Password != HashPassword(password))
            {
                return null;  // Invalid login credentials
            }
            return staff;  // Return the staff object on successful login
        }

        public async Task<IEnumerable<Staff>> GetAllStaffAsync()
        {
            return await _context.Staff.ToListAsync(); // Ensure this line is correctly fetching staff from DB
        }


        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return false;
            }

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
