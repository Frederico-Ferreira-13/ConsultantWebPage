using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IRealEstateTypeServices
{
    Task<Result<IEnumerable<RealEstateType>>> GetAllRealEstateTypesAsync();
    Task<Result<RealEstateType>> GetRealEstateTypeByIdAsync(int id);
    Task<Result<RealEstateType>> CreateRealEstateTypeAsync(RealEstateType realEstateType);
    Task<Result<RealEstateType>> UpdateRealEstateTypeAsync(RealEstateType updateRealEstateType);
    Task<Result> DeleteRealEstateTypeAsync(int id);
    Task<bool> TypeNameExistsAsync(string typeName);
}
