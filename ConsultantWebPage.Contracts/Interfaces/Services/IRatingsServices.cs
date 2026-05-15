using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IRatingsServices
{
    Task<Result> AddOrUpdateRatingAsync(int userId, int realEstateId, int stars);
    
    Task<Result<double>> GetAverageForPropertyAsync(int realEstateId);
    Task<Result<Ratings>> GetUserRatingForPropertyAsync(int realEstateId, int userId);
    Task<Result<IEnumerable<Ratings>>> GetRatingsByPropertyIdAsync(int realEstateId);

    Task<Result<Ratings>> GetRatingByIdAsync(int ratingId);
    Task<Result> DeleteRatingAsync(int ratingId);

    Task<Result<double>> GetAverageRatingForConsultantAsync(int consultantId);
}