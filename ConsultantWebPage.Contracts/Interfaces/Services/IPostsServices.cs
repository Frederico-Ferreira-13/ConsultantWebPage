using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IPostsServices
{
    Task<Result<IEnumerable<Posts>>> GetLatestBlogPostsAsync(int count);
    Task<Result> IncrementPostViewAsync(int postId);
}