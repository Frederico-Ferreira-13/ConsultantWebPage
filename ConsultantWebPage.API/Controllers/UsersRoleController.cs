using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Gestão de roles é sempre restrita
public class UsersRoleController : ControllerBase
{
    private readonly IUsersServices _usersService;

    public UsersRoleController(IUsersServices usersService)
    {
        _usersService = usersService;
    }

    // GET api/usersrole
    // Lista todas as roles → GET sem parâmetros
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _usersService.GetAllUsersRolesAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    // GET api/usersrole/{id}
    // Detalhes de uma role → GET + id na rota
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _usersService.GetUsersRoleByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    // GET api/usersrole/name/{name}
    // Busca por nome → GET com sub-rota descritiva
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("O nome não pode estar vazio.");

        var result = await _usersService.GetUsersRoleByNameAsync(name);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    // POST api/usersrole
    // Criação de nova role → POST com body
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UsersRoleRequest request)
    {
        UsersRole newRole;
        try
        {
            newRole = new UsersRole(request.RoleName);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _usersService.CreateUsersRoleAsync(newRole);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.UsersRoleId }, result.Value);
    }

    // PUT api/usersrole/{id}
    // Atualização completa → PUT porque só há um campo editável (RoleName)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UsersRoleRequest request)
    {
        var currentResult = await _usersService.GetUsersRoleByIdAsync(id);
        if (!currentResult.IsSuccessful)
            return NotFound("Role não encontrada.");

        var role = currentResult.Value!;

        try
        {
            role.UpdateName(request.RoleName);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _usersService.UpdateUsersRoleAsync(role);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return Ok(role);
    }

    // DELETE api/usersrole/{id}
    // Eliminação → DELETE + id na rota
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _usersService.DeleteUsersRoleAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — eliminado com sucesso
    }
}

public record UsersRoleRequest(string RoleName);