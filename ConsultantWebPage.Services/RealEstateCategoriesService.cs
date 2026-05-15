using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Services;

public class RealEstateCategoriesService
{
    public async Task<Result> AssignCategoryToPropertyAsync(int realEstateId, int categoryId)
    {
        // Implementation here
    }

    public async Task<Result> RemoveCategoryFromPropertyAsync(int realEstateId, int categoryId)
    {
        // Implementation here
    }

    public async Task<Result> SyncPropertyCategoriesAsync(int realEstateId, IEnumerable<int> categoryIds)
    {
        // Implementation here
    }

    public async Task<Result<IEnumerable<Category>>> GetCategoriesByPropertyIdAsync(int realEstateId)
    {
        // Implementation here
    }

    public async Task<Result<IEnumerable<RealEstate>>> GetPropertiesByCategoryIdAsync(int categoryId)
    {
        // Implementation here
    }

    public async Task<bool> IsPropertyInCategoryAsync(int realEstateId, int categoryId)
    {
        // Implementation here
    }
}
