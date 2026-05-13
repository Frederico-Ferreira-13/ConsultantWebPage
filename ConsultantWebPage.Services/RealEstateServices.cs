using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Services;

public class RealEstateServices : IRealEstateServices
{
    private readonly IUnitOfWork _unitOfWork;

    public RealEstateServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<RealEstate>>> GetAllActivePropertiesAsync()
    {
        // Aqui usamos o repositório genérico através da Unit of Work
        return await _unitOfWork.RealEstates.GetAllAsync();
    }

    public async Task<Result<RealEstate>> GetPropertyDetailsAsync(int id)
    {
        return await _unitOfWork.RealEstates.GetByIdAsync(id);
    }
}