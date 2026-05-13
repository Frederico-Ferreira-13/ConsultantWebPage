using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ILeadsServices
{
    Task<Result<IEnumerable<Leads>>> GetLeadsByConsultantAsync(int consultantId);
    Task<Result> CreateLeadAsync(Leads lead);
}