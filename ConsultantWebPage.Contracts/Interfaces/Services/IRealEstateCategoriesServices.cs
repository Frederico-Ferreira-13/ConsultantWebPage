using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IRealEstateCategoriesServices
{
    Task<Result> AssignCategoryToPropertyAsync(int realEstateId, int categoryId);
    Task<Result> RemoveCategoryFromPropertyAsync(int realEstateId, int categoryId);

    Task<Result> SyncPropertyCategoriesAsync(int realEstateId, IEnumerable<int> categoryIds);
    Task<Result<IEnumerable<Category>>> GetCategoriesByPropertyIdAsync(int realEstateId);

    Task<Result<IEnumerable<RealEstate>>> GetPropertiesByCategoryIdAsync(int categoryId);
    Task<bool> IsPropertyInCategoryAsync(int realEstateId, int categoryId);
}
