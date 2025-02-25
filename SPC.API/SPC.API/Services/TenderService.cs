using Microsoft.EntityFrameworkCore;
using SPC.API.Data;
using SPC.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPC.API.Services
{
    public class TenderService : ITenderService
    {
        private readonly ApplicationDbContext _context;

        public TenderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tender>> GetAllTenders()
        {
            return await _context.Tenders.ToListAsync();
        }

        public async Task<IEnumerable<Tender>> GetTendersBySupplierId(int supplierId)
        {
            return await _context.Tenders
                .Where(t => t.SupplierId == supplierId)
                .ToListAsync();
        }


        public async Task<IEnumerable<Tender>> SearchTenders(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllTenders(); // Return all if no search term is provided
            }

            return await _context.Tenders
                .Where(t => t.SupplierId.ToString().Contains(searchTerm) ||
                            t.Description.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> SubmitTender(Tender tender)
        {
            _context.Tenders.Add(tender);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ApproveTender(int tenderId)
        {
            var tender = await _context.Tenders.FindAsync(tenderId);
            if (tender == null) return false;
            tender.Status = "Approved";
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RejectTender(int tenderId)
        {
            var tender = await _context.Tenders.FindAsync(tenderId);
            if (tender == null) return false;
            tender.Status = "Rejected";
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteTender(int tenderId)
        {
            // Logic to delete tender from database
            var tender = await _context.Tenders.FindAsync(tenderId);
            if (tender == null) return false;

            _context.Tenders.Remove(tender);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkTenderAsDone(int tenderId)
        {
            var tender = await _context.Tenders.FindAsync(tenderId);
            if (tender == null) return false;

            tender.Status = "Done"; // Update the status instead of deleting
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
