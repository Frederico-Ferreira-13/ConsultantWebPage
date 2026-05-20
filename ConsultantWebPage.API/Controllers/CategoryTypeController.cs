using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryTypeController : ControllerBase
{
    private readonly ICategoryServices _categoryServices;

    public CategoryTypeController(ICategoryServices categoryServices)
    {
        _categoryServices = categoryServices;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryServices.GetAllCategoryTypesAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryServices.GetCategoryTypeByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("O nome para pesquisa não pode estar vazio.");

        var result = await _categoryServices.GetCategoryTypeByNameAsync(name);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryTypeRequest request)
    {
        // Instancia o modelo Domain usando o teu construtor que valida o texto
        var categoryType = new CategoryType(request.TypeName);

        var result = await _categoryServices.CreateCategoryTypeAsync(categoryType);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.CategoryTypeId }, result.Value);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryTypeRequest request)
    {
        // 1. Verificar se o registo existe na Base de Dados
        var currentResult = await _categoryServices.GetCategoryTypeByIdAsync(id);
        if (!currentResult.IsSuccessful)
            return NotFound("Tipo de categoria não encontrado para atualização.");

        var categoryTypeToUpdate = currentResult.Value!;

        // 2. Usar o método de mutação do Domain para atualizar o estado interno com segurança
        categoryTypeToUpdate.UpdateCategoryType(request.TypeName);

        var result = await _categoryServices.UpdateCategoryTypeAsync(categoryTypeToUpdate);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryServices.DeleteCategoryTypeAsync(id);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return NoContent(); // Retorna 204 No Content se for apagado com sucesso
    }
}

public record CreateCategoryTypeRequest(string TypeName);
public record UpdateCategoryTypeRequest(string TypeName);