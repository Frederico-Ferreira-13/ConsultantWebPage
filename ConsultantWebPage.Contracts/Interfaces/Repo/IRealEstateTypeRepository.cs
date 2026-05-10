using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IRealEstateTypeRepository : IGenericRepository<RealEstateType>
{
    Task<Result<RealEstateType>> GetByNameAsync(string typeName);

    Task<Result<IEnumerable<RealEstateType>>> GetActiveTypesAsync();

    Task<Result<Dictionary<string, int>>> GetTypeUsageCountAsync();
}