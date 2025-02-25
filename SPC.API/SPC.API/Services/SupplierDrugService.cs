using Microsoft.EntityFrameworkCore;
using SPC.API.Data;
using SPC.API.Models;

namespace SPC.API.Services
{
    public class SupplierDrugService : ISupplierDrugService
    {
        private readonly ApplicationDbContext _context;

        public SupplierDrugService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add a new drug for a supplier
        public async Task<SupplierDrug> AddSupplierDrug(SupplierDrug supplierDrug)
        {
            // Check if the supplier exists
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == supplierDrug.SupplierId);
            if (!supplierExists)
            {
                throw new InvalidOperationException("Supplier not found.");
            }

            _context.SupplierDrugs.Add(supplierDrug);
            await _context.SaveChangesAsync();
            return supplierDrug;
        }

        // Get all drugs for a specific supplier
        public async Task<IEnumerable<SupplierDrug>> GetDrugsBySupplier(int supplierId)
        {
            return await _context.SupplierDrugs
                .Where(d => d.SupplierId == supplierId)
                .ToListAsync();
        }


        // Search drugs by name, description, or supplier (updated to join with Suppliers)
        public async Task<IEnumerable<SupplierDrug>> SearchDrugs(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return await _context.SupplierDrugs.ToListAsync();
            }

            return await _context.SupplierDrugs
                .Where(d => d.Name.Contains(searchTerm) ||
                            d.Description.Contains(searchTerm) ||
                            _context.Suppliers.Any(s => s.Id == d.SupplierId && s.Name.Contains(searchTerm))) // Join condition
                .ToListAsync();
        }

        // Get drug details by ID
        public async Task<SupplierDrug> GetDrugById(int id)
        {
            return await _context.SupplierDrugs.FindAsync(id);
        }

        // Update drug details


        // Update stock level for a specific supplier's drug
        public async Task UpdateStock(int id, int quantity)
        {
            // Find the supplier drug by its ID
            var supplierDrug = await _context.SupplierDrugs.FindAsync(id);
            if (supplierDrug == null)
            {
                throw new KeyNotFoundException("Supplier drug not found");
            }

            // Update the stock level for the drug
            supplierDrug.StockLevel = quantity;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }


        public async Task<bool> DeleteSupplierDrug(int id)
        {
            var drug = await _context.SupplierDrugs.FindAsync(id);
            if (drug == null)
            {
                return false;
            }

            _context.SupplierDrugs.Remove(drug);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<SupplierDrug>> GetAllDrugs()
        {
            return await _context.SupplierDrugs.ToListAsync();
        }

    }
}

    
