using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IRealEstateServices
{
    Task<Result<IEnumerable<RealEstate>>> GetAllActivePropertiesAsync();
    Task<Result<RealEstate>> GetPropertyDetailsAsync(int id);
}