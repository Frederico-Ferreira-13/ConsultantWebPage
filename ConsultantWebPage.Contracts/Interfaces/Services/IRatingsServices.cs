using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IRatingsServices
{
    Task<Result> AddOrUpdateRatingAsync(int userId, int realEstateId, int stars);
    Task<Result<double>> GetAverageForPropertyAsync(int realEstateId);
}