using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentsServices _commentsService;    

    public CommentsController(ICommentsServices commentsServices)
    {
        _commentsService = commentsServices;
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _commentsService.GetCommentsByIdAsync(id);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    [HttpGet("realestate/{realEstateId:int}")]
    public async Task<IActionResult> GetByRealEstate(int realEstateId)
    {
        var result = await _commentsService.GetCommentsByRealEstateIdAsync(realEstateId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
    {
        // A validação e construção ficam no domínio (construtor da entidade)
        Comments newComment;
        try
        {
            newComment = new Comments(
                request.RealEstateId,
                request.UserId,
                request.CommentText,
                request.ParentCommentId
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _commentsService.CreateCommentAsync(newComment);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);
        
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.CommentsId }, result.Value);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentRequest request)
    {
        var currentResult = await _commentsService.GetCommentsByIdAsync(id);
        if (!currentResult.IsSuccessful)
            return NotFound("Comentário não encontrado.");

        var comment = currentResult.Value!;
        
        try
        {
            comment.UpdateComment(request.NewCommentText);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _commentsService.UpdateCommentAsync(id, comment);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return Ok(comment);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _commentsService.DeleteCommentAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent(); // 204 — sucesso sem body, padrão para DELETE
    }
}

    public record CreateCommentRequest(
    int RealEstateId,
    int UserId,
    string CommentText,
    int? ParentCommentId = null
);

public record UpdateCommentRequest(string NewCommentText);