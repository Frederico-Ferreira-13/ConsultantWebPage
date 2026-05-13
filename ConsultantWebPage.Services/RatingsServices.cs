using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Services;

public class RatingsServices : IRatingsServices
{
    private readonly IUnitOfWork _unitOfWork;

    public RatingsServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> AddOrUpdateRatingAsync(int userId, int realEstateId, int stars)
    {
        // Regra de negócio: as estrelas devem ser entre 1 e 5
        if (stars < 1 || stars > 5)
            return Result.Failure(Error.Validation("A avaliação deve ser entre 1 e 5 estrelas."));

        var result = await _unitOfWork.Rating.UpsertRatingAsync(userId, realEstateId, stars);
        await _unitOfWork.CommitAsync();
        
        return result;
    }

    public async Task<Result<double>> GetAverageForPropertyAsync(int realEstateId)
    {
        return await _unitOfWork.Rating.GetAverageRatingAsync(realEstateId);
    }
}