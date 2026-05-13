using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class RealEstateTypeRepository : GenericRepository<RealEstateType>, IRealEstateTypeRepository
{
    public RealEstateTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<RealEstateType>> GetByNameAsync(string typeName)
    {
        var realEstateType = await _context.RealEstateTypes
            .FirstOrDefaultAsync(ret => ret.TypeName == typeName);

        if (realEstateType == null)
            return Result<RealEstateType>.Failure(Error.NotFound($"Tipo '{typeName}' não encontrado."));
        
        return Result<RealEstateType>.Success(realEstateType);
    }

    public async Task<Result<IEnumerable<RealEstateType>>> GetActiveTypesAsync()
    {
        var allTypes = await _context.RealEstateTypes.ToListAsync();

        var activeTypes = allTypes.Where(ret => ret.GetIsActive());

        return Result<IEnumerable<RealEstateType>>.Success(activeTypes);
    }

    public async Task<Result<Dictionary<string, int>>> GetTypeUsageCountAsync()
    {
        var usageCount = await _context.RealEstates
            .GroupBy(re => re.RealEstateTypeId)
            .Select(g => new 
            {
                TypeName = _context.RealEstateTypes
                    .Where(ret => ret.RealEstateTypeId == g.Key)
                    .Select(ret => ret.TypeName)
                    .FirstOrDefault() ?? "Unknown",
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.TypeName, x => x.Count);

            return Result<Dictionary<string, int>>.Success(usageCount);
    }
}
