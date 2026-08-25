using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class UsersRoleController : ControllerBase
{
    private readonly IUsersServices _usersService;

    public UsersRoleController(IUsersServices usersService)
    {
        _usersService = usersService;
    }
   
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _usersService.GetAllUsersRolesAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _usersService.GetUsersRoleByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }
    
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
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _usersService.DeleteUsersRoleAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }
}

public record UsersRoleRequest(string RoleName);