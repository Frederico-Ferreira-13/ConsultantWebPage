using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IRealEstateMediaRepository : IGenericRepository<RealEstateMedia>
{    
    Task<Result<IEnumerable<RealEstateMedia>>> GetByRealEstateIdAsync(int realEstateId);
    
    Task<Result<RealEstateMedia>> GetMainImageByRealEstateIdAsync(int realEstateId);
    
    Task<Result> SetMainImageAsync(int realEstateId, int mediaId);
    
    Task<Result> UpdateMediaOrderAsync(int realEstateId, List<int> orderedMediaIds);
    
    Task<int> GetMediaCountByRealEstateIdAsync(int realEstateId);
}
