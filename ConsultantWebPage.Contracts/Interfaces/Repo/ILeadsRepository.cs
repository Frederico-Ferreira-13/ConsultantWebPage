using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface ILeadsRepository : IGenericRepository<Leads>
{
    Task<Result<IEnumerable<Leads>>> GetByConsultantIdAsync(int consultantId);
    Task<Result<IEnumerable<Leads>>> GetByRealEstateIdAsync(int realEstateId);
    Task<int> GetCountByRealEstateIdAsync(int realEstateId);
    Task<Result<IEnumerable<Leads>>> GetByStatusAsync(string status);
    Task<bool> HasRecentLeadFromEmailAsync(string email, int realEstateId);
}
