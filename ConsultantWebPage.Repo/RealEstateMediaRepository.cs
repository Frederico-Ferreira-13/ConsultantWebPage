using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class RealEstateMediaRepository : GenericRepository<RealEstateMedia>, IRealEstateMediaRepository
{
    public RealEstateMediaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<RealEstateMedia>>> GetByRealEstateIdAsync(int realEstateId)
    {
        var media = await _context.RealEstateMedia
            .Where(m => m.RealEstateId == realEstateId)
            .ToListAsync();

        return Result<IEnumerable<RealEstateMedia>>.Success(media);
    }

    public async Task<Result<RealEstateMedia>> GetMainImageByRealEstateIdAsync(int realEstateId)
    {
        var mainImage = await _context.RealEstateMedia
            .Where(m => m.RealEstateId == realEstateId && m.IsMain)
            .FirstOrDefaultAsync();

        if (mainImage == null) return Result<RealEstateMedia>.Failure(Error.NotFound("Imagem principal não encontrada."));

        return Result<RealEstateMedia>.Success(mainImage);
    }

    public async Task<Result> SetMainImageAsync(int realEstateId, int mediaId)
    {
        var newMain = await _context.RealEstateMedia
            .FirstOrDefaultAsync(m => m.RealEstateId == realEstateId && m.MediaId == mediaId);

        if (newMain == null)
        {
        return Result.Failure(Error.NotFound("Mídia não encontrada para o imóvel especificado."));
        }

        var currentMedia = await _context.RealEstateMedia
            .Where(m => m.RealEstateId == realEstateId && m.IsMain)
            .ToListAsync();

        foreach (var item in currentMedia)
        {            
            item.UpdateRealEstateMedia(item.RealEstateId, item.MediaUrl, item.MediaType, false);
        }

        newMain.UpdateRealEstateMedia(newMain.RealEstateId, newMain.MediaUrl, newMain.MediaType, true);
        
        return Result.Success();
    }

    public async Task<Result> UpdateMediaOrderAsync(int realEstateId, List<int> orderedMediaIds)
    {
        var mediaItems = await _context.RealEstateMedia
            .Where(m => m.RealEstateId == realEstateId && orderedMediaIds.Contains(m.MediaId)) // Corrigido: MediaId
            .ToListAsync();

        if (mediaItems.Count != orderedMediaIds.Count)
        {
            return Result.Failure(Error.Validation("Algumas mídias não foram encontradas."));
        }        

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<int> GetMediaCountByRealEstateIdAsync(int realEstateId)
    {
        return await _context.RealEstateMedia
            .Where(m => m.RealEstateId == realEstateId)
            .CountAsync();
    }
}
