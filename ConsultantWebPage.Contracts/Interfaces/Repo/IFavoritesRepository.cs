using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IFavoritesRepository : IGenericRepository<Favorites>
{
    Task<Result<IEnumerable<Favorites>>> GetByUserIdAsync(int userId);
    Task<bool> IsFavoriteAsync(int userId, int realEstateId);
    Task<int> GetCountByRealEstateIdAsync(int realEstateId);
    Task DeleteFavoriteAsync(int userId, int realEstateId);
    Task<Result> RemoveFavoriteAsync(int userId, int realEstateId);
    Task<Result<Favorites>> GetByUserAndRealEstateAsync(int userId, int realEstateId);
}
