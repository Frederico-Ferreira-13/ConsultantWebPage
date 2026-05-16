using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class FavoritesServices : IFavoritesServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsersServices _usersServices;

    public FavoritesServices(IUnitOfWork unitOfWork, IUsersServices usersServices)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _usersServices = usersServices ?? throw new ArgumentNullException(nameof(usersServices));
    }

    public async Task<Result<int>> GetCurrentUserIdAsync()
    {
        var userIdResult = await _usersServices.GetCurrentUserIdAsync();
        if (!userIdResult.IsSuccessful || userIdResult.Value <= 0)
        {
            return Result<int>.Failure(
                Error.Unauthorized(
                    userIdResult.ErrorCode ?? ErrorCodes.AuthUnauthorized,
                    userIdResult.Message ?? "Utilizador não autenticado."));
        }

        return Result<int>.Success(userIdResult.Value);
    }

    public async Task<Result> AddFavoriteAsync(Favorites favorite)
    {
        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
        {
            return Result.Failure(currentUserIdResult.Error);
        }

        int currentUserId = currentUserIdResult.Value;

        if (favorite.UserId != currentUserId)
        {
            return Result.Failure(
                Error.Forbidden(
                    ErrorCodes.AuthForbidden,
                    "Não pode adicionar favorito para outro utilizador."));
        }

        var realEstateResult = await _unitOfWork.RealEstates.GetByIdAsync(favorite.RealEstateId);
        if (realEstateResult.IsFailure || realEstateResult.Value == null || !realEstateResult.Value.GetIsActive())
        {
            return Result.Failure(
                Error.NotFound(
                    ErrorCodes.NotFound,
                    $"Receita com ID {favorite.RealEstateId} não encontrada ou inativa."));
        }

        var allFavoritesResult = await _unitOfWork.Favorites.GetAllAsync();
        if (allFavoritesResult.IsSuccessful && allFavoritesResult.Value != null)
        {
            bool exists = allFavoritesResult.Value.Any(f => f.UserId == currentUserId && f.RealEstateId == favorite.RealEstateId);
            if (exists)
            {
                return Result.Success("O imóvel já se encontra nos favoritos.");
            }
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var favoriteToAdd = new Favorites(currentUserId, favorite.RealEstateId);
            await _unitOfWork.Favorites.AddAsync(favoriteToAdd);
            await _unitOfWork.CommitAsync();

            return Result.Success("Favorito adicionado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Erro ao adicionar favorito: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<Favorites>>> GetUserFavoritesAsync(int userId)
    {
        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
        {
            return Result<IEnumerable<Favorites>>.Failure(currentUserIdResult.Error);
        }

        int currentUserId = currentUserIdResult.Value;

        if (userId != currentUserId)
        {
            return Result<IEnumerable<Favorites>>.Failure(
                Error.Forbidden(
                    ErrorCodes.AuthForbidden,
                    "Não pode ver favoritos de outro utilizador."
                )
            );
        }

        var allFavoritesResult = await _unitOfWork.Favorites.GetAllAsync();
        if (allFavoritesResult.IsFailure || allFavoritesResult.Value == null)
        {
            return Result<IEnumerable<Favorites>>.Failure(allFavoritesResult.Error);
        }

        var userFavorites = allFavoritesResult.Value.Where(f => f.UserId == userId);
        return Result<IEnumerable<Favorites>>.Success(userFavorites);
    }

    public async Task<Result> RemoveFavoriteAsync(int realEstateId)
    {
        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
        {
            return Result.Failure(currentUserIdResult.Error);
        }

        int currentUserId = currentUserIdResult.Value;

        var allFavoritesResult = await _unitOfWork.Favorites.GetAllAsync();
        if (allFavoritesResult.IsFailure || allFavoritesResult.Value == null)
        {
            return Result.Success("Favorito já removido ou não encontrado.");
        }

        var favoriteItem = allFavoritesResult.Value.FirstOrDefault(f => f.RealEstateId == realEstateId && f.UserId == currentUserId);
        if (favoriteItem == null)
        {
            return Result.Success("Favorito já removido.");
        }
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await _unitOfWork.Favorites.DeleteAsync(favoriteItem.FavoritesId);
            await _unitOfWork.CommitAsync();
            return Result.Success("Favorito removido com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Erro ao remover favorito: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> ToggleFavoriteAsync(int realEstateId)
    {
        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
        {
            return Result<bool>.Failure(currentUserIdResult.Error);
        }

        int currentUserId = currentUserIdResult.Value;

        var realEstateResult = await _unitOfWork.RealEstates.GetByIdAsync(realEstateId);
        if (realEstateResult.IsFailure || realEstateResult.Value == null || !realEstateResult.Value.GetIsActive())
        {
            return Error.NotFound(
                ErrorCodes.NotFound, 
                $"Imóvel com ID {realEstateId} não encontrado ou inativo.");
        }

        var allFavoritesResult = await _unitOfWork.Favorites.GetAllAsync();
        if (allFavoritesResult.IsFailure || allFavoritesResult.Value == null)
        {
            return Error.InternalServer("Não foi possível carregar a listagem de favoritos.");
        }

        var existingFavorite = allFavoritesResult.Value.FirstOrDefault(f => f.RealEstateId == realEstateId && f.UserId == currentUserId);
        bool isNowFavorite;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (existingFavorite != null)
            {
                await _unitOfWork.Favorites.DeleteAsync(existingFavorite.FavoritesId);
                isNowFavorite = false;
            }
            else
            {
                var newFavorite = new Favorites(currentUserId, realEstateId);
                await _unitOfWork.Favorites.AddAsync(newFavorite);
                isNowFavorite = true;
            }

            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(isNowFavorite);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<bool>.Failure(
                Error.InternalServer($"Erro ao togglear favorito: {ex.Message}"));
        }
    }
}