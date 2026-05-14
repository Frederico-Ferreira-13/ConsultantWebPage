using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ICitiesServices
{
    Task<Result<Cities>> GetCityByIdAsync(int id);
    Task<Result<Cities>> GetCityByNameAsync(string name);
    Task<Result<IEnumerable<Cities>>> GetByDistrictAsync(string district);
    Task<Result<IEnumerable<Cities>>> GetAllCitiesAsync();

    Task<Result<IEnumerable<string>>> GetAllDistrictsAsync();

    Task<Result<Cities>> CreateCityAsync(string name, string district);
    Task<Result> UpdateCityAsync(int id, string name, string district);    
}