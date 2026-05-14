using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IFavoritesServices
{
    Task<Result<int>> GetCurrentUserIdAsync();
    Task<Result> AddFavoriteAsync(Favorites favorite);
    Task<Result<IEnumerable<Favorites>>> GetUserFavoritesAsync(int userId);
    Task<Result> RemoveFavoriteAsync(int favoriteId);
    Task<Result<bool>> ToggleFavoriteAsync(int realEstateId);
}