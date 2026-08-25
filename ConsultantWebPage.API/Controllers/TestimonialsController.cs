using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestimonialsController : ControllerBase
{
    private readonly ITestimonialsServices _testimonialsService;

    public TestimonialsController(ITestimonialsServices testimonialsService)
    {
        _testimonialsService = testimonialsService;
    }
   
    [HttpGet("consultant/{consultantId:int}")]
    public async Task<IActionResult> GetApproved(int consultantId)
    {
        var result = await _testimonialsService.GetApprovedTestimonialsAsync(consultantId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpGet("consultant/{consultantId:int}/moderation")]
    public async Task<IActionResult> GetAllForModeration(int consultantId)
    {
        var result = await _testimonialsService.GetAllTestimonialsForModerationAsync(consultantId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }
    
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitTestimonialRequest request)
    {
        Testimonials testimonial;
        try
        {
            testimonial = new Testimonials(
                request.ConsultantId,
                request.ClientName,
                request.TestimonialText,
                request.Rating,
                isVisible: false // sempre começa invisível, aguarda moderação
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _testimonialsService.SubmitTestimonialAsync(testimonial);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        // 202 Accepted em vez de 201 Created — o recurso foi recebido mas ainda não está ativo
        // Comunica ao cliente que o testemunho está pendente de aprovação
        return Accepted(new { message = "Testemunho submetido com sucesso. Ficará visível após aprovação." });
    }
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPatch("{id:int}/visibility")]
    public async Task<IActionResult> UpdateVisibility(int id, [FromBody] TestimonialVisibilityRequest request)
    {
        var result = await _testimonialsService.UpdateVisibilityAsync(id, request.IsVisible);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }
    
    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _testimonialsService.DeleteTestimonialAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }
}

public record SubmitTestimonialRequest(
    int ConsultantId,
    string ClientName,
    string TestimonialText,
    int Rating
);

public record TestimonialVisibilityRequest(bool IsVisible);