using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class LeadsServices : ILeadsServices
{
    private readonly IUnitOfWork _unitOfWork;

    public LeadsServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<Leads>>> GetLeadsByConsultantAsync(int consultantId)
    {
        return await _unitOfWork.Leads.GetByConsultantIdAsync(consultantId);
    }

    public async Task<Result> CreateLeadAsync(Leads lead)
    {
        // Regra de Negócio: Não deixar criar lead se já enviou uma recentemente para o mesmo imóvel
        var hasRecent = await _unitOfWork.Leads.HasRecentLeadFromEmailAsync(lead.Contact.Email, lead.RealEstateId ?? 0);
        
        if (hasRecent)
            return Result.Failure(Error.Validation("Já enviou um contacto recentemente para este imóvel."));

        await _unitOfWork.Leads.AddAsync(lead);
        await _unitOfWork.CommitAsync();
        
        return Result.Success();
    }
}