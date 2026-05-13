using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class FavoritesServices : IFavoritesServices
{
    private readonly IUnitOfWork _unitOfWork;

    public FavoritesServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ToggleFavoriteAsync(int userId, int realEstateId)
    {
        var isFavorite = await _unitOfWork.Favorites.IsFavoriteAsync(userId, realEstateId);

        if (isFavorite)
        {
            await _unitOfWork.Favorites.RemoveFavoriteAsync(userId, realEstateId);
        }
        else
        {
            var favorite = new Favorites { UserId = userId, RealEstateId = realEstateId };
            await _unitOfWork.Favorites.AddAsync(favorite);
        }

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }

    public async Task<Result<IEnumerable<Favorites>>> GetUserFavoritesAsync(int userId)
    {
        return await _unitOfWork.Favorites.GetByUserIdAsync(userId);
    }
}