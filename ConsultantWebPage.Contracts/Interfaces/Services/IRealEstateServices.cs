using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IRealEstateServices
{
    Task<Result<RealEstate>> GetRealEstateByIdAsync(int id, int currentUserId);
    Task<Result<IEnumerable<RealEstate>>> GetAllActivePropertiesAsync();
    Task<Result<RealEstate>> GetPropertyDetailsAsync(int id);

    Task<Result<RealEstate>> CreatePropertyAsync(RealEstate property, int consultantId);
    Task<Result> UpdatePropertyStatusAsync(int id, bool isActive);

    Task<Result> RequestInformationAsync(int propertyId, string clientName, string clientEmail, string message);
    Task<Result> ScheduleVisitAsync(int propertyId, int clientId, DateTime preferredDate);

    Task<Result> AddCommentAsync(int propertyId, int userId, string commentText);
    Task<Result<IEnumerable<Comments>>> GetCommentsAsync(int propertyId);

    Task<Result<IEnumerable<RealEstate>>> SearchPropertiesAsync(string city, decimal? maxPrice, int? rooms);
}