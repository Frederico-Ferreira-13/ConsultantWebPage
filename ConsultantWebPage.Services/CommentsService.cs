using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class CommentsService : ICommentsServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsersServices _usersService;

    public CommentsService(IUnitOfWork unitOfWork, IUsersServices usersService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<Result<Comments>> GetCommentsByIdAsync(int id)
    {
        if (id <= 0)
            return Result<Comments>.Failure(
                Error.Validation("ID do comentário inválido.",
                    new Dictionary<string, string[]> { { nameof(id), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.Comments.GetByIdAsync(id);
        if (!result.IsSuccessful)
            return Result<Comments>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Comentário com ID {id} não encontrado."));

        var comment = result.Value!;

        if (comment.IsDeleted)
            return Result<Comments>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Comentário com ID {id} não encontrado."));

        return Result<Comments>.Success(comment);
    }

    public async Task<Result<IEnumerable<Comments>>> GetCommentsByRealEstateIdAsync(int realEstateId)
    {
        if (realEstateId <= 0)
            return Result<IEnumerable<Comments>>.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.Comments.GetCommentsByRealEstateIdAsync(realEstateId);
        if (!result.IsSuccessful)
            return Result<IEnumerable<Comments>>.Failure(result.Error);

        // Filtra comentários eliminados
        var visible = result.Value!.Where(c => !c.IsDeleted);
        return Result<IEnumerable<Comments>>.Success(visible);
    }

    public async Task<Result<Comments>> CreateCommentAsync(Comments newComment)
    {
        if (newComment == null)
            return Result<Comments>.Failure(Error.Validation("O comentário não pode ser nulo."));

        var currentUserIdResult = await _usersService.GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
            return Result<Comments>.Failure(
                Error.Unauthorized(ErrorCodes.AuthUnauthorized, "Utilizador não autenticado."));

        int currentUserId = currentUserIdResult.Value;

        if (newComment.RealEstateId <= 0)
            return Result<Comments>.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(newComment.RealEstateId), new[] { "Deve ser maior que zero" } } }));

        if (string.IsNullOrWhiteSpace(newComment.CommentText))
            return Result<Comments>.Failure(
                Error.Validation("O texto do comentário é obrigatório.",
                    new Dictionary<string, string[]> { { nameof(newComment.CommentText), new[] { "Campo obrigatório" } } }));

        if (newComment.CommentText.Length > 1000)
            return Result<Comments>.Failure(
                Error.Validation("O comentário não pode exceder 1000 caracteres.",
                    new Dictionary<string, string[]> { { nameof(newComment.CommentText), new[] { "Máximo 1000 caracteres" } } }));

        var propertyExists = await _unitOfWork.RealEstates.ExistsAsync(newComment.RealEstateId);
        if (!propertyExists)
            return Result<Comments>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imóvel com ID {newComment.RealEstateId} não encontrado."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var comment = new Comments(
                realEstateId: newComment.RealEstateId,
                userId: currentUserId,
                commentText: newComment.CommentText,
                parentCommentId: newComment.ParentCommentId
            );

            var addResult = await _unitOfWork.Comments.AddAsync(comment);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result<Comments>.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao criar o comentário."));
            }

            await _unitOfWork.CommitAsync();
            return Result<Comments>.Success(comment);
        }
        catch (ArgumentException ex)
        {
            _unitOfWork.Rollback();
            return Result<Comments>.Failure(
                Error.Validation("Dados inválidos ao criar o comentário.",
                    new Dictionary<string, string[]> { { ex.ParamName ?? "Geral", new[] { ex.Message } } }));
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<Comments>.Failure(
                Error.InternalServer($"Erro inesperado ao criar comentário: {ex.Message}"));
        }
    }

    public async Task<Result> UpdateCommentAsync(int id, Comments updateComment)
    {
        if (id <= 0)
            return Result.Failure(
                Error.Validation("ID do comentário inválido.",
                    new Dictionary<string, string[]> { { nameof(id), new[] { "Deve ser maior que zero" } } }));

        if (string.IsNullOrWhiteSpace(updateComment?.CommentText))
            return Result.Failure(
                Error.Validation("O comentário não pode estar vazio.",
                    new Dictionary<string, string[]> { { nameof(updateComment.CommentText), new[] { "Campo obrigatório" } } }));

        if (updateComment.CommentText.Length > 1000)
            return Result.Failure(
                Error.Validation("O comentário não pode exceder 1000 caracteres.",
                    new Dictionary<string, string[]> { { nameof(updateComment.CommentText), new[] { "Máximo 1000 caracteres" } } }));

        var currentUserIdResult = await _usersService.GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
            return Result.Failure(currentUserIdResult.Error);

        int currentUserId = currentUserIdResult.Value;

        var existingResult = await _unitOfWork.Comments.GetByIdAsync(id);
        if (!existingResult.IsSuccessful)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Comentário com ID {id} não encontrado."));

        var existingComment = existingResult.Value!;

        if (existingComment.IsDeleted)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Comentário com ID {id} não encontrado."));

        if (existingComment.UserId != currentUserId)
            return Result.Failure(
                Error.Forbidden(ErrorCodes.AuthForbidden, "Só o autor pode editar este comentário."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            existingComment.UpdateComment(updateComment.CommentText);
            await _unitOfWork.Comments.UpdateAsync(existingComment);
            await _unitOfWork.CommitAsync();

            return Result.Success("Comentário atualizado com sucesso.");
        }
        catch (InvalidOperationException ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.BusinessRuleViolation(ErrorCodes.BizInvalidOperation, ex.Message));
        }
        catch (ArgumentException ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.Validation("Dados inválidos ao atualizar comentário.",
                    new Dictionary<string, string[]> { { ex.ParamName ?? "Geral", new[] { ex.Message } } }));
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Erro ao atualizar comentário: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteCommentAsync(int id)
    {
        if (id <= 0)
            return Result.Failure(
                Error.Validation("ID do comentário inválido.",
                    new Dictionary<string, string[]> { { nameof(id), new[] { "Deve ser maior que zero" } } }));

        var currentUserIdResult = await _usersService.GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
            return Result.Failure(currentUserIdResult.Error);

        var existingResult = await _unitOfWork.Comments.GetByIdAsync(id);
        if (!existingResult.IsSuccessful || existingResult.Value!.IsDeleted)
            return Result.Success("Comentário já eliminado ou não encontrado (idempotente).");

        var existingComment = existingResult.Value!;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            existingComment.Delete();
            await _unitOfWork.Comments.UpdateAsync(existingComment);
            await _unitOfWork.CommitAsync();

            return Result.Success("Comentário eliminado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Erro ao eliminar comentário: {ex.Message}"));
        }
    }
}