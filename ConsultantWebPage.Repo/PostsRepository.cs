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

public class PostsRepository : GenericRepository<Posts>, IPostsRepository
{
    public PostsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<Posts>>> GetLatestPostsAsync(int count)
    {
        var posts = await _dbSet
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync<Posts>();

        return Result<IEnumerable<Posts>>.Success(posts);
    }

    public async Task<Result<IEnumerable<Posts>>> GetPostsByCategoryIdAsync(int categoryId)
    {
        var posts = await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync<Posts>();

        return Result<IEnumerable<Posts>>.Success(posts);
    }

    public async Task<Result<IEnumerable<Posts>>> GetPostsByAuthorIdAsync(int authorId)
    {
        var posts = await _dbSet
            .Where(p => p.AuthorId == authorId)
            .ToListAsync<Posts>();

        return Result<IEnumerable<Posts>>.Success(posts);
    }    

    public async Task<Result<IEnumerable<Posts>>> GetPostsByStatusAsync(string status)
    {
        var posts = await _dbSet
            .Where(p => p.Status.ToLower() == status.ToLower())
            .ToListAsync<Posts>();

        return Result<IEnumerable<Posts>>.Success(posts);   
    }

    public async Task<Result> IncrementViewCountAsync(int postId)
    {
        var post = await _dbSet.FindAsync(postId);
        if (post == null)
        {        
            return Result.Failure(Error.NotFound("O post não foi encontrado."));
        }

        post.IncrementView();
        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
