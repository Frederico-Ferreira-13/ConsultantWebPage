using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Services; 
using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Services;

public class PostsServices : IPostsServices 
{
    private readonly IUnitOfWork _unitOfWork;

    public PostsServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<Posts>>> GetLatestBlogPostsAsync(int count)
    {
        return await _unitOfWork.Posts.GetLatestPostsAsync(count);
    }

    public async Task<Result> IncrementPostViewAsync(int postId)
    {
        var result = await _unitOfWork.Posts.IncrementViewCountAsync(postId);
        // Não esquecer do commit se o repositório não o fizer automaticamente
        await _unitOfWork.CommitAsync();
        return result;
    }
}