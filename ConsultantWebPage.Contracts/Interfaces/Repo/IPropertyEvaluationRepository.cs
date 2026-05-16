using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IPropertyEvaluationRepository : IGenericRepository<PropertyEvaluations>
{
    Task<Result<IEnumerable<PropertyEvaluations>>> GetByConsultantIdAsync(int consultantId);
    Task<Result<IEnumerable<PropertyEvaluations>>> GetByStatusAsync(string status);
    Task<Result<IEnumerable<PropertyEvaluations>>> GetByCityAsync(string city);
    Task<bool> HasPendingEvaluationAsync(string email);
    Task<int> GetPendingCountAsync(int consultantId);

    Task<Result<IEnumerable<PropertyEvaluations>>> GetPendingEvaluationsAsync(int consultantId);
    Task<Result<PropertyEvaluations>> GetDetailsAsync(int evaluationId);
    Task<Result> UpdateStatusAsync(int evaluationId, string newStatus);
}
