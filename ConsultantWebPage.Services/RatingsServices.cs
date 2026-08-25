using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Services;

public class RatingsServices : IRatingsServices
{
    private readonly IUnitOfWork _unitOfWork;

    public RatingsServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Ratings>> GetRatingByIdAsync(int ratingId)
    {
        if (ratingId <= 0)
            return Result<Ratings>.Failure(
                Error.Validation(
                    "ID da avaliação inválido.",
                    new Dictionary<string, string[]> { { nameof(ratingId), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.Rating.GetByIdAsync(ratingId);
        if (!result.IsSuccessful)
            return Result<Ratings>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Avaliação com ID {ratingId} não encontrada."));

        return Result<Ratings>.Success(result.Value!);
    }

    public async Task<Result<Ratings>> GetUserRatingForPropertyAsync(int realEstateId, int userId)
    {
        if (realEstateId <= 0)
            return Result<Ratings>.Failure(
                Error.Validation(
                    "O ID da propriedade é inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "ID da propriedade deve ser positivo." } } }));

        if (userId <= 0)
            return Result<Ratings>.Failure(
                Error.Validation(
                    "O ID do utilizador é inválido.",
                    new Dictionary<string, string[]> { { nameof(userId), new[] { "ID do utilizador deve ser positivo." } } }));

        var result = await _unitOfWork.Rating.GetByUserAndRealEstateAsync(realEstateId, userId);
        if (!result.IsSuccessful)
            return Result<Ratings>.Failure(
                Error.NotFound(
                    ErrorCodes.NotFound,
                    $"Avaliação não encontrada para o Utilizador {userId} na Propriedade {realEstateId}."));

        return Result<Ratings>.Success(result.Value!);
    }

    public async Task<Result<IEnumerable<Ratings>>> GetRatingsByPropertyIdAsync(int realEstateId)
    {
        if (realEstateId <= 0)
            return Result<IEnumerable<Ratings>>.Failure(
                Error.Validation(
                    "O ID da propriedade é inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "ID da propriedade deve ser positivo." } } }));

        var result = await _unitOfWork.Rating.GetByRealEstateIdAsync(realEstateId);
        if (!result.IsSuccessful)
            return Result<IEnumerable<Ratings>>.Failure(result.Error);

        return Result<IEnumerable<Ratings>>.Success(result.Value!);
    }

    public async Task<Result> AddOrUpdateRatingAsync(int userId, int realEstateId, int stars)
    {
        if (userId <= 0)
            return Result.Failure(
                Error.Validation(
                    "O ID do utilizador é inválido.",
                    new Dictionary<string, string[]> { { nameof(userId), new[] { "Deve ser maior que zero" } } }));

        if (realEstateId <= 0)
            return Result.Failure(
                Error.Validation(
                    "O ID da propriedade é inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        if (stars < 1 || stars > 5)
            return Result.Failure(
                Error.Validation(
                    "A avaliação deve ser entre 1 e 5 estrelas.",
                    new Dictionary<string, string[]> { { nameof(stars), new[] { "Valor entre 1 e 5" } } }));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var result = await _unitOfWork.Rating.UpsertRatingAsync(userId, realEstateId, stars);
            if (!result.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(result.Message ?? "Erro ao registar a avaliação."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Avaliação registada com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao registar a avaliação: {ex.Message}"));
        }
    }

    public async Task<Result<double>> GetAverageForPropertyAsync(int realEstateId)
    {
        if (realEstateId <= 0)
            return Result<double>.Failure(
                Error.Validation(
                    "O ID da propriedade é inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        return await _unitOfWork.Rating.GetAverageRatingAsync(realEstateId);
    }

    public async Task<Result<double>> GetAverageRatingForConsultantAsync(int consultantId)
    {
        if (consultantId <= 0)
            return Result<double>.Failure(
                Error.Validation(
                    "O ID do consultor é inválido.",
                    new Dictionary<string, string[]> { { nameof(consultantId), new[] { "Deve ser maior que zero" } } }));

        // CORRIGIDO: chamava GetAverageRatingAsync(consultantId), que filtra por RealEstateId,
        // não por ConsultantId — devolvia sempre valores sem sentido. Agora usa o método
        // dedicado do repositório, que faz join correto com RealEstate.ConsultantId.
        return await _unitOfWork.Rating.GetAverageRatingForConsultantAsync(consultantId);
    }

    public async Task<Result> DeleteRatingAsync(int ratingId)
    {
        if (ratingId <= 0)
            return Result.Failure(
                Error.Validation(
                    "ID da avaliação inválido.",
                    new Dictionary<string, string[]> { { nameof(ratingId), new[] { "Deve ser maior que zero" } } }));

        var exists = await _unitOfWork.Rating.ExistsAsync(ratingId);
        if (!exists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Avaliação com ID {ratingId} não encontrada."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var deleteResult = await _unitOfWork.Rating.DeleteAsync(ratingId);
            if (!deleteResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(deleteResult.Message ?? "Erro ao eliminar a avaliação."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Avaliação eliminada com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao eliminar a avaliação: {ex.Message}"));
        }
    }
}