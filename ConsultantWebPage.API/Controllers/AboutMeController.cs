using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AboutMeController : ControllerBase
{
    private readonly IAboutMeServices _aboutMeServices;

    public AboutMeController(IAboutMeServices aboutMeServices)
    {
        _aboutMeServices = aboutMeServices;
    }

    [HttpGet]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var result = await _aboutMeServices.GetByUserIdAsync(userId);        
        
        if (result.IsSuccessful) 
            return Ok(result.Value);
            
        return BadRequest(result.Message); 
    }

    [HttpGet("user/{userId:int}/has-profile")]
    public async Task<IActionResult> HasProfile(int userId)
    {
        var result = await _aboutMeServices.HasProfileAsync(userId);

        if (result.IsSuccessful)
            return Ok(new {hasProfile = result.Value});

        return BadRequest(result.Message);
    }
   
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAboutMeRequest request)
    {        
        var aboutMe = new AboutMe(
            request.UserId,
            request.Bio ?? string.Empty,
            request.ProfileImageUrl ?? string.Empty,
            request.YearsOfExperience,
            request.LinkedIn ?? string.Empty,
            request.Instagram ?? string.Empty,
            request.X ?? string.Empty,
            request.Facebook ?? string.Empty
        );

        var result = await _aboutMeServices.CreateAboutMeAsync(aboutMe);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return CreatedAtAction(nameof(GetByUserId), new { userId = result.Value!.UserId }, result.Value);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAboutMeRequest request)
    {
        // Primeiro, vais buscar o perfil atual para garantir que ele existe antes de atualizar
        var currentResult = await _aboutMeServices.GetByUserIdAsync(request.UserId);
        if (!currentResult.IsSuccessful)
            return NotFound("Perfil não encontrado para atualização.");

        var aboutMeToUpdate = currentResult.Value!;
        
        // Usa o método de mutação que criaste dentro da tua própria classe AboutMe
        aboutMeToUpdate.UpdateAboutMe(
            request.Bio ?? string.Empty,
            request.ProfileImageUrl ?? string.Empty,
            request.YearsOfExperience,
            request.LinkedIn ?? string.Empty,
            request.Instagram ?? string.Empty,
            request.X ?? string.Empty,
            request.Facebook ?? string.Empty
        );

        var result = await _aboutMeServices.UpdateAboutMeAsync(aboutMeToUpdate);
        
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Value);
    }
}

public record CreateAboutMeRequest(
    int UserId,
    string? Bio,
    string? ProfileImageUrl,
    int YearsOfExperience,
    string? LinkedIn,
    string? Instagram,
    string? X,
    string? Facebook
);

public record UpdateAboutMeRequest(
    int UserId,
    string? Bio,
    string? ProfileImageUrl,
    int YearsOfExperience,
    string? LinkedIn,
    string? Instagram,
    string? X,
    string? Facebook
);