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

public class AboutMeRepository : GenericRepository<AboutMe>, IAboutMeRepository
{
    public AboutMeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<AboutMe>> GetByUserIdAsync(int userId)
    {        
        var result = await _context.AboutMe
            .Where(a => a.UserId == userId)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (result == null)
            return Result<AboutMe>.Failure(Error.NotFound("Ainda não foi criada uma biografia para este consultor."));

        return Result<AboutMe>.Success(result);
    }

    public async Task<Result<IEnumerable<AboutMe>>> GetAllByUserIdAsync(int userId)
    {
        var list = await _context.AboutMe
            .Where(a => a.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

        return Result<IEnumerable<AboutMe>>.Success(list);
    }
    
    public async Task<Result> DeactivateAllForUserAsync(int userId)
    {
        var list = await _context.AboutMe
            .Where(a => a.UserId == userId)
            .ToListAsync();

        if (!list.Any())
            return Result.Success("Nenhum registo para desativar (idempotente).");

        _context.AboutMe.RemoveRange(list);
        await _context.SaveChangesAsync();

        return Result.Success("Registos removidos com sucesso.");
    }
}