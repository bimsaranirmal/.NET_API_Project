using Microsoft.EntityFrameworkCore;
using SPC.API.Data;
using SPC.API.Models;

namespace SPC.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> PlaceOrder(Order order)
        {
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        // Get all orders
        public async Task<List<Order>> GetAllOrders()
        {
            // Include related order items in the result
            return await _context.Orders
                .Include(order => order.Items)  // Eager load related items
                .ToListAsync();
        }

        // Get a specific order by its ID
        

        public async Task<Order> GetOrderById(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)  // Eager loading the associated order items
                .FirstOrDefaultAsync(o => o.Id == orderId);  // Get the order by its ID

            return order;  // Returns the order along with its items if found, or null if not found
        }

        public async Task<Order> UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            if (newStatus == "Approved")
            {
                if (order.Status != "Pending")
                {
                    throw new InvalidOperationException("Only pending orders can be approved.");
                }

                // Deduct stock for each order item
                foreach (var item in order.Items)
                {
                    var drug = await _context.Drugs.FindAsync(item.DrugId);
                    if (drug == null)
                    {
                        throw new KeyNotFoundException($"Drug with ID {item.DrugId} not found.");
                    }

                    if (drug.StockLevel < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for {drug.Name}. Available: {drug.StockLevel}, Required: {item.Quantity}");
                    }

                    // Deduct stock level
                    drug.StockLevel -= item.Quantity;
                }
            }
            else if (newStatus == "Rejected")
            {
                if (order.Status != "Pending")
                {
                    throw new InvalidOperationException("Only pending orders can be rejected.");
                }
            }

            // Update order status
            order.Status = newStatus;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>> GetOrdersByPharmacyId(int pharmacyId)
        {
            return await _context.Orders
                .Include(order => order.Items)
                .Where(order => order.PharmacyId == pharmacyId.ToString()) // Convert pharmacyId to string
                .OrderByDescending(order => order.OrderDate)
                .ToListAsync();
        }

    }
}
