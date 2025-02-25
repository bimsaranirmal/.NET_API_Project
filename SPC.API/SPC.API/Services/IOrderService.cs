using SPC.API.Models;

namespace SPC.API.Services
{
    public interface IOrderService
    {
        Task<Order> PlaceOrder(Order order);
        Task<List<Order>> GetAllOrders();
        Task<Order> GetOrderById(int orderId);
        Task<Order> UpdateOrderStatus(int orderId, string newStatus);
        Task<List<Order>> GetOrdersByPharmacyId(int pharmacyId);
    }
}
