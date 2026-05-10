using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IRealEstateRepository : IGenericRepository<RealEstate>
{
    Task<(IEnumerable<RealEstate> Items, int TotalCount)> SearchAsync(
        string? searchTerm, 
        int? categoryId, 
        decimal? minPrice, 
        decimal? maxPrice,
        int page, 
        int pageSize);

    Task<Result<IEnumerable<RealEstate>>> GetFeaturedAsync(int count);    
}
