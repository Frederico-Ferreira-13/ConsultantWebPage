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

public class CitiesRepository : GenericRepository<Cities>, ICitiesRepository
{
    public CitiesRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<Cities>> GetByNameAsync(string cityName)
    {
       var city = await _dbSet
             .FirstOrDefaultAsync(ct => ct.CityName.ToLower() == cityName.ToLower()); 

        if (city == null)
            return Result<Cities>.Failure(Error.NotFound($"A cidade '{cityName}' não foi encontrada."));

        return Result<Cities>.Success(city);
    }

    public async Task<Result<IEnumerable<Cities>>> GetByDistrictAsync(string districtName)
    {
        var cities = await _dbSet
            .Where(ct => ct.DistrictName.ToLower() == districtName.ToLower())
            .ToListAsync();

        if (cities == null || !cities.Any())
            return Result<IEnumerable<Cities>>.Failure(Error.NotFound($"Nenhuma cidade encontrada para o distrito '{districtName}'."));

        return Result<IEnumerable<Cities>>.Success(cities);
    }
}
