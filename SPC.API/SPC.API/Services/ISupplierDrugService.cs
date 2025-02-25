using SPC.API.Models;

namespace SPC.API.Services
{
    public interface ISupplierDrugService
    {
        Task<SupplierDrug> AddSupplierDrug(SupplierDrug supplierDrug);
        Task<IEnumerable<SupplierDrug>> GetDrugsBySupplier(int supplierId);
        Task<IEnumerable<SupplierDrug>> SearchDrugs(string searchTerm);
        Task<SupplierDrug> GetDrugById(int id);
        Task UpdateStock(int id, int quantity);
    
        Task<bool> DeleteSupplierDrug(int id);
        Task<IEnumerable<SupplierDrug>> GetAllDrugs();
    }
}
