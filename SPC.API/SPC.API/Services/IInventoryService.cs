using SPC.API.Models;

namespace SPC.API.Services
{
    public interface IInventoryService
    {
        Task<Drug> AddDrug(Drug drug);
        Task UpdateStock(int drugId, int quantity);
        Task<IEnumerable<Drug>> SearchDrugs(string searchTerm);
        Task<IEnumerable<Drug>> GetAllDrugs();
        Task DeleteDrug(int drugId);
        Task<IEnumerable<Drug>> GetDrugsByManufacturingPlant(string manufacturingPlantId);
    }

}
