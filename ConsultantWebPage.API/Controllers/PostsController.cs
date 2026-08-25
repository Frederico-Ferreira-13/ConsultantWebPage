using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostsServices _postsService;

    public PostsController(IPostsServices postsService)
    {
        _postsService = postsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _postsService.GetAllPostsAsync();
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest([FromQuery] int count = 5)
    {
        if (count <= 0)
            return BadRequest("O número de posts deve ser positivo.");

        var result = await _postsService.GetLatestBlogPostsAsync(count);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return BadRequest("O slug não pode estar vazio.");

        var result = await _postsService.GetPostBySlugAsync(slug);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return NotFound(result.Message);
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var result = await _postsService.GetPostsByCategoryAsync(categoryId);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest("O termo de pesquisa não pode estar vazio.");

        var result = await _postsService.SearchPostsAsync(term);
        if (result.IsSuccessful)
            return Ok(result.Value);

        return BadRequest(result.Message);
    }

    [HttpPost("{id:int}/view")]
    public async Task<IActionResult> IncrementView(int id)
    {
        var result = await _postsService.IncrementPostViewAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
    {
        Posts newPost;
        try
        {
            newPost = new Posts(
                request.AuthorId,
                request.CategoryId,
                request.Title,
                request.Slug,
                request.Content,
                request.Summary,
                request.ImageUrl ?? string.Empty,
                request.Status ?? "Draft"
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _postsService.CreatePostAsync(newPost);
        if (!result.IsSuccessful)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Value!.Slug }, result.Value);
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest request)
    {
        var currentResult = await _postsService.GetPostBySlugAsync(request.Slug);

        Posts postToUpdate;
        try
        {
            postToUpdate = new Posts(
                request.AuthorId,
                request.CategoryId,
                request.Title,
                request.Slug,
                request.Content,
                request.Summary,
                request.ImageUrl ?? string.Empty,
                request.IsPublished ? "Published" : "Draft"
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var result = await _postsService.UpdatePostAsync(id, postToUpdate);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return Ok(result.Value);
    }

    [Authorize(Roles = AppRoles.AdminOrConsultant)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _postsService.DeletePostAsync(id);
        if (!result.IsSuccessful)
            return NotFound(result.Message);

        return NoContent();
    }
}

public record CreatePostRequest(
    int AuthorId,
    int CategoryId,
    string Title,
    string Slug,
    string Content,
    string Summary,
    string? ImageUrl,
    string? Status
);

public record UpdatePostRequest(
    int AuthorId,
    int CategoryId,
    string Title,
    string Slug,
    string Content,
    string Summary,
    string? ImageUrl,
    bool IsPublished
);