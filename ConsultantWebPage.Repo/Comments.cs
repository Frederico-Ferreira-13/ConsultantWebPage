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
        
    }

    public async Task<Result<IEnumerable<Comments>>> GetCommentsByPostIdAsync(int postId)
    {
        
    }

    public async Task<Result<IEnumerable<Comments>>> GetPendingCommentsAsync()
    {
        
    }
}
