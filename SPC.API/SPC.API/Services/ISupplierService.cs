using SPC.API.Models;

namespace SPC.API.Services
{
    public interface ISupplierService
    {
        Task<Supplier> RegisterSupplier(Supplier supplier);
        Task<IEnumerable<Supplier>> GetAllSuppliers();
        Task<IEnumerable<Supplier>> SearchSuppliers(string searchTerm);
        Task<bool> DeleteSupplier(int id);
        Task<Supplier> LoginAsync(string email, string password);
        Task<bool> UpdateSupplierApproval(int id, bool isApproved);

    }
}
