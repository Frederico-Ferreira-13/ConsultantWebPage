using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RealEstateTypeController : ControllerBase
{
    private readonly IRealEstateServices _realEstateService;

    public RealEstateTypeController(IRealEstateServices realEstateService)
    {
        _realEstateService = realEstateService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _realEstateService.GetAllRealEstateTypesAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _realEstateService.GetRealEstateTypeByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }
    
    [HttpGet("exists")]
    public async Task<IActionResult> CheckNameExists([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("O nome não pode estar vazio.");

        var exists = await _realEstateService.TypeNameExistsAsync(name);
        return Ok(new { exists });
    }

    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RealEstateTypeRequest request)
    {
        var exists = await _realEstateService.TypeNameExistsAsync(request.TypeName);
        if (exists)
            return Conflict($"Já existe um tipo com o nome '{request.TypeName}'.");

        RealEstateType newType;
        try
        {
            newType = new RealEstateType(request.TypeName);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _realEstateService.CreateRealEstateTypeAsync(newType);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.RealEstateTypeId }, result.Value);
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RealEstateTypeRequest request)
    {
        var currentResult = await _realEstateService.GetRealEstateTypeByIdAsync(id);
        if (!currentResult.IsSuccessful)
            return NotFound("Tipo não encontrado.");

        var isSameName = currentResult.Value!.TypeName
            .Equals(request.TypeName, StringComparison.OrdinalIgnoreCase);

        if (!isSameName)
        {
            var exists = await _realEstateService.TypeNameExistsAsync(request.TypeName);
            if (exists)
                return Conflict($"Já existe um tipo com o nome '{request.TypeName}'.");
        }

        var typeToUpdate = currentResult.Value!;

        try
        {            
            typeToUpdate.UpdateRealEstateType(request.TypeName);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _realEstateService.UpdateRealEstateTypeAsync(typeToUpdate);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return Ok(result.Value);
    }
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _realEstateService.DeleteRealEstateTypeAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }
}

public record RealEstateTypeRequest(string TypeName);