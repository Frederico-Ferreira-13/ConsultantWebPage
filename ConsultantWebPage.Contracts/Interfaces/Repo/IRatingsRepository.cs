using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IRatingsRepository : IGenericRepository<Ratings>
{
    Task<Result<Ratings>> GetByUserAndRealEstateAsync(int userId, int realEstateId);
    Task<Result<IEnumerable<Ratings>>> GetByRealEstateIdAsync(int realEstateId);
    Task<Result<double>> GetAverageRatingAsync(int realEstateId);

    Task<Result<double>> GetAverageRatingForConsultantAsync(int consultantId);

    Task<bool> HasUserRatedAsync(int userId, int realEstateId);
    Task<Result> UpsertRatingAsync(int userId, int realEstateId, int stars);
}
