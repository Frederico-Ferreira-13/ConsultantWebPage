using ConsultantWebPage.Contracts.Interfaces.Services;
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

    // GET api/testimonials/consultant/{consultantId}
    // Lista testemunhos aprovados e visíveis → GET público, sem autenticação
    // Um visitante pode ver os testemunhos de um consultor sem ter conta
    [HttpGet("consultant/{consultantId:int}")]
    public async Task<IActionResult> GetApproved(int consultantId)
    {
        var result = await _testimonialsService.GetApprovedTestimonialsAsync(consultantId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    // GET api/testimonials/consultant/{consultantId}/moderation
    // Lista TODOS os testemunhos para moderação → GET com sub-rota descritiva
    // "moderation" deixa claro que esta rota devolve também os não visíveis
    // Requer autenticação — só o consultor ou admin deve ver os pendentes
    [Authorize]
    [HttpGet("consultant/{consultantId:int}/moderation")]
    public async Task<IActionResult> GetAllForModeration(int consultantId)
    {
        var result = await _testimonialsService.GetAllTestimonialsForModerationAsync(consultantId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    // POST api/testimonials
    // Submissão de novo testemunho → POST sem autenticação
    // Um cliente anónimo pode submeter um testemunho — fica invisível até ser aprovado
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

    // PATCH api/testimonials/{id}/visibility
    // Altera a visibilidade → PATCH porque só um campo muda (IsVisible)
    // É a ação de aprovação/rejeição pelo consultor ou admin
    [Authorize]
    [HttpPatch("{id:int}/visibility")]
    public async Task<IActionResult> UpdateVisibility(int id, [FromBody] TestimonialVisibilityRequest request)
    {
        var result = await _testimonialsService.UpdateVisibilityAsync(id, request.IsVisible);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — alteração bem sucedida sem necessidade de devolver o recurso
    }

    // DELETE api/testimonials/{id}
    // Eliminação → DELETE + id na rota, retorna 204
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _testimonialsService.DeleteTestimonialAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — eliminado com sucesso, sem body
    }
}

public record SubmitTestimonialRequest(
    int ConsultantId,
    string ClientName,
    string TestimonialText,
    int Rating
);

public record TestimonialVisibilityRequest(bool IsVisible);