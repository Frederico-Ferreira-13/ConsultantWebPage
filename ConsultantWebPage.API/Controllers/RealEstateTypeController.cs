using ConsultantWebPage.Contracts.Interfaces.Services;
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

    // GET api/realestatetype
    // Lista todos os tipos → GET sem parâmetros, leitura pública
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _realEstateService.GetAllRealEstateTypesAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    // GET api/realestatetype/{id}
    // Detalhes de um tipo específico → GET + id na rota
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _realEstateService.GetRealEstateTypeByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    // GET api/realestatetype/exists?name=Apartamento
    // Verifica se um nome já existe → GET com query string
    // É GET porque é uma leitura — não altera estado
    // Útil para validação em tempo real no frontend antes de submeter o formulário
    [HttpGet("exists")]
    public async Task<IActionResult> CheckNameExists([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("O nome não pode estar vazio.");

        var exists = await _realEstateService.TypeNameExistsAsync(name);
        return Ok(new { exists });
    }

    // POST api/realestatetype
    // Criação de novo tipo → POST com body, retorna 201
    // Só admins devem poder criar tipos — [Authorize] obrigatório
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RealEstateTypeRequest request)
    {
        // Verifica duplicado antes de tentar criar
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

        // 201 com Location a apontar para o novo recurso
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.RealEstateTypeId }, result.Value);
    }

    // PUT api/realestatetype/{id}
    // Atualização completa → PUT porque só tens um campo editável (TypeName)
    // Quando a entidade é simples e tens apenas um campo, PUT e PATCH são equivalentes
    // Optamos por PUT porque o cliente envia o recurso "completo"
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RealEstateTypeRequest request)
    {
        var currentResult = await _realEstateService.GetRealEstateTypeByIdAsync(id);
        if (!currentResult.IsSuccessful)
            return NotFound("Tipo não encontrado.");

        // Verifica duplicado — mas ignora se o nome for o mesmo do atual
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
            // A lógica de validação fica na entidade
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

    // DELETE api/realestatetype/{id}
    // Eliminação → DELETE + id na rota, retorna 204
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _realEstateService.DeleteRealEstateTypeAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — eliminado com sucesso, sem body
    }
}

// Record partilhado para Create e Update — ambos precisam apenas do TypeName
public record RealEstateTypeRequest(string TypeName);