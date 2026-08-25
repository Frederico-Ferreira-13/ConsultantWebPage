using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
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

    [HttpGet("property/{realEstateId:int}")]
    public async Task<IActionResult> GetByProperty(int realEstateId)
    {
        var result = await _realEstateService.GetMediaByPropertyIdAsync(realEstateId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }
    
    [HttpGet("property/{realEstateId:int}/main")]
    public async Task<IActionResult> GetMainImage(int realEstateId)
    {
        var result = await _realEstateService.GetMainImageAsync(realEstateId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }
  
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
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
   
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
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
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPatch("{mediaId:int}/main")]
    public async Task<IActionResult> SetMainImage(int mediaId, [FromBody] SetMainImageRequest request)
    {
        var result = await _realEstateService.SetMainImageAsync(mediaId, request.RealEstateId);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPatch("order")]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateMediaOrderRequest request)
    {
        if (request.MediaIdsInOrder == null || !request.MediaIdsInOrder.Any())
            return BadRequest("A lista de IDs não pode estar vazia.");

        var result = await _realEstateService.UpdateMediaOrderAsync(request.MediaIdsInOrder);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return NoContent();
    }
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpDelete("{mediaId:int}")]
    public async Task<IActionResult> Delete(int mediaId)
    {
        var result = await _realEstateService.DeleteMediaAsync(mediaId);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }
}

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