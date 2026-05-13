using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IPropertyEvaluationServices
{
    Task<Result> RequestEvaluationAsync(PropertyEvaluations evaluation);
}