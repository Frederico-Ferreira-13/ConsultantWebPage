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

public class CommentsRepository  : GenericRepository<Comments>, ICommentsRepository
{
    public CommentsRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<Result<IEnumerable<Comments>>> GetCommentsByRealEstateIdAsync(int realEstateId)
    {
        var comments = await _dbSet
            .Where(c => c.RealEstateId == realEstateId && !c.IsDeleted)
            .ToListAsync<Comments>();

        return Result<IEnumerable<Comments>>.Success(comments);
    }    

    public async Task<Result<IEnumerable<Comments>>> GetPendingCommentsAsync()
    {
        var comments = await _dbSet
            .Where(c => c.IsApproved == false && !c.IsDeleted)
            .ToListAsync<Comments>();

        return Result<IEnumerable<Comments>>.Success(comments);
    }
}
