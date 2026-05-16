using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Services;

public class PropertyEvaluationServices : IPropertyEvaluationServices
{
    private readonly IUnitOfWork _unitOfWork;

    public PropertyEvaluationServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> RequestEvaluationAsync(PropertyEvaluations evaluation)
    {
        if (evaluation == null)
            return Result.Failure(Error.Validation("O pedido de avaliação não pode ser nulo."));

        if (string.IsNullOrWhiteSpace(evaluation.ClientContact))
            return Result.Failure(Error.Validation(
                "O contacto do cliente é obrigatório.",
                new Dictionary<string, string[]> { { nameof(evaluation.ClientContact), new[] { "Campo obrigatório" } } }));

        var hasPending = await _unitOfWork.PropertiesEvaluations.HasPendingEvaluationAsync(evaluation.ClientContact);
        if (hasPending)
            return Result.Failure(
                Error.Conflict(ErrorCodes.AlreadyExists, "Já existe um pedido de avaliação pendente para este contacto."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var addResult = await _unitOfWork.PropertiesEvaluations.AddAsync(evaluation);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao registar o pedido de avaliação."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Pedido de avaliação registado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao registar o pedido: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<PropertyEvaluations>>> GetPendingEvaluationsAsync(int consultantId)
    {
        if (consultantId <= 0)
            return Result<IEnumerable<PropertyEvaluations>>.Failure(
                Error.Validation(
                    "ID do consultor inválido.",
                    new Dictionary<string, string[]> { { nameof(consultantId), new[] { "Deve ser maior que zero" } } }));

        return await _unitOfWork.PropertiesEvaluations.GetPendingEvaluationsAsync(consultantId);
    }

    public async Task<Result<PropertyEvaluations>> GetEvaluationDetailsAsync(int evaluationId)
    {
        if (evaluationId <= 0)
            return Result<PropertyEvaluations>.Failure(
                Error.Validation(
                    "ID da avaliação inválido.",
                    new Dictionary<string, string[]> { { nameof(evaluationId), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.PropertiesEvaluations.GetDetailsAsync(evaluationId);
        if (!result.IsSuccessful)
            return Result<PropertyEvaluations>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Avaliação com ID {evaluationId} não encontrada."));

        return Result<PropertyEvaluations>.Success(result.Value!);
    }

    public async Task<Result> UpdateStatusAsync(int evaluationId, string newStatus)
    {
        if (evaluationId <= 0)
            return Result.Failure(
                Error.Validation(
                    "ID da avaliação inválido.",
                    new Dictionary<string, string[]> { { nameof(evaluationId), new[] { "Deve ser maior que zero" } } }));

        if (string.IsNullOrWhiteSpace(newStatus))
            return Result.Failure(
                Error.Validation(
                    "O novo estado não pode estar vazio.",
                    new Dictionary<string, string[]> { { nameof(newStatus), new[] { "Campo obrigatório" } } }));

        var exists = await _unitOfWork.PropertiesEvaluations.ExistsAsync(evaluationId);
        if (!exists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Avaliação com ID {evaluationId} não encontrada."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Usa UpdateStatusAsync(id, status) do repositório — não o UpdateAsync genérico
            var updateResult = await _unitOfWork.PropertiesEvaluations.UpdateStatusAsync(evaluationId, newStatus);
            if (!updateResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(updateResult.Message ?? "Erro ao atualizar o estado da avaliação."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Estado atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao atualizar o estado: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteEvaluationAsync(int evaluationId)
    {
        if (evaluationId <= 0)
            return Result.Failure(
                Error.Validation(
                    "ID da avaliação inválido.",
                    new Dictionary<string, string[]> { { nameof(evaluationId), new[] { "Deve ser maior que zero" } } }));

        var exists = await _unitOfWork.PropertiesEvaluations.ExistsAsync(evaluationId);
        if (!exists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Avaliação com ID {evaluationId} não encontrada."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var deleteResult = await _unitOfWork.PropertiesEvaluations.DeleteAsync(evaluationId);
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