using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryServices _categoryServices;

    public CategoryController(ICategoryServices categoryServices)
    {
        _categoryServices = categoryServices;
    }

    [HttpGet("root")]
    public async Task<IActionResult> GetRootCategories()
    {
        var result = await _categoryServices.GetRootCategoriesAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryServices.GetCategoryByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }
   
    [HttpGet("{id:int}/with-subs")]
    public async Task<IActionResult> GetCategoryWithSubs(int id)
    {
        var result = await _categoryServices.GetCategoryWithSubsAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        // Instancia o modelo Domain usando o construtor da classe Category
        var category = new Category(
            request.CategoryName,
            request.CategoryTypeId,
            request.ParentCategoryId
        );

        var result = await _categoryServices.CreateCategoryAsync(category);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.CategoriesId }, result.Value);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        // 1. Validar se a categoria existe
        var currentResult = await _categoryServices.GetCategoryByIdAsync(id);
        if (!currentResult.IsSuccessful)
            return NotFound("Categoria não encontrada para atualização.");

        var categoryToUpdate = currentResult.Value!;

        // 2. Usar o método de mutação seguro definido no teu Domain
        categoryToUpdate.UpdateCategory(
            request.CategoryName,
            request.CategoryTypeId,
            request.ParentCategoryId
        );

        var result = await _categoryServices.UpdateCategoryAsync(categoryToUpdate);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryServices.DeleteCategoryAsync(id);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return NoContent(); // Resposta padrão 204 para remoções feitas com sucesso
    }
}

public record CreateCategoryRequest(
    string CategoryName,
    int CategoryTypeId,
    int? ParentCategoryId
);

public record UpdateCategoryRequest(
    string CategoryName,
    int CategoryTypeId,
    int? ParentCategoryId
);