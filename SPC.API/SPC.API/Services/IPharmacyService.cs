using SPC.Web.Models;

namespace SPC.API.Services
{
    public interface IPharmacyService
    {
        Task<Pharmacy> RegisterPharmacyAsync(Pharmacy pharmacy);
        Task<IEnumerable<Pharmacy>> GetAllPharmaciesAsync();
        Task<IEnumerable<Pharmacy>> SearchPharmaciesAsync(string query);
        Task<bool> DeletePharmacyAsync(int id);
        Task<Pharmacy> LoginAsync(string email, string password);
        Task<Pharmacy> GetPharmacyById(int pharmacyId);

    }

}
