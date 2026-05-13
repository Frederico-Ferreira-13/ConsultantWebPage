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
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> RequestEvaluationAsync(PropertyEvaluations evaluation)
    {
        // Regra: Não permitir múltiplos pedidos pendentes do mesmo email
        var hasPending = await _unitOfWork.PropertiesEvaluations.HasPendingEvaluationAsync(evaluation.ClientContact);
        
        if (hasPending)
            return Result.Failure(Error.Conflict("Já existe um pedido de avaliação pendente para este contacto."));

        await _unitOfWork.PropertiesEvaluations.AddAsync(evaluation);
        await _unitOfWork.CommitAsync();
        
        return Result.Success();
    }
}