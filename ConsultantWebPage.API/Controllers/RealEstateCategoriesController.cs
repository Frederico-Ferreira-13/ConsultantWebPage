using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RealEstateCategoriesController : ControllerBase
{
    private readonly IRealEstateServices _realEstateService;

    public RealEstateCategoriesController(IRealEstateServices realEstateService)
    {
        _realEstateService = realEstateService;
    }
    
    [HttpGet("property/{realEstateId:int}")]
    public async Task<IActionResult> GetCategoriesByProperty(int realEstateId)
    {
        var result = await _realEstateService.GetCategoriesByPropertyIdAsync(realEstateId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }
    
    [HttpGet("category/{categoryId:int}/properties")]
    public async Task<IActionResult> GetPropertiesByCategory(int categoryId)
    {
        var result = await _realEstateService.GetPropertiesByCategoryIdAsync(categoryId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }
    
    [HttpGet("property/{realEstateId:int}/category/{categoryId:int}/exists")]
    public async Task<IActionResult> CheckCategoryAssignment(int realEstateId, int categoryId)
    {
        var isAssigned = await _realEstateService.IsPropertyInCategoryAsync(realEstateId, categoryId);
        return Ok(new { isAssigned });
    }

    // POST api/realestatecategories/property/{realEstateId}/category/{categoryId}
    // Associa uma categoria a um imóvel → POST porque cria uma relação nova
    // Os ids ficam na rota porque não há body — a relação é definida pelos dois ids
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPost("property/{realEstateId:int}/category/{categoryId:int}")]
    public async Task<IActionResult> AssignCategory(int realEstateId, int categoryId)
    {        
        try
        {
            _ = new RealEstateCategories(realEstateId, categoryId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _realEstateService.AssignCategoryToPropertyAsync(realEstateId, categoryId);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return StatusCode(201, new { realEstateId, categoryId });
    }
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPut("property/{realEstateId:int}/sync")]
    public async Task<IActionResult> SyncCategories(int realEstateId, [FromBody] SyncCategoriesRequest request)
    {
        if (request.CategoryIds == null || !request.CategoryIds.Any())
            return BadRequest("A lista de categorias não pode estar vazia.");

        var result = await _realEstateService.SyncPropertyCategoriesAsync(realEstateId, request.CategoryIds);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return NoContent();
    }
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpDelete("property/{realEstateId:int}/category/{categoryId:int}")]
    public async Task<IActionResult> RemoveCategory(int realEstateId, int categoryId)
    {
        var result = await _realEstateService.RemoveCategoryFromPropertyAsync(realEstateId, categoryId);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }
}

public record SyncCategoriesRequest(IEnumerable<int> CategoryIds);