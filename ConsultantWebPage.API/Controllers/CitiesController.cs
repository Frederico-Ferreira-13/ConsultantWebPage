using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICitiesServices _citiesServices;

    public CitiesController(ICitiesServices citiesServices)
    {
        _citiesServices = citiesServices;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _citiesServices.GetAllCitiesAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _citiesServices.GetCityByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> GetByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("O nome da cidade não pode estar vazio.");

        var result = await _citiesServices.GetCityByNameAsync(name);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    [HttpGet("district/{districtName}")]
    public async Task<IActionResult> GetByDistrict(string districtName)
    {
        var result = await _citiesServices.GetByDistrictAsync(districtName);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [HttpGet("districts")]
    public async Task<IActionResult> GetAllDistricts()
    {
        var result = await _citiesServices.GetAllDistrictsAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCityRequest request)
    {
        var validationResult = Cities.Create(request.CityName, request.DistrictName);
        
        if (!validationResult.IsSuccessful)
            return BadRequest(validationResult.Message);

        var result = await _citiesServices.CreateCityAsync(request.CityName, request.DistrictName);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.CityId }, result.Value);
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCityRequest request)
    {
        var currentCityResult = await _citiesServices.GetCityByIdAsync(id);
        if (!currentCityResult.IsSuccessful)
            return NotFound("Cidade não encontrada para atualização.");

        var cityToUpdate = currentCityResult.Value!;

        var domainUpdateResult = cityToUpdate.Update(request.CityName, request.DistrictName);
        if (!domainUpdateResult.IsSuccessful)
            return BadRequest(domainUpdateResult.Message);

        var result = await _citiesServices.UpdateCityAsync(id, request.CityName, request.DistrictName);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return Ok(cityToUpdate);
    }
}

public record CreateCityRequest(string CityName, string DistrictName);
public record UpdateCityRequest(string CityName, string DistrictName);