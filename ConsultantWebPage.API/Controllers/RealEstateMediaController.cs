using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RealEstateMediaController : ControllerBase
{
    private readonly IRealEstateServices _realEstateService;

    public RealEstateMediaController(IRealEstateServices realEstateService)
    {
        _realEstateService = realEstateService;
    }

    // GET api/realestatema/property/{realEstateId}
    // Lista todos os media de um imóvel → GET com sub-rota de contexto
    [HttpGet("property/{realEstateId:int}")]
    public async Task<IActionResult> GetByProperty(int realEstateId)
    {
        var result = await _realEstateService.GetMediaByPropertyIdAsync(realEstateId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    // GET api/realestatema/property/{realEstateId}/main
    // Imagem principal de um imóvel → GET com sub-rota descritiva
    // Separado do anterior porque devolve um único item em vez de uma lista
    [HttpGet("property/{realEstateId:int}/main")]
    public async Task<IActionResult> GetMainImage(int realEstateId)
    {
        var result = await _realEstateService.GetMainImageAsync(realEstateId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    // POST api/realestatema
    // Adiciona um único media → POST com body
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddMedia([FromBody] AddMediaRequest request)
    {
        RealEstateMedia media;
        try
        {
            media = new RealEstateMedia(
                request.RealEstateId,
                request.MediaUrl,
                request.MediaType,
                request.IsMain
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _realEstateService.AddMediaAsync(media);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return StatusCode(201, result.Value);
    }

    // POST api/realestatema/batch
    // Adiciona múltiplos media de uma vez → POST com sub-rota "batch"
    // É um POST separado e não o mesmo endpoint porque o serviço e o body são diferentes
    // "batch" é a convenção para operações em massa
    [Authorize]
    [HttpPost("batch")]
    public async Task<IActionResult> AddMultipleMedia([FromBody] AddMultipleMediaRequest request)
    {
        if (request.MediaList == null || !request.MediaList.Any())
            return BadRequest("A lista de media não pode estar vazia.");

        var mediaList = new List<RealEstateMedia>();

        foreach (var item in request.MediaList)
        {
            try
            {
                mediaList.Add(new RealEstateMedia(
                    item.RealEstateId,
                    item.MediaUrl,
                    item.MediaType,
                    item.IsMain
                ));
            }
            catch (ArgumentException ex)
            {
                // Identificamos qual o item inválido pelo índice para facilitar debug
                return BadRequest($"Item inválido ({item.MediaUrl}): {ex.Message}");
            }
        }

        var result = await _realEstateService.AddMultipleMediaAsync(mediaList);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return StatusCode(201, result.Value);
    }

    // PATCH api/realestatema/{mediaId}/main
    // Define a imagem principal → PATCH porque só altera o campo IsMain
    // Requer também o realEstateId no body para o serviço poder limpar o IsMain anterior
    [Authorize]
    [HttpPatch("{mediaId:int}/main")]
    public async Task<IActionResult> SetMainImage(int mediaId, [FromBody] SetMainImageRequest request)
    {
        var result = await _realEstateService.SetMainImageAsync(mediaId, request.RealEstateId);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — alteração bem sucedida
    }

    // PATCH api/realestatema/order
    // Reordena os media → PATCH porque é uma atualização parcial (só a ordem muda)
    // Não é PUT porque não estás a substituir os recursos, só a mudar a sua sequência
    [Authorize]
    [HttpPatch("order")]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateMediaOrderRequest request)
    {
        if (request.MediaIdsInOrder == null || !request.MediaIdsInOrder.Any())
            return BadRequest("A lista de IDs não pode estar vazia.");

        var result = await _realEstateService.UpdateMediaOrderAsync(request.MediaIdsInOrder);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return NoContent(); // 204 — reordenação bem sucedida
    }

    // DELETE api/realestatema/{mediaId}
    // Elimina um media → DELETE + id na rota, retorna 204
    [Authorize]
    [HttpDelete("{mediaId:int}")]
    public async Task<IActionResult> Delete(int mediaId)
    {
        var result = await _realEstateService.DeleteMediaAsync(mediaId);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — eliminado com sucesso, sem body
    }
}

// Records de request
public record AddMediaRequest(
    int RealEstateId,
    string MediaUrl,
    string MediaType,
    bool IsMain
);

public record AddMultipleMediaRequest(
    IEnumerable<AddMediaRequest> MediaList
);

public record SetMainImageRequest(int RealEstateId);

public record UpdateMediaOrderRequest(List<int> MediaIdsInOrder);