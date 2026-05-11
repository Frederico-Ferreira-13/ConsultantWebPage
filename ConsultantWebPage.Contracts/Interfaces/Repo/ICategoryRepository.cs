using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Result<Category>> GetCategoryByNameAndAccount(string categoryName, int accountId);
    Task<Result<IEnumerable<Category>>> GetRootCategoriesByAccount(int accountId);
    Task<Result<Category>> GetByIdWithSubCategories(int categoryId, int accountId);    
}
