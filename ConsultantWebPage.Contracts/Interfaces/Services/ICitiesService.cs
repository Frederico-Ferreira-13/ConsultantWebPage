using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ICitiesServices
{
    Task<Result<IEnumerable<Cities>>> GetByDistrictAsync(string district);
    Task<Result<Cities>> GetCityByNameAsync(string name);
}