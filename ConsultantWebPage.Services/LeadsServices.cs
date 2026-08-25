using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    
    public async Task<Result<Leads>> GetLeadByIdAsync(int leadId)
    {
        var leadResult = await _unitOfWork.Leads.GetByIdAsync(leadId);
        if (leadResult.IsFailure || leadResult.Value == null)
        {
            return Result<Leads>.Failure(leadResult.Error);
        }

        return Result<Leads>.Success(leadResult.Value);
        
    }

    public async Task<Result<IEnumerable<Leads>>> GetLeadsByConsultantAsync(int consultantId)
    {
        var consultantResult = await _unitOfWork.Leads.GetByConsultantIdAsync(consultantId);
        if (consultantResult.IsFailure || consultantResult.Value == null) 
        {
            return Result<IEnumerable<Leads>>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Consultor com ID {consultantId} não encontrado."));
        }

        return Result<IEnumerable<Leads>>.Success(consultantResult.Value);
        
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

    public async Task<Result> UpdateLeadStatusAsync(int leadId, string newStatus)
    {
        var leadResult = await _unitOfWork.Leads.GetByIdAsync(leadId);
        if (leadResult.IsFailure || leadResult.Value == null)
        {
            return Error.NotFound("LEAD_NOT_FOUND", $"A lead com o ID {leadId} não existe.");
        }

        var lead = leadResult.Value;

        try
        {
            lead.UpdateStatus(newStatus);

            var repoResult = await _unitOfWork.Leads.UpdateAsync(lead);
            if (repoResult.IsFailure)
            {
                return Error.InternalServer("Erro ao atualizar o estado da lead no repositório.");
            }

            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Error.Validation(ex.Message);
        }
    }

    public async Task<Result> AssignLeadAsync(int leadId, int consultantId)
    {
        var leadResult = await _unitOfWork.Leads.GetByIdAsync(leadId);
        if (leadResult.IsFailure || leadResult.Value == null)
        {
            return Error.NotFound("LEAD_NOT_FOUND", $"A lead com o ID {leadId} não existe.");
        }

        var lead = leadResult.Value;

        try
        {            
            lead.AssignToConsultant(consultantId);
            
            var repoResult = await _unitOfWork.Leads.UpdateAsync(lead);
            if (repoResult.IsFailure)
            {
                return Error.InternalServer("Erro ao atribuir o consultor à lead no repositório.");
            }

            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Error.Validation(ex.Message);
        }
    }
}