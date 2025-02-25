using System.Collections.Generic;
using System.Threading.Tasks;
using SPC.API.Models;

namespace SPC.API.Services
{
    public interface ISupplierDrugOrderService
    {
        Task<IEnumerable<SupplierDrugOrder>> GetAllOrdersAsync();
        Task<SupplierDrugOrder> GetOrderByIdAsync(int id);
        Task<SupplierDrugOrder> CreateOrderAsync(SupplierDrugOrder order);
        Task<bool> UpdateOrderAsync(SupplierDrugOrder order); // ✅ Fixed Syntax Error
        Task<bool> DeleteOrderAsync(int id);
        Task<IEnumerable<SupplierDrugOrder>> GetOrdersBySupplierIdAsync(int supplierId);

    }
}
