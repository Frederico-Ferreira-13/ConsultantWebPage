using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoritesServices _favoritesService;

    public FavoritesController(IFavoritesServices favoritesServices)
    {
        _favoritesService = favoritesServices;
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserFavorites(int userId)
    {
        var result = await _favoritesService.GetUserFavoritesAsync(userId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest request)
    {        
        var userIdResult = await _favoritesService.GetCurrentUserIdAsync();
        if (!userIdResult.IsSuccessful)
            return Unauthorized("Não foi possível identificar o utilizador.");

        Favorites favorite;
        try
        {
            favorite = new Favorites(userIdResult.Value, request.RealEstateId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _favoritesService.AddFavoriteAsync(favorite);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return Created($"api/favorites/{userIdResult.Value}", null);
    }

    [HttpDelete("{favoriteId:int}")]
    public async Task<IActionResult> RemoveFavorite(int favoriteId)
    {
        var result = await _favoritesService.RemoveFavoriteAsync(favoriteId);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — padrão REST para DELETE bem sucedido
    }

    [HttpPost("toggle/{realEstateId:int}")]
    public async Task<IActionResult> ToggleFavorite(int realEstateId)
    {
        var result = await _favoritesService.ToggleFavoriteAsync(realEstateId);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        // Retorna o novo estado: true = adicionado, false = removido
        return Ok(new { isFavorite = result.Value });
    }
}

public record AddFavoriteRequest(int RealEstateId);
