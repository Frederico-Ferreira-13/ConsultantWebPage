using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IRealEstateCategoriesRepository : IGenericRepository<RealEstateCategories>
{
    Task<Result<IEnumerable<RealEstateCategories>>> GetByRealEstateIdAsync(int realEstateId);
    Task<Result<IEnumerable<RealEstateCategories>>> GetByCategoryIdAsync(int categoryId);
    Task<Result> RemoveAllFromRealEstateAsync(int realEstateId);
    Task<bool> IsRealEstateInCategoryAsync(int realEstateId, int categoryId);
}
