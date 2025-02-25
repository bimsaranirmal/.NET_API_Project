using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SPC.API.Data;
using SPC.API.Models;

namespace SPC.API.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _context;

        public SupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Supplier> RegisterSupplier(Supplier supplier)
        {
            // Check if the email is already in use
            var existingSupplierByEmail = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Email == supplier.Email);
            if (existingSupplierByEmail != null)
            {
                throw new InvalidOperationException("The email address is already registered.");
            }

            // Check if the registration number is already in use
            var existingSupplierByRegNum = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.RegistrationNumber == supplier.RegistrationNumber);
            if (existingSupplierByRegNum != null)
            {
                throw new InvalidOperationException("The registration number is already in use.");
            }

            // Validate mobile number (must be exactly 10 digits)
            if (!Regex.IsMatch(supplier.ContactNumber, @"^\d{10}$"))
            {
                throw new InvalidOperationException("The mobile number must be exactly 10 digits.");
            }

            // Proceed with the registration
            supplier.RegistrationDate = DateTime.UtcNow;
            supplier.IsApproved = false;

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return supplier;
        }


        public async Task<IEnumerable<Supplier>> GetAllSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> SearchSuppliers(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return await GetAllSuppliers();
            }

            return await _context.Suppliers
                .Where(s => s.Name.Contains(searchTerm) ||
                            s.RegistrationNumber.Contains(searchTerm) ||
                            s.Email.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return false;
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Supplier> LoginAsync(string email, string password)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Email == email);

            if (supplier == null)
            {
                return null; // Supplier not found
            }

            // Check if the supplier is approved before allowing login
            if (!supplier.IsApproved)
            {
                throw new UnauthorizedAccessException("Your account has not been approved yet.");
            }

            // Verify password
            if (!VerifyPassword(password, supplier.Password))
            {
                return null; // Invalid password
            }

            return supplier; // Successful login
        }


        // Password verification method (used for Login)
        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var inputHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
                return Convert.ToBase64String(inputHash) == storedHashedPassword;
            }
        }

        public async Task<bool> UpdateSupplierApproval(int id, bool isApproved)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return false; // Supplier not found
            }

            supplier.IsApproved = isApproved;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
