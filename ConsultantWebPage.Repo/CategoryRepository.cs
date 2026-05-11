using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<Category>> GetCategoryByNameAndAccount(string categoryName, int accountId)
    {
        var category = await _dbSet
            .Where(c => c.CategoryName == categoryName)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return category != null 
            ? Result<Category>.Success(category) 
            : Result<Category>.Failure(Error.NotFound("Categoria não encontrada."));
    }

    public async Task<Result<IEnumerable<Category>>> GetRootCategoriesByAccount(int accountId)
    {
        var roots = await _dbSet
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.CategoryType)
            .AsNoTracking()
            .ToListAsync();

        return Result<IEnumerable<Category>>.Success(roots);
    }

    public async Task<Result<Category>> GetByIdWithSubCategories(int categoryId, int accountId)
    {
        var category = await _dbSet
            .Include(c => c.SubCategories)
            .Include(c => c.CategoryType)
            .FirstOrDefaultAsync(c => c.CategoriesId == categoryId);

        return category != null 
            ? Result<Category>.Success(category) 
            : Result<Category>.Failure(Error.NotFound("Categoria não encontrada."));
    }   
}
