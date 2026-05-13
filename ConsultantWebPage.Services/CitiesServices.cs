using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class CitiesServices : ICitiesServices
{
    private readonly IUnitOfWork _unitOfWork;

    public CitiesServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<Cities>>> GetByDistrictAsync(string district)
    {
        return await _unitOfWork.Cities.GetByDistrictAsync(district);
    }

    public async Task<Result<Cities>> GetCityByNameAsync(string name)
    {
        return await _unitOfWork.Cities.GetByNameAsync(name);
    }
}