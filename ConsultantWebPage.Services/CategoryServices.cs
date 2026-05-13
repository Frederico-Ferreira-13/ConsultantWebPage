using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class CategoryServices : ICategoryServices
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<Category>>> GetRootCategoriesAsync(int accountId)
    {
        return await _unitOfWork.Categories.GetRootCategoriesByAccount(accountId);
    }

    public async Task<Result<Category>> GetCategoryWithSubsAsync(int categoryId, int accountId)
    {
        return await _unitOfWork.Categories.GetByIdWithSubCategories(categoryId, accountId);
    }
}