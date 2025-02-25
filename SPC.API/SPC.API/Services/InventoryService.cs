using Microsoft.EntityFrameworkCore;
using SPC.API.Data;
using SPC.API.Models;

namespace SPC.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateStock(int drugId, int quantity)
        {
            var drug = await _context.Drugs.FindAsync(drugId);
            if (drug == null) throw new KeyNotFoundException("Drug not found");

            drug.StockLevel = quantity;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Drug>> SearchDrugs(string searchTerm)
        {
            return await _context.Drugs
                .Where(d => d.Name.Contains(searchTerm) || d.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<Drug> AddDrug(Drug drug)
        {
            if (drug == null) throw new ArgumentNullException(nameof(drug));

            try
            {
                _context.Drugs.Add(drug);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding drug: {ex.Message}");
                throw;
            }

            return drug;
        }

        public async Task<IEnumerable<Drug>> GetAllDrugs()
        {
            return await _context.Drugs.ToListAsync();
        }

        public async Task DeleteDrug(int drugId)
        {
            // Your database deletion logic here
            // Example with Entity Framework:
            var drug = await _context.Drugs.FindAsync(drugId);
            if (drug == null)
            {
                throw new Exception("Drug not found");
            }

            _context.Drugs.Remove(drug);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Drug>> GetDrugsByManufacturingPlant(string manufacturingPlantId)
        {
            if (string.IsNullOrEmpty(manufacturingPlantId))
            {
                throw new ArgumentException("Manufacturing plant ID cannot be null or empty");
            }

            return await _context.Drugs
                .Where(d => d.ManufacturingPlantId == manufacturingPlantId)
                .ToListAsync();
        }


    }
}
