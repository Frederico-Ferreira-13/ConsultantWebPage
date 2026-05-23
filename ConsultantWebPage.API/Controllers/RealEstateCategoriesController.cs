using ConsultantWebPage.Contracts.Interfaces.Services;
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

    // GET api/realestatecategories/property/{realEstateId}
    // Lista todas as categorias de um imóvel → GET com sub-rota de contexto
    [HttpGet("property/{realEstateId:int}")]
    public async Task<IActionResult> GetCategoriesByProperty(int realEstateId)
    {
        var result = await _realEstateService.GetCategoriesByPropertyIdAsync(realEstateId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    // GET api/realestatecategories/category/{categoryId}/properties
    // Lista todos os imóveis de uma categoria → GET com sub-rota descritiva
    // "properties" no final torna explícito que devolves imóveis, não categorias
    [HttpGet("category/{categoryId:int}/properties")]
    public async Task<IActionResult> GetPropertiesByCategory(int categoryId)
    {
        var result = await _realEstateService.GetPropertiesByCategoryIdAsync(categoryId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    // GET api/realestatecategories/property/{realEstateId}/category/{categoryId}/exists
    // Verifica se um imóvel pertence a uma categoria → GET que devolve bool
    // "exists" na rota deixa claro que é uma verificação, não uma listagem
    [HttpGet("property/{realEstateId:int}/category/{categoryId:int}/exists")]
    public async Task<IActionResult> CheckCategoryAssignment(int realEstateId, int categoryId)
    {
        var isAssigned = await _realEstateService.IsPropertyInCategoryAsync(realEstateId, categoryId);
        return Ok(new { isAssigned });
    }

    // POST api/realestatecategories/property/{realEstateId}/category/{categoryId}
    // Associa uma categoria a um imóvel → POST porque cria uma relação nova
    // Os ids ficam na rota porque não há body — a relação é definida pelos dois ids
    [Authorize]
    [HttpPost("property/{realEstateId:int}/category/{categoryId:int}")]
    public async Task<IActionResult> AssignCategory(int realEstateId, int categoryId)
    {
        // Validação na entidade antes de chamar o serviço
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

    // PUT api/realestatecategories/property/{realEstateId}/sync
    // Sincroniza a lista completa de categorias → PUT porque substitui o conjunto inteiro
    // É PUT e não PATCH porque estás a definir o estado final completo das categorias,
    // não a fazer uma alteração parcial — categorias não enviadas são removidas
    [Authorize]
    [HttpPut("property/{realEstateId:int}/sync")]
    public async Task<IActionResult> SyncCategories(int realEstateId, [FromBody] SyncCategoriesRequest request)
    {
        if (request.CategoryIds == null || !request.CategoryIds.Any())
            return BadRequest("A lista de categorias não pode estar vazia.");

        var result = await _realEstateService.SyncPropertyCategoriesAsync(realEstateId, request.CategoryIds);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return NoContent(); // 204 — sincronização bem sucedida, sem necessidade de devolver dados
    }

    // DELETE api/realestatecategories/property/{realEstateId}/category/{categoryId}
    // Remove a associação entre imóvel e categoria → DELETE com dois ids na rota
    // Sem body porque a relação é completamente identificada pelos dois ids
    [Authorize]
    [HttpDelete("property/{realEstateId:int}/category/{categoryId:int}")]
    public async Task<IActionResult> RemoveCategory(int realEstateId, int categoryId)
    {
        var result = await _realEstateService.RemoveCategoryFromPropertyAsync(realEstateId, categoryId);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — removido com sucesso, sem body
    }
}

public record SyncCategoriesRequest(IEnumerable<int> CategoryIds);