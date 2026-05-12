using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class FavoritesRepository : GenericRepository<Favorites>, IFavoritesRepository
{
    public FavoritesRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<Favorites>>> GetByUserIdAsync(int userId)
    {
        var favorites = await _dbSet
            .Where(f => f.UserId == userId)
            .ToListAsync();

        if (favorites == null || !favorites.Any())
            return Result<IEnumerable<Favorites>>.Failure(Error.NotFound($"Nenhum favorito encontrado para o usuário com ID '{userId}'."));

        return Result<IEnumerable<Favorites>>.Success(favorites);
    }

    public async Task<bool> IsFavoriteAsync(int userId, int realEstateId)
    {
        var isFavorite = await _dbSet
            .AnyAsync(f => f.UserId == userId && f.RealEstateId == realEstateId);

        return isFavorite;
    }

    public async Task<int> GetCountByRealEstateIdAsync(int realEstateId)
    {
        var count = await _dbSet
            .CountAsync(f => f.RealEstateId == realEstateId);

        return count;
    }

    public async Task DeleteFavoriteAsync(int userId, int realEstateId)
    {
        var favorite = await _dbSet
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RealEstateId == realEstateId);

        if (favorite != null)
        {
            _dbSet.Remove(favorite);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Result> RemoveFavoriteAsync(int userId, int realEstateId)
    {
        var favorite = await _dbSet
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RealEstateId == realEstateId);

        if (favorite == null)
            return Result.Failure(Error.NotFound($"Favorito não encontrado para o usuário com ID '{userId}' e imóvel com ID '{realEstateId}'."));

        _dbSet.Remove(favorite);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<Favorites>> GetByUserAndRealEstateAsync(int userId, int realEstateId)
    {
        var favorite = await _dbSet
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RealEstateId == realEstateId);

        if (favorite == null)
            return Result<Favorites>.Failure(Error.NotFound($"Favorito não encontrado para o usuário com ID '{userId}' e imóvel com ID '{realEstateId}'."));

        return Result<Favorites>.Success(favorite);
    }
}
