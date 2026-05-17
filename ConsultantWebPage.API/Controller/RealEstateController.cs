using ConsultantWebPage.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantWebPage.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class RealEstateController : ControllerBase
{
    private readonly IRealEstateServices _realEstateServices;

    public RealEstateController(IRealEstateServices realEstateServices)
    {
        _realEstateServices = realEstateServices;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRealEstates()
    {
        var result = await _realEstateServices.GetAllActivePropertiesAsync();
        
        // Corrigido para bater certo com a tua classe Result
        if (result.IsSuccessful) 
            return Ok(result.Value);
            
        return BadRequest(result.Message); 
    }
}