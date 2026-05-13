using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IFavoritesServices
{
    Task<Result> ToggleFavoriteAsync(int userId, int realEstateId);
    Task<Result<IEnumerable<Favorites>>> GetUserFavoritesAsync(int userId);
}