using SPC.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPC.API.Services
{
    public interface ITenderService
    {
        Task<IEnumerable<Tender>> GetAllTenders();
        Task<IEnumerable<Tender>> GetTendersBySupplierId(int supplierId);
        Task<IEnumerable<Tender>> SearchTenders(string searchTerm);
        Task<bool> SubmitTender(Tender tender);
        Task<bool> ApproveTender(int tenderId);
        Task<bool> RejectTender(int tenderId);
        Task<bool> DeleteTender(int tenderId);
        Task<bool> MarkTenderAsDone(int tenderId);
    }
}
