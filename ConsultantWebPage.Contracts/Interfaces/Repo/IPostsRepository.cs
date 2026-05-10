using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IPostsRepository : IGenericRepository<Posts>
{
    Task<Result<IEnumerable<Posts>>> GetLatestPostsAsync(int count);
    Task<Result<IEnumerable<Posts>>> GetPostsByCategoryIdAsync(int categoryId);
    Task<Result<IEnumerable<Posts>>> GetPostsByAuthorIdAsync(int authorId);
    Task<Result<IEnumerable<Posts>>> GetPostsByStatusAsync(string status);
    Task<Result> IncrementViewCountAsync(int postId);
}
