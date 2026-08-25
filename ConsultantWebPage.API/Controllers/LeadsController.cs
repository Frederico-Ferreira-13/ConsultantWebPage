using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly ILeadsServices _leadsService;

    public LeadsController(ILeadsServices leadsService)
    {
        _leadsService = leadsService;
    }
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpGet("consultant/{consultantId:int}")]
    public async Task<IActionResult> GetByConsultant(int consultantId)
    {
        var result = await _leadsService.GetLeadsByConsultantAsync(consultantId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _leadsService.GetLeadByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeadRequest request)
    {
        var contactResult = ContactInfo.Create(request.Email, request.Phone);
        if (!contactResult.IsSuccessful)
            return BadRequest(contactResult.Message);

        var leadResult = Leads.Create(
            request.RealEstateId,
            request.ConsultantId,
            request.ClientName,
            contactResult.Value!,
            request.Message
        );

        if (!leadResult.IsSuccessful)
            return BadRequest(leadResult.Message);

        var result = await _leadsService.CreateLeadAsync(leadResult.Value!);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = leadResult.Value!.InquiryId }, leadResult.Value);
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] LeadUpdateStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NewStatus))
            return BadRequest("O novo status não pode estar vazio.");

        var result = await _leadsService.UpdateLeadStatusAsync(id, request.NewStatus);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPatch("{id:int}/assign")]
    public async Task<IActionResult> AssignConsultant(int id, [FromBody] LeadAssignRequest request)
    {
        if (request.ConsultantId <= 0)
            return BadRequest("ID de consultor inválido.");

        var result = await _leadsService.AssignLeadAsync(id, request.ConsultantId);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }
}

public record CreateLeadRequest(
    int? RealEstateId,
    int? ConsultantId,
    string ClientName,
    string Email,
    string Phone,
    string Message
);

public record LeadUpdateStatusRequest(string NewStatus);
public record LeadAssignRequest(int ConsultantId);