using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class RatingsRepository : GenericRepository<Ratings>, IRatingsRepository
{
    public RatingsRepository(ApplicationDbContext context) : base(context)
    {
    }   

    public async Task<Result<Ratings>> GetByUserAndRealEstateAsync(int userId, int realEstateId)
    {
        var rating = await _dbSet
            .Where(r => r.UserId == userId && r.RealEstateId == realEstateId)
            .FirstOrDefaultAsync<Ratings>();        

        if (rating == null) return Result<Ratings>.Failure(Error.NotFound("Avaliação não encontrada."));

        return Result<Ratings>.Success(rating);
        
    }

    public async Task<Result<IEnumerable<Ratings>>> GetByRealEstateIdAsync(int realEstateId)
    {
        var ratings = await _dbSet
            .Where(r => r.RealEstateId == realEstateId)
            .ToListAsync<Ratings>();

        return Result<IEnumerable<Ratings>>.Success(ratings);
    }

    public async Task<Result<double>> GetAverageRatingAsync(int realEstateId)
    {
        var averageRating = await _dbSet
            .Where(r => r.RealEstateId == realEstateId)
            .AverageAsync(r => (double?)r.RatingValue!.Value) ?? 0;

        return Result<double>.Success(averageRating);
    }
    

    public async Task<bool> HasUserRatedAsync(int userId, int realEstateId)
    {
        var hasRated = await _dbSet
            .AnyAsync(r => r.UserId == userId && r.RealEstateId == realEstateId);

            return hasRated;
    }

    public async Task<Result> UpsertRatingAsync(int userId, int realEstateId, int stars)
    {
        var existingRating = await _dbSet
            .FirstOrDefaultAsync<Ratings>(r => r.UserId == userId && r.RealEstateId == realEstateId);

        if (existingRating != null)
        {
            existingRating.UpdateRating(stars);
            _dbSet.Update(existingRating);
        }
        else
        {
            var newRating = new Ratings(realEstateId, userId, stars);
            await _dbSet.AddAsync(newRating);
        }

        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
