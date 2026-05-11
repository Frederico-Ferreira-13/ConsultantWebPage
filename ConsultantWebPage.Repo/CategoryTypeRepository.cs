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

public class CategoryTypeRepository : GenericRepository<CategoryType>, ICategoryTypeRepository
{
    public CategoryTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<CategoryType>> GetByNameAsync(string typeName)
    {
        var categoryType = await _dbSet
            .FirstOrDefaultAsync(ct => ct.TypeName.ToLower() == typeName.ToLower()); 

        if (categoryType == null)
            return Result<CategoryType>.Failure(Error.NotFound($"O tipo de categoria '{typeName}' não existe."));

        return Result<CategoryType>.Success(categoryType);
    }        
}
