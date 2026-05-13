using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ICategoryServices
{
    Task<Result<IEnumerable<Category>>> GetRootCategoriesAsync(int accountId);
    Task<Result<Category>> GetCategoryWithSubsAsync(int categoryId, int accountId);
}