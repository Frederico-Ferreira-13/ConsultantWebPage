using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IPostsServices
{
    Task<Result<IEnumerable<Posts>>> GetLatestBlogPostsAsync(int count);
    Task<Result<Posts>> GetPostBySlugAsync(string slug);
    Task<Result<IEnumerable<Posts>>> GetPostsByCategoryAsync(int categoryId);
    Task<Result<IEnumerable<Posts>>> SearchPostsAsync(string searchTerm);

    Task<Result> IncrementPostViewAsync(int postId);

    Task<Result<Posts>> CreatePostAsync(Posts newPost);
    Task<Result<Posts>> UpdatePostAsync(Posts updatePost);
    Task<Result> DeletePostAsync(int postId);
    Task<Result<IEnumerable<Posts>>> GetAllPostsAsync();
}