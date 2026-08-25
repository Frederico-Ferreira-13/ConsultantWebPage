using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IPropertyEvaluationServices
{
    Task<Result> RequestEvaluationAsync(PropertyEvaluations evaluation);
    Task<Result<IEnumerable<PropertyEvaluations>>> GetAllEvaluationsAsync();
    Task<Result<IEnumerable<PropertyEvaluations>>> GetPendingEvaluationsAsync(int consultantId);
    Task<Result<PropertyEvaluations>> GetEvaluationDetailsAsync(int evaluationId);
    Task<Result> UpdateStatusAsync(int evaluationId, string newStatus);
    Task<Result> DeleteEvaluationAsync(int evaluationId);
}