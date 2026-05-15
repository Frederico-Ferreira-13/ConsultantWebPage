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

    public async Task<Result<Category>> GetCategoryByIdAsync(int categoryId)
    {
        return await _unitOfWork.Categories.GetByIdAsync(categoryId);
    }

    public async Task<Result<IEnumerable<Category>>> GetRootCategoriesAsync()
    {
        return await _unitOfWork.Categories.GetRootCategoriesAsync();
    }

    public async Task<Result<Category>> GetCategoryWithSubsAsync(int categoryId)
    {
        return await _unitOfWork.Categories.GetByIdWithSubCategories(categoryId);
    }

    public async Task<Result<Category>> CreateCategoryAsync(Category newCategory)
    {
        return await _unitOfWork.Categories.CreateAsync(newCategory);
    }

    public async Task<Result<Category>> UpdateCategoryAsync(Category updateCategory)
    {
        return await _unitOfWork.Categories.UpdateAsync(updateCategory);
    }

    public async Task<Result>> DeleteCategoryAsync(int categoryId)
    {
        return await _unitOfWork.Categories.DeleteAsync(categoryId);
    }

    public async Task<Result<IEnumerable<CategoryType>>> GetAllCategoryTypesAsync()
    {
        return await _unitOfWork.CategoryTypes.GetAllAsync();
    }

    public async Task<Result<CategoryType>> GetCategoryTypeByIdAsync(int id)
    {
        return await _unitOfWork.CategoryTypes.GetByIdAsync(id);
    }

    public async Task<Result<CategoryType>> GetCategoryTypeByNameAsync(string name)
    {
        return await _unitOfWork.CategoryTypes.ByNameAsync(name);
    }

    public async Task<Result<CategoryType>> CreateCategoryTypeAsync(CategoryType categoryType)
    {
        return await _unitOfWork.CategoryTypes.CreateAsync(categoryType);
    }

    public async Task<Result<CategoryType>> UpdateCategoryTypeAsync(CategoryType updateCategoryType)
    {
        return await _unitOfWork.CategoryTypes.UpdateAsync(updateCategoryType);
    }

    public async Task<Result>> DeleteCategoryTypeAsync(int id)
    {
        return await _unitOfWork.CategoryTypes.DeleteAsync(id);
    }   
}