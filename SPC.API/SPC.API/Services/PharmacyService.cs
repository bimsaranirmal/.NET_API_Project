using SPC.API.Data;
using SPC.API.Models;
using Microsoft.EntityFrameworkCore;
using SPC.Web.Models;
using System.Text;
using System.Security.Cryptography;

namespace SPC.API.Services
{
    public class PharmacyService : IPharmacyService
    {
        private readonly ApplicationDbContext _context;

        public PharmacyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Pharmacy> RegisterPharmacyAsync(Pharmacy pharmacy)
        {
            // Check if Email is already used
            if (await _context.Pharmacies.AnyAsync(p => p.Email == pharmacy.Email))
            {
                throw new InvalidOperationException("A pharmacy with this email already exists.");
            }

            // Check if Registration Number is already used
            if (await _context.Pharmacies.AnyAsync(p => p.RegistrationNumber == pharmacy.RegistrationNumber))
            {
                throw new InvalidOperationException("A pharmacy with this registration number already exists.");
            }

            // Ensure contact number is exactly 10 digits
            if (!System.Text.RegularExpressions.Regex.IsMatch(pharmacy.ContactNumber, @"^\d{10}$"))
            {
                throw new InvalidOperationException("Contact number must be exactly 10 digits.");
            }

            _context.Pharmacies.Add(pharmacy);
            await _context.SaveChangesAsync();
            return pharmacy;
        }


        public async Task<IEnumerable<Pharmacy>> GetAllPharmaciesAsync()
        {
            return await _context.Pharmacies.ToListAsync();
        }

        public async Task<IEnumerable<Pharmacy>> SearchPharmaciesAsync(string query)
        {
            return await _context.Pharmacies
                .Where(p => EF.Functions.Like(p.PharmacyName, $"%{query}%") ||
                            EF.Functions.Like(p.RegistrationNumber, $"%{query}%") ||
                            EF.Functions.Like(p.Email, $"%{query}%"))
                .ToListAsync();
        }

        public async Task<bool> DeletePharmacyAsync(int id)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);
            if (pharmacy == null)
            {
                return false; // Pharmacy not found
            }

            _context.Pharmacies.Remove(pharmacy);
            await _context.SaveChangesAsync();
            return true; // Successfully deleted
        }
        public async Task<Pharmacy> LoginAsync(string email, string password)
        {
            var pharmacy = await _context.Pharmacies
                                          .FirstOrDefaultAsync(p => p.Email == email);

            if (pharmacy == null)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            if (!VerifyPassword(password, pharmacy.Password))
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            return pharmacy;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string password, string storedHashedPassword)
        {
            string hashedPassword = HashPassword(password);
            return hashedPassword == storedHashedPassword;
        }

        public async Task<Pharmacy> GetPharmacyById(int pharmacyId)
        {
            return await _context.Pharmacies
                                 .FirstOrDefaultAsync(p => p.Id == pharmacyId);
        }
    }
}
