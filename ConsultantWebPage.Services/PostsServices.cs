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
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<IEnumerable<Posts>>> GetAllPostsAsync()
    {
        return await _unitOfWork.Posts.GetAllAsync();
    }

    public async Task<Result<IEnumerable<Posts>>> GetLatestBlogPostsAsync(int count)
    {
        if(count <= 0)
        {
            return Result<IEnumerable<Posts>>.Failure(
                Error.Validation("O número de posts deve ser maior que zero."));
        }

        return await _unitOfWork.Posts.GetLatestPostsAsync(count);
    }

    public async Task<Result<Posts>> GetPostBySlugAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return Result<Posts>.Failure(Error.Validation("O slug não pode estar vazio."));
 
        var allPostsResult = await _unitOfWork.Posts.GetAllAsync();
        if (!allPostsResult.IsSuccessful)
            return Result<Posts>.Failure(allPostsResult.Error);
 
        var normalizedSlug = slug.ToLower().Trim().Replace(" ", "-");
        var post = allPostsResult.Value!.FirstOrDefault(p => p.Slug == normalizedSlug);
 
        if (post == null)
            return Result<Posts>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Post com o slug '{slug}' não foi encontrado."));
 
        return Result<Posts>.Success(post);
    }

    public async Task<Result<IEnumerable<Posts>>> GetPostsByCategoryAsync(int categoryId)
    {
        if (categoryId <= 0)
            return Result<IEnumerable<Posts>>.Failure(
                Error.Validation("O ID da categoria deve ser maior que zero."));
 
        return await _unitOfWork.Posts.GetPostsByCategoryIdAsync(categoryId);
    }

    public async Task<Result<IEnumerable<Posts>>> SearchPostsAsync(string searchTerm)
    {
        var allPostsResult = await _unitOfWork.Posts.GetAllAsync();
        if (!allPostsResult.IsSuccessful)
            return Result<IEnumerable<Posts>>.Failure(allPostsResult.Error);

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Result<IEnumerable<Posts>>.Success(allPostsResult.Value);
        }

        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<IEnumerable<Posts>>.Success(allPostsResult.Value!);
 
        var filtered = allPostsResult.Value!.Where(p =>
            p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            p.Summary.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            p.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
 
        return Result<IEnumerable<Posts>>.Success(filtered);
    }

    public async Task<Result<Posts>> CreatePostAsync(Posts newPost)
    {
        if (newPost == null)
        {
            return Result<Posts>.Failure(Error.Validation("O post não pode ser nulo."));
        }

        if (string.IsNullOrWhiteSpace(newPost.Title))
            return Result<Posts>.Failure(Error.Validation(
                "O título é obrigatório.",
                new Dictionary<string, string[]> { { nameof(newPost.Title), new[] { "Campo obrigatório" } } }));

        if (string.IsNullOrWhiteSpace(newPost.Slug))
            return Result<Posts>.Failure(Error.Validation(
                "O slug é obrigatório.",
                new Dictionary<string, string[]> { { nameof(newPost.Slug), new[] { "Campo obrigatório" } } }));
 
        if (string.IsNullOrWhiteSpace(newPost.Content))
            return Result<Posts>.Failure(Error.Validation(
                "O conteúdo é obrigatório.",
                new Dictionary<string, string[]> { { nameof(newPost.Content), new[] { "Campo obrigatório" } } }));
 
        await _unitOfWork.BeginTransactionAsync();
 
        try
        {
            // AddAsync retorna Result (sem genérico) — não tem .Error, usa .Message
            var addResult = await _unitOfWork.Posts.AddAsync(newPost);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result<Posts>.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao criar o post."));
            }
 
            await _unitOfWork.CommitAsync();
            return Result<Posts>.Success(newPost);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<Posts>.Failure(
                Error.InternalServer($"Ocorreu um erro ao criar o post: {ex.Message}"));
        }
    }
    
    public async Task<Result<Posts>> UpdatePostAsync(int postId, Posts updatePost)
    {
        if (updatePost == null)
            return Result<Posts>.Failure(Error.Validation("Os dados de atualização não podem ser nulos."));
 
        if (string.IsNullOrWhiteSpace(updatePost.Title))
            return Result<Posts>.Failure(Error.Validation(
                "O título é obrigatório.",
                new Dictionary<string, string[]> { { nameof(updatePost.Title), new[] { "Campo obrigatório" } } }));
 
        if (string.IsNullOrWhiteSpace(updatePost.Slug))
            return Result<Posts>.Failure(Error.Validation(
                "O slug é obrigatório.",
                new Dictionary<string, string[]> { { nameof(updatePost.Slug), new[] { "Campo obrigatório" } } }));
 
        if (string.IsNullOrWhiteSpace(updatePost.Content))
            return Result<Posts>.Failure(Error.Validation(
                "O conteúdo é obrigatório.",
                new Dictionary<string, string[]> { { nameof(updatePost.Content), new[] { "Campo obrigatório" } } }));
 
        var postResult = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (!postResult.IsSuccessful)
            return Result<Posts>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"O post com ID {postId} não existe."));
 
        var existingPost = postResult.Value!;
 
        await _unitOfWork.BeginTransactionAsync();
 
        try
        {
            existingPost.UpdatePost(
                updatePost.Title,
                updatePost.Slug,
                updatePost.Content,
                updatePost.Summary,
                updatePost.ImageUrl,
                updatePost.IsPublished
            );
 
            // UpdateAsync retorna Result (sem genérico) — não tem .Error, usa .Message
            var updateResult = await _unitOfWork.Posts.UpdateAsync(existingPost);
            if (!updateResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result<Posts>.Failure(
                    Error.InternalServer(updateResult.Message ?? "Erro ao atualizar o post."));
            }
 
            await _unitOfWork.CommitAsync();
            return Result<Posts>.Success(existingPost);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<Posts>.Failure(
                Error.InternalServer($"Ocorreu um erro ao atualizar o post: {ex.Message}"));
        }
    }

    public async Task<Result> DeletePostAsync(int postId)
    {
        if (postId <= 0)
            return Result.Failure(Error.Validation("O ID do post deve ser maior que zero."));
 
        var exists = await _unitOfWork.Posts.ExistsAsync(postId);
        if (!exists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"O post com ID {postId} não existe."));
 
        await _unitOfWork.BeginTransactionAsync();
 
        try
        {
            var deleteResult = await _unitOfWork.Posts.DeleteAsync(postId);
            if (!deleteResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(deleteResult.Message ?? "Erro ao eliminar o post."));
            }
 
            await _unitOfWork.CommitAsync();
            return Result.Success("Post eliminado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao eliminar o post: {ex.Message}"));
        }
    }

    public async Task<Result> IncrementPostViewAsync(int postId)
    {
        if (postId <= 0)
            return Result.Failure(Error.Validation("O ID do post deve ser maior que zero."));
 
        var result = await _unitOfWork.Posts.IncrementViewCountAsync(postId);
        if (!result.IsSuccessful)
            return result;
 
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}