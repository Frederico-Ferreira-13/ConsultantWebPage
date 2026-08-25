using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class RealEstateRepository : GenericRepository<RealEstate>, IRealEstateRepository
{
    public RealEstateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<RealEstate> Items, int TotalCount)> SearchAsync(
        string? searchTerm, 
        int? categoryId, 
        decimal? minPrice, 
        decimal? maxPrice,
        int? rooms,
        int page, 
        int pageSize)
    {
        var query = _context.RealEstates.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(re => re.Title.Contains(searchTerm));

        if (categoryId.HasValue)
            query = query.Where(re => re.RealEstateCategories.Any(rec => rec.CategoriesId == categoryId.Value));

        if (minPrice.HasValue)
            query = query.Where(re => re.Price.Amount >= minPrice.Value); 

        if (maxPrice.HasValue)
            query = query.Where(re => re.Price.Amount <= maxPrice.Value);

        if (rooms.HasValue)
        {
            var typologyName = $"T{rooms.Value}";
            query = query.Where(re => re.RealEstateCategories
                .Any(rec => rec.Category != null && rec.Category.CategoryName == typologyName));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Result<IEnumerable<RealEstate>>> GetFeaturedAsync(int count)
    {
         var featuredRealEstates = await _context.RealEstates
            .Where(re => re.IsFeatured)
            .Take(count)
            .ToListAsync();
        
        return Result<IEnumerable<RealEstate>>.Success(featuredRealEstates);
    }
}
