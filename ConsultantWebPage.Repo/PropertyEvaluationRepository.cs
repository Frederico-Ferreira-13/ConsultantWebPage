using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class PropertyEvaluationRepository : GenericRepository<PropertyEvaluations>, IPropertyEvaluationRepository
{

    public PropertyEvaluationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<PropertyEvaluations>>> GetByConsultantIdAsync(int consultantId)
    {
        var evaluations = await _dbSet
            .Where(e => e.ConsultantId == consultantId)
            .ToListAsync<PropertyEvaluations>();

        return Result<IEnumerable<PropertyEvaluations>>.Success(evaluations);
    }

    public async Task<Result<IEnumerable<PropertyEvaluations>>> GetByStatusAsync(string status)
    {
        var evaluations = await _dbSet
            .Where(e => e.Status.ToLower() == status.ToLower())
            .ToListAsync<PropertyEvaluations>();

        return Result<IEnumerable<PropertyEvaluations>>.Success(evaluations);
    }

    public async Task<Result<IEnumerable<PropertyEvaluations>>> GetByCityAsync(string city)
    {
        var evaluations = await _dbSet
            .Where(e => e.Address.ToLower() == city.ToLower())
            .ToListAsync<PropertyEvaluations>();

        return Result<IEnumerable<PropertyEvaluations>>.Success(evaluations);
    }

    public async Task<bool> HasPendingEvaluationAsync(string email)
    {
        return await _dbSet
            .AnyAsync(e => e.ClientContact.ToLower() == email.ToLower() 
                        && e.Status.ToLower() == "pending");
    }

    public async Task<int> GetPendingCountAsync(int consultantId)
    {
        return await _dbSet
            .CountAsync(e => e.ConsultantId == consultantId 
                          && e.Status.ToLower() == "pending");
    }

    public async Task<Result<IEnumerable<PropertyEvaluations>>> GetPendingEvaluationsAsync(int consultantId)
    {
        var evaluations = await _dbSet
            .Where(e => e.ConsultantId == consultantId 
                     && e.Status.ToLower() == "pending")
            .ToListAsync<PropertyEvaluations>();

        return Result<IEnumerable<PropertyEvaluations>>.Success(evaluations);
    }

    public async Task<Result<PropertyEvaluations>> GetDetailsAsync(int evaluationId)
    {
        var evaluation = await _dbSet
            .FirstOrDefaultAsync(e => e.EvaluationId == evaluationId);

        if (evaluation == null)
            return Result<PropertyEvaluations>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Avaliação com ID {evaluationId} não encontrada."));

        return Result<PropertyEvaluations>.Success(evaluation);
    }

    public async Task<Result> UpdateStatusAsync(int evaluationId, string newStatus)
    {
        var evaluation = await _dbSet
            .FirstOrDefaultAsync(e => e.EvaluationId == evaluationId);

        if (evaluation == null)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Avaliação com ID {evaluationId} não encontrada."));

        evaluation.UpdateStatus(newStatus);
        _dbSet.Update(evaluation);

        return Result.Success();
    }
}