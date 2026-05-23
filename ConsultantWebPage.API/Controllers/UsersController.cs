using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersServices _usersService;

    public UsersController(IUsersServices usersService)
    {
        _usersService = usersService;
    }

    // GET api/users/{id}
    // Perfil de um utilizador → GET + id na rota
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _usersService.GetUserByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    // GET api/users/me
    // Utilizador autenticado atual → GET, rota fixa "me"
    // Separado do GetById para não exigir que o cliente conheça o seu próprio id
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _usersService.GetCurrentUserAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return Unauthorized(result.Message);
    }

    // GET api/users/pending
    // Lista utilizadores pendentes de aprovação → GET com sub-rota descritiva
    // Só admins devem aceder a esta rota
    [Authorize]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _usersService.GetPendingUsersAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    // GET api/users/exists/email?email=...
    // Verifica se email já existe → GET com query string
    // Útil para validação em tempo real no formulário de registo
    [HttpGet("exists/email")]
    public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("O email não pode estar vazio.");

        var exists = await _usersService.UserExistsByEmailAsync(email);
        return Ok(new { exists });
    }

    // GET api/users/exists/username?username=...
    // Verifica se username já existe → GET com query string
    [HttpGet("exists/username")]
    public async Task<IActionResult> CheckUsernameUnique([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("O username não pode estar vazio.");

        var isUnique = await _usersService.IsUserNameUniqueAsync(username);
        return Ok(new { isUnique });
    }

    // POST api/users/register
    // Registo de novo utilizador → POST sem autenticação
    // Sub-rota "register" porque POST /users poderia colidir com criação admin
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var result = await _usersService.RegisterUserAsync(
            request.UserName,
            request.FullName,
            request.Email,
            request.Password
        );

        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.UserId }, result.Value);
    }

    // PUT api/users/{id}
    // Atualização completa do perfil → PUT com todos os campos editáveis
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileRequest request)
    {
        var currentResult = await _usersService.GetUserByIdAsync(id);
        if (!currentResult.IsSuccessful)
            return NotFound("Utilizador não encontrado.");

        var user = currentResult.Value!;

        try
        {
            user.UpdateUserName(request.UserName);
            user.UpdateSocialLinks(request.FacebookUrl, request.InstagramUrl, request.XUrl);

            if (request.ProfilePicture != null)
                user.UpdateProfilePicture(request.ProfilePicture);

            if (request.Email != null || request.Phone != null)
            {
                var contactResult = ContactInfo.Create(
                    request.Email ?? user.Contact.Email,
                    request.Phone ?? user.Contact.PhoneNumber
                );
                if (!contactResult.IsSuccessful)
                    return BadRequest(contactResult.Message);

                user.UpdateContact(contactResult.Value!);
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _usersService.UpdateUserProfileAsync(user);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return Ok(user);
    }

    // PATCH api/users/{id}/password
    // Alteração de password → PATCH porque só um aspeto muda
    [Authorize]
    [HttpPatch("{id:int}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest("Password antiga e nova são obrigatórias.");

        var result = await _usersService.ChangeUserPasswordAsync(id, request.OldPassword, request.NewPassword);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return NoContent(); // 204 — password alterada com sucesso
    }

    // PATCH api/users/{id}/approve
    // Aprovação de utilizador → PATCH porque só IsApproved muda
    [Authorize]
    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await _usersService.ApproveUserAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — aprovação bem sucedida
    }

    // PATCH api/users/{id}/deactivate
    // Desativar utilizador → PATCH porque só IsActive muda
    [Authorize]
    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _usersService.DeactivateUserAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }

    // PATCH api/users/{id}/activate
    // Reativar utilizador → PATCH porque só IsActive muda
    [Authorize]
    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        var result = await _usersService.ActivateUserAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }

    // DELETE api/users/{id}
    // Eliminação permanente → DELETE + id na rota
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _usersService.DeleteUserAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — eliminado com sucesso
    }
}

public record RegisterUserRequest(
    string FullName,
    string UserName,
    string Email,
    string Password
);

public record UpdateProfileRequest(
    string UserName,
    string? Email,
    string? Phone,
    string? ProfilePicture,
    string? FacebookUrl,
    string? InstagramUrl,
    string? XUrl
);

public record ChangePasswordRequest(string OldPassword, string NewPassword);