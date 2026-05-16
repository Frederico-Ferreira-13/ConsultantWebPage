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

    Task<Result> AssignCategoryToPropertyAsync(int realEstateId, int categoryId);
    Task<Result> RemoveCategoryFromPropertyAsync(int realEstateId, int categoryId);

    Task<Result> SyncPropertyCategoriesAsync(int realEstateId, IEnumerable<int> categoryIds);
    Task<Result<IEnumerable<Category>>> GetCategoriesByPropertyIdAsync(int realEstateId);

    Task<Result<IEnumerable<RealEstate>>> GetPropertiesByCategoryIdAsync(int categoryId);
    Task<bool> IsPropertyInCategoryAsync(int realEstateId, int categoryId);

    Task<Result<RealEstateMedia>> AddMediaAsync(RealEstateMedia media);
    Task<Result<IEnumerable<RealEstateMedia>>> AddMultipleMediaAsync(IEnumerable<RealEstateMedia> mediaList);
    Task<Result<IEnumerable<RealEstateMedia>>> GetMediaByPropertyIdAsync(int realEstateId);
    Task<Result<RealEstateMedia>> GetMainImageAsync(int realEstateId);
    Task<Result> SetMainImageAsync(int mediaId, int realEstateId);
    Task<Result> DeleteMediaAsync(int mediaId);
    Task<Result> UpdateMediaOrderAsync(List<int> mediaIdsInOrder);

    Task<Result<IEnumerable<RealEstateType>>> GetAllRealEstateTypesAsync();
    Task<Result<RealEstateType>> GetRealEstateTypeByIdAsync(int id);
    Task<Result<RealEstateType>> CreateRealEstateTypeAsync(RealEstateType realEstateType);
    Task<Result<RealEstateType>> UpdateRealEstateTypeAsync(RealEstateType updateRealEstateType);
    Task<Result> DeleteRealEstateTypeAsync(int id);
    Task<bool> TypeNameExistsAsync(string typeName);
}