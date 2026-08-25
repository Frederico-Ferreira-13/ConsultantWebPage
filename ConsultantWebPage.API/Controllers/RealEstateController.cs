using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RealEstateController : ControllerBase
{
    private readonly IRealEstateServices _realEstateServices;
    private readonly IPropertyImportService _importService;
    private readonly IUsersServices _usersService;

    public RealEstateController(IRealEstateServices realEstateServices,
        IPropertyImportService importService,
        IUsersServices usersService)
    {
        _realEstateServices = realEstateServices;
        _importService = importService;
        _usersService = usersService;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _realEstateServices.GetPropertyDetailsAsync(id);
 
        if (!result.IsSuccessful)
            return NotFound(result.Message);
 
        return Ok(result.Value);
    }
 

    [HttpGet("search")]
    public async Task<IActionResult> SearchProperties([FromQuery] string? city,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? rooms)
    {
        var result = await _realEstateServices.SearchPropertiesAsync(city ?? string.Empty,
            maxPrice, rooms);
 
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Value);
    }
 
    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetByCategoryId(int categoryId)
    {
        var result = await _realEstateServices.GetPropertiesByCategoryIdAsync(categoryId);
 
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Value);
    }
 
    // Só o consultor/admin autenticado pode criar imóveis.
    // Faltava aqui — permitia criação anónima de imóveis.
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRealEstateRequest request)
    {
        try
        {
            var priceResult = Money.Create(request.Price, request.Currency ?? "EUR");
            if (!priceResult.IsSuccessful)
                return BadRequest(priceResult.Error.Message);
 
            var addressResult = Address.Create(
                request.Street, request.Number, request.City,
                request.PostalCode, request.Country, request.Complement);
 
            if (!addressResult.IsSuccessful)
                return BadRequest(addressResult.Error.Message);
 
            var propertyResult = RealEstate.Create(
                realEstateTypeId: request.RealEstateTypeId,
                consultantId: request.ConsultantId,
                cityId: request.CityId,
                title: request.Title,
                description: request.Description,
                thumbnailUrl: request.ThumbnailUrl,
                externalSourceUrl: request.ExternalSourceUrl,
                externalReferenceId: request.ExternalReferenceId,
                price: priceResult.Value!,
                address: addressResult.Value!,
                status: request.Status ?? "Disponível",
                isFeatured: request.IsFeatured
            );
 
            if (!propertyResult.IsSuccessful)
                return BadRequest(propertyResult.Error.Message);
 
            var result = await _realEstateServices.CreatePropertyAsync(propertyResult.Value!, request.ConsultantId);
 
            if (!result.IsSuccessful)
                return BadRequest(result.Message);
 
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.RealEstateId }, result.Value);
        }
        catch (Exception ex)
        {
            return BadRequest($"Ocorreu um erro ao criar o imóvel: {ex.Message}");
        }
    }
 
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var result = await _realEstateServices.UpdatePropertyStatusAsync(id, request.IsActive);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Message);
    }
 
    // Pedido de informação — mantém-se público (visitante anónimo/não registado).
    // Não precisa de UserId porque gera diretamente um Lead com o contacto fornecido.
    [HttpPost("{id:int}/information")]
    public async Task<IActionResult> RequestInformation(int id, [FromBody] RequestInformationRequest request)
    {
        var result = await _realEstateServices.RequestInformationAsync(
            id, request.ClientName, request.ClientEmail, request.Message);
 
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Message);
    }
 
    // Agendar visita exige conta — o clientId vem sempre do token, nunca do body.
    [Authorize]
    [HttpPost("{id:int}/visit")]
    public async Task<IActionResult> ScheduleVisit(int id, [FromBody] ScheduleVisitRequest request)
    {
        var currentUserIdResult = await _usersService.GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
            return Unauthorized("Não foi possível identificar o utilizador.");
 
        var result = await _realEstateServices.ScheduleVisitAsync(id, currentUserIdResult.Value, request.PreferredDate);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Message);
    }
 
    [HttpGet("{id:int}/comments")]
    public async Task<IActionResult> GetComments(int id)
    {
        var result = await _realEstateServices.GetCommentsAsync(id);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Value);
    }
 
    // Comentar exige conta — o UserId vem sempre do token, nunca do body.
    [Authorize]
    [HttpPost("{id:int}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentRequest request)
    {
        var currentUserIdResult = await _usersService.GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
            return Unauthorized("Não foi possível identificar o utilizador.");
 
        var result = await _realEstateServices.AddCommentAsync(id, currentUserIdResult.Value, request.CommentText);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Message);
    }
 
    [HttpGet("{id:int}/categories")]
    public async Task<IActionResult> GetCategories(int id)
    {
        var result = await _realEstateServices.GetCategoriesByPropertyIdAsync(id);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Value);
    }
 
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPost("{id:int}/categories/{categoryId:int}")]
    public async Task<IActionResult> AssignCategory(int id, int categoryId)
    {
        var result = await _realEstateServices.AssignCategoryToPropertyAsync(id, categoryId);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Message);
    }
 
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpDelete("{id:int}/categories/{categoryId:int}")]
    public async Task<IActionResult> RemoveCategory(int id, int categoryId)
    {
        var result = await _realEstateServices.RemoveCategoryFromPropertyAsync(id, categoryId);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Message);
    }
 
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPut("{id:int}/categories")]
    public async Task<IActionResult> SyncCategories(int id, [FromBody] RealEstateSyncCategoriesRequest request)
    {
        var result = await _realEstateServices.SyncPropertyCategoriesAsync(id, request.CategoryIds);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
 
        return Ok(result.Message);
    }
 
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPost("import")]
    public async Task<IActionResult> ImportFromUrl([FromBody] ImportPropertyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("O URL é obrigatório.");
 
        var result = await _importService.ImportFromUrlAsync(request.Url);
 
        if (!result.Success)
            return BadRequest(new { message = "Não foi possível importar dados deste URL.", warnings = result.Warnings });
 
        return Ok(result);
    }
}
 
public record CreateRealEstateRequest(
    int RealEstateTypeId,
    int ConsultantId,
    int CityId,
    string Title,
    string? Description,
    string? ThumbnailUrl,
    string? ExternalSourceUrl,
    string? ExternalReferenceId,
    decimal Price,
    string? Currency,
    string Street,
    string Number,
    string City,
    string PostalCode,
    string Country,
    string? Complement,
    string? Status,
    bool IsFeatured
);
 
public record UpdateStatusRequest(bool IsActive);
 
public record RequestInformationRequest(
    string ClientName,
    string ClientEmail,
    string Message
);
 
// ClientId removido — agora vem sempre do token autenticado.
public record ScheduleVisitRequest(
    DateTime PreferredDate
);
 
// UserId removido — agora vem sempre do token autenticado.
public record AddCommentRequest(
    string CommentText
);
 
public record RealEstateSyncCategoriesRequest(IEnumerable<int> CategoryIds);
 
public record ImportPropertyRequest(string Url);