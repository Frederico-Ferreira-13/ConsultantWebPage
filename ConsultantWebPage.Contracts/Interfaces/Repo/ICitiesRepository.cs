using System;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface ICitiesRepository : IGenericRepository<Cities>
{
    Task<Result<Cities>> GetByNameAsync(string cityName);
    Task<Result<IEnumerable<Cities>>> GetByDistrictAsync(string districtName);
}
