using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingsController : ControllerBase
{
    private readonly IRatingsServices _ratingsService;
    private readonly IUsersServices _usersService;

    public RatingsController(IRatingsServices ratingsServices, IUsersServices usersService)
    {
        _ratingsService = ratingsServices;
        _usersService = usersService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _ratingsService.GetRatingByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    [HttpGet("property/{realEstateId:int}")]
    public async Task<IActionResult> GetByProperty(int realEstateId)
    {
        var result = await _ratingsService.GetRatingsByPropertyIdAsync(realEstateId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [HttpGet("property/{realEstateId:int}/average")]
    public async Task<IActionResult> GetPropertyAverage(int realEstateId)
    {
        var result = await _ratingsService.GetAverageForPropertyAsync(realEstateId);
        if (result.IsSuccessful)
            return Ok(new { average = result.Value });

        return BadRequest(result.Message);
    }

    [HttpGet("property/{realEstateId:int}/user/{userId:int}")]
    public async Task<IActionResult> GetUserRatingForProperty(int realEstateId, int userId)
    {
        var result = await _ratingsService.GetUserRatingForPropertyAsync(realEstateId, userId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    [HttpGet("consultant/{consultantId:int}/average")]
    public async Task<IActionResult> GetConsultantAverage(int consultantId)
    {
        var result = await _ratingsService.GetAverageRatingForConsultantAsync(consultantId);
        if (result.IsSuccessful)
            return Ok(new { average = result.Value });

        return BadRequest(result.Message);
    }

    // O UserId nunca vem do body — evita que um utilizador avalie "em nome" de outro.
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddOrUpdate([FromBody] AddRatingRequest request)
    {
        try
        {
            StarRating.Create(request.Stars);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }

        var currentUserIdResult = await _usersService.GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
            return Unauthorized("Não foi possível identificar o utilizador.");

        var result = await _ratingsService.AddOrUpdateRatingAsync(
            currentUserIdResult.Value,
            request.RealEstateId,
            request.Stars
        );

        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        // 200 em vez de 201 porque pode ser uma atualização e não uma criação
        return Ok(new { message = "Avaliação registada com sucesso." });
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _ratingsService.DeleteRatingAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — eliminado com sucesso, sem body
    }

    // UserId removido — agora vem sempre do token autenticado.
    public record AddRatingRequest(
        int RealEstateId,
        int Stars
    );
}