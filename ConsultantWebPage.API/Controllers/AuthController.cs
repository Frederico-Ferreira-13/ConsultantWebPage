using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IUsersServices _usersService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthenticationService authService, IUsersServices usersService, 
        ITokenService tokenService)
    {
        _authService = authService;
        _usersService = usersService;
        _tokenService = tokenService;
    }

    // POST api/auth/login
    // Autenticação → POST porque altera estado (cria sessão/token)
    // Não é GET porque envia credenciais que não devem ficar na URL
     [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Identificador e password são obrigatórios.");
 
        var result = await _authService.AuthenticateAsync(request.Identifier, request.Password);
        if (!result.IsSuccessful)
        {
             if (result.ErrorCode == ErrorCodes.AuthForbidden)
                return StatusCode(403, result.Message);
                
            return Unauthorized(result.Message);
        }
            
 
        var user = result.Value!;
        var tokenResponse = _tokenService.GenerateToken(user);
 
        return Ok(new
        {
            token = tokenResponse.Token,
            expiration = tokenResponse.Expiration,
            user = new
            {
                userId = user.UserId,
                name = user.FullName,
                userName = user.UserName,
                email = user.Contact.Email,
                usersRoleId = user.UsersRoleId
            }
        });
    }

    // POST api/auth/logout
    // Logout → POST porque altera estado (invalida sessão)
     [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return NoContent();
    }

    // GET api/auth/me
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _authService.GetPersistedUserAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);
 
        return Unauthorized(result.Message);
    }
}

public record LoginRequest(string Identifier, string Password);