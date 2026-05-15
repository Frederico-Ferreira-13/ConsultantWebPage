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

    public async Task<Result<Posts>> GetPostBySlugAsync(string slug)
    {
        return await _unitOfWork.Posts.GetBySlugAsync(slug);
    }

    public async Task<Result<IEnumerable<Posts>>> GetPostsByCategoryAsync(int categoryId)
    {
        return await _unitOfWork.Posts.GetByCategoryAsync(categoryId);
    }

    public async Task<Result<IEnumerable<Posts>>> SearchPostsAsync(string searchTerm)
    {
        return await _unitOfWork.Posts.SearchAsync(searchTerm);
    }

    public async Task<Result> IncrementPostViewAsync(int postId)
    {
        var result = await _unitOfWork.Posts.IncrementViewCountAsync(postId);
        // Não esquecer do commit se o repositório não o fizer automaticamente
        await _unitOfWork.CommitAsync();
        return result;
    }
    

    public async Task<Result<Posts>> CreatePostAsync(Posts newPost)
    {
        return await _unitOfWork.Posts.CreateAsync(newPost);
    }

    public async Task<Result<Posts>> UpdatePostAsync(Posts updatePost)
    {
        return await _unitOfWork.Posts.UpdateAsync(updatePost);
    }

    public async Task<Result> DeletePostAsync(int postId)
    {
        return await _unitOfWork.Posts.DeleteAsync(postId);
    }

    public async Task<Result<IEnumerable<Posts>>> GetAllPostsAsync()
    {
        return await _unitOfWork.Posts.GetAllAsync();
    }

    public async Task<Result> IncrementPostViewAsync(int postId)
    {
        var result = await _unitOfWork.Posts.IncrementViewCountAsync(postId);
        // Não esquecer do commit se o repositório não o fizer automaticamente
        await _unitOfWork.CommitAsync();
        return result;
    }
}