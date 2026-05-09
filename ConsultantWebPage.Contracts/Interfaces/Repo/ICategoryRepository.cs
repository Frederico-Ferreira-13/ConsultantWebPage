using System;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetCategoryByNameAndAccount(string categoryName, int accountId);
    Task<List<Category>> GetRootCategoriesByAccount(int accountId);
    Task<Category?> GetByIdWithSubCategories(int categoryId, int accountId);
    Task<Category?> ReadByIdAndAccountAsync(int id, int accountId);
}
