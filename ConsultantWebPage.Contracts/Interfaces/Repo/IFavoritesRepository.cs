using System;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IFavoritesRepository : IGenericRepository<Favorites>
{
    Task<IEnumerable<Favorites>> GetByUserIdAsync(int userId);
    Task<bool> ExistsAsync(int userId, int realEstateId);
    Task<int> GetCountByRealEstateIdAsync(int recipeId);
    Task DeleteFavoriteAsync(int userId, int realEstateId);
    Task<Favorites?> GetByUserAndRealEstateAsync(int userId, int realEstateId);
}
