using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ICategoryServices
{
    Task<Result<Category>> GetCategoryByIdAsync(int categoryId);
    Task<Result<Category>> GetCategoryByNameAsync(string categoryName);
    Task<Result<IEnumerable<Category>>> GetRootCategoriesAsync(); // Root não costuma precisar de ID se vier do contexto
    Task<Result<Category>> GetCategoryWithSubsAsync(int categoryId);

    Task<Result<Category>> CreateCategoryAsync(Category newCategory);
    Task<Result<Category>> UpdateCategoryAsync(Category updateCategory);
    Task<Result> DeleteCategoryAsync(int categoryId);        

    Task<Result<IEnumerable<CategoryType>>> GetAllCategoryTypesAsync();
    Task<Result<CategoryType>> GetCategoryTypeByIdAsync(int id);

    Task<Result<CategoryType>> GetCategoryTypeByNameAsync(string name);
    Task<Result<CategoryType>> CreateCategoryTypeAsync(CategoryType categoryType);
    Task<Result<CategoryType>> UpdateCategoryTypeAsync(CategoryType updateCategoryType);
    Task<Result> DeleteCategoryTypeAsync(int id);
}