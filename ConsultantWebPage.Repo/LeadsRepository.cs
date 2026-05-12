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

public class LeadsRepository : GenericRepository<Leads>, ILeadsRepository
{
    public LeadsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<Leads>>> GetByConsultantIdAsync(int consultantId)
    {
        var leads = await _dbSet
            .Where(l => l.ConsultantId == consultantId)
            .ToListAsync();

        return Result<IEnumerable<Leads>>.Success(leads);
    }

    public async Task<Result<IEnumerable<Leads>>> GetByRealEstateIdAsync(int realEstateId)
    {
        var leads = await _dbSet
            .Where(l => l.RealEstateId == realEstateId)
            .ToListAsync();

            return Result<IEnumerable<Leads>>.Success(leads);
    }

    public async Task<int> GetCountByRealEstateIdAsync(int realEstateId)
    {
        return await _dbSet
            .Where(l => l.RealEstateId == realEstateId)
            .CountAsync();            
    }

    public async Task<Result<IEnumerable<Leads>>> GetByStatusAsync(string status)
    {
        var leads = await _dbSet
            .Where(l => l.Status.ToLower() == status.ToLower())
            .ToListAsync();

            return Result<IEnumerable<Leads>>.Success(leads);
    }
    public async Task<bool> HasRecentLeadFromEmailAsync(string email, int realEstateId)
    {
       var lead = await _dbSet
        .Where(l => l.Contact.Email.ToLower() == email.ToLower() && l.RealEstateId == realEstateId)
        .OrderByDescending(l => l.CreatedAt)
        .FirstOrDefaultAsync<Leads>();

        return lead != null;
    }
}
