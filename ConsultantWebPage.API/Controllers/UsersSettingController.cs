using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Todas as definições requerem autenticação
public class UserSettingsController : ControllerBase
{
    private readonly IUsersServices _usersService;

    public UserSettingsController(IUsersServices usersService)
    {
        _usersService = usersService;
    }

    // GET api/usersettings/user/{userId}
    // Definições de um utilizador → GET com sub-rota de contexto
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var result = await _usersService.GetSettingsByUserIdAsync(userId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    // POST api/usersettings/user/{userId}/default
    // Cria definições padrão → POST com sub-rota "default"
    // Chamado automaticamente após registo para criar as definições iniciais
    [HttpPost("user/{userId:int}/default")]
    public async Task<IActionResult> CreateDefault(int userId)
    {
        var result = await _usersService.CreateDefaultSettingsAsync(userId);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return StatusCode(201, result.Value);
    }

    // PUT api/usersettings/user/{userId}
    // Atualização completa das definições → PUT porque envias todos os campos
    [HttpPut("user/{userId:int}")]
    public async Task<IActionResult> Update(int userId, [FromBody] UpdateSettingsRequest request)
    {
        var currentResult = await _usersService.GetSettingsByUserIdAsync(userId);
        if (!currentResult.IsSuccessful)
            return NotFound("Definições não encontradas.");

        var settings = currentResult.Value!;

        try
        {
            settings.UpdateSettings(request.Theme, request.Language, request.ReceiveNotifications);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _usersService.UpdateUserSettingsAsync(settings);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return Ok(settings);
    }
}

public record UpdateSettingsRequest(
    string Theme,
    string Language,
    bool ReceiveNotifications
);