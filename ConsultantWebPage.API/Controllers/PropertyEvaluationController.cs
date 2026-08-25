using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using ConsultantWebPage.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyEvaluationController : ControllerBase
{
    private readonly IPropertyEvaluationServices _propertyEvaluationService;

    public PropertyEvaluationController(IPropertyEvaluationServices propertyEvaluationServices)
    {
        _propertyEvaluationService = propertyEvaluationServices;
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _propertyEvaluationService.GetAllEvaluationsAsync();

        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _propertyEvaluationService.GetEvaluationDetailsAsync(id);        
        
        if (result.IsSuccessful) 
            return Ok(result.Value);
            
        return BadRequest(result.Message); 
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpGet("consultant/{consultantId:int}/pending")]
    public async Task<IActionResult> GetPending(int consultantId)
    {
        var result = await _propertyEvaluationService.GetPendingEvaluationsAsync(consultantId);

        if (result.IsSuccessful)
            return Ok(new {hasProfile = result.Value});

        return BadRequest(result.Message);
    }
   
    [HttpPost]
    public async Task<IActionResult> RequestEvaluation([FromBody] RequestEvaluationRequest request)
    {    
        PropertyEvaluations evaluation;
        try
        {

            evaluation = new PropertyEvaluations(
                request.ConsultantId,
                request.ClientName,
                request.ClientContact,
                request.PropertyType ?? string.Empty,
                request.Address ?? string.Empty,
                request.Typology ?? string.Empty,
                request.Area,
                request.Condition ?? string.Empty,
                request.UserId,
                request.VisitPreference
            );
        }    
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _propertyEvaluationService.RequestEvaluationAsync(evaluation);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return StatusCode(201, new { message = "Pedido de avaliação enviado com sucesso." });
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] PropertyEvaluationUpdateStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NewStatus))
        {
            return BadRequest("O novo status não pode estar vazio.");
        }

        var result = await _propertyEvaluationService.UpdateStatusAsync(id, request.NewStatus);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return NoContent();
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _propertyEvaluationService.DeleteEvaluationAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — eliminado com sucesso, sem body
    }
}

public record RequestEvaluationRequest(
    int ConsultantId,
    int? UserId,
    string ClientName,
    string ClientContact,
    string? PropertyType,
    string? Address,
    string? Typology,
    decimal Area,
    string? Condition,
    string? VisitPreference
);

public record PropertyEvaluationUpdateStatusRequest(string NewStatus);