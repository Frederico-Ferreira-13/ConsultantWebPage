using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ILeadsServices
{
    Task<Result<IEnumerable<Leads>>> GetLeadsByConsultantAsync(int consultantId);
    Task<Result<Leads>> GetLeadByIdAsync(int leadId);
    Task<Result> CreateLeadAsync(Leads lead);
    Task<Result> UpdateLeadStatusAsync(int leadId, string newStatus);
    Task<Result> AssignLeadAsync(int leadId, int consultantId);
}