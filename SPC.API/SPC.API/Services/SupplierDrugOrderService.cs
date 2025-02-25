using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SPC.API.Data;
using SPC.API.Models;

namespace SPC.API.Services
{
    public class SupplierDrugOrderService : ISupplierDrugOrderService
    {
        private readonly ApplicationDbContext _context;

        public SupplierDrugOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SupplierDrugOrder>> GetAllOrdersAsync()
        {
            return await _context.SupplierDrugOrders.ToListAsync();
        }

        public async Task<SupplierDrugOrder> GetOrderByIdAsync(int id)
        {
            return await _context.SupplierDrugOrders.FindAsync(id);
        }

        public async Task<SupplierDrugOrder> CreateOrderAsync(SupplierDrugOrder order)
        {
            _context.SupplierDrugOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> UpdateOrderAsync(SupplierDrugOrder order)
        {
            var existingOrder = await _context.SupplierDrugOrders.FindAsync(order.OrderId); // Ensure 'Id' exists in model
            if (existingOrder == null) return false;

            _context.Entry(existingOrder).CurrentValues.SetValues(order);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.SupplierDrugOrders.FindAsync(id);
            if (order == null) return false;

            _context.SupplierDrugOrders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SupplierDrugOrder>> GetOrdersBySupplierIdAsync(int supplierId)
        {
            return await _context.SupplierDrugOrders
                                 .Where(order => order.SupplierId == supplierId)
                                 .ToListAsync();
        }

    }
}
