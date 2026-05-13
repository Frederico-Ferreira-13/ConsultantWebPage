using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class RealEstateCategoriesRepository : GenericRepository<RealEstateCategories>, IRealEstateCategoriesRepository
{
    public RealEstateCategoriesRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<RealEstateCategories>>> GetByRealEstateIdAsync(int realEstateId)
    {
        var categories = await _context.RealEstateCategories
            .Include(rec => rec.Category)
            .Where(rec => rec.RealEstateId == realEstateId)
            .ToListAsync();

        return Result<IEnumerable<RealEstateCategories>>.Success(categories);  
    }

    public async Task<Result<IEnumerable<RealEstateCategories>>> GetByCategoryIdAsync(int categoryId)
    {
        var realEstates = await _context.RealEstateCategories
            .Include(rec => rec.RealEstate)
            .Where(rec => rec.CategoriesId == categoryId)
            .ToListAsync();

        return Result<IEnumerable<RealEstateCategories>>.Success(realEstates);
    }

    public async Task<Result> RemoveAllFromRealEstateAsync(int realEstateId)
    {
        var categories = await _context.RealEstateCategories
            .Where(rec => rec.RealEstateId == realEstateId)
            .ToListAsync();

        _context.RealEstateCategories.RemoveRange(categories);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<bool> IsRealEstateInCategoryAsync(int realEstateId, int categoryId)
    {
        // Implementação para verificar se um imóvel pertence a uma categoria
        var exists = await _context.RealEstateCategories
            .AnyAsync(rec => rec.RealEstateId == realEstateId && rec.CategoriesId == categoryId);

        return exists;
    }
}
