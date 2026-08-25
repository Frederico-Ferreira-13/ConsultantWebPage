using System.Linq;
using ConsultantWebPage.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantWebPage.API.Controllers;

[ApiController]
[Route("api/[Controller]")]
[Authorize]
public class MediaUploadController : ControllerBase
{
    private readonly ICloudService _cloudService;

    private const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10MB
    private const long MaxVideoSizeBytes = 100 * 1024 * 1024; // 100MB

    private static readonly string [] AllowedImageContentTypes =
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private static readonly string[] AllowedVideoContentTypes =
    {
        "video/mp4", "video/webm", "video/quicktime"
    };

    public MediaUploadController(ICloudService cloudService)
    {
        _cloudService = cloudService;
    }

    [HttpPost("image")]
    [RequestSizeLimit(MaxImageSizeBytes)]
    public async Task<ActionResult> UploadImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Nenhum ficheiro de imagem foi enviado.");
        }

        if (!AllowedImageContentTypes.Contains(file.ContentType))
        {
            return BadRequest("Tipo de imagem não suportado. Formatos aceites: JPEG, PNG, WebP.");
        }

        if (file.Length > MaxImageSizeBytes)
        {
            return BadRequest("A imagem excede o tamanho máximo permitido (10MB).");
        }

        var url = await _cloudService.UploadImageAsync(file);
        if (string.IsNullOrWhiteSpace(url))
        {
            return StatusCode(500, "Não foi possível concluir o upload da imagem.");
        }

        return Ok(new { url });
    }

    [HttpPost("video")]
    [RequestSizeLimit(MaxVideoSizeBytes)]
    public async Task<IActionResult> UploadVideo( IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Nenhum ficheiro de vídeo foi enviado.");
 
        if (!AllowedVideoContentTypes.Contains(file.ContentType))
            return BadRequest("Tipo de vídeo não suportado. Formatos aceites: MP4, WebM, MOV.");
 
        if (file.Length > MaxVideoSizeBytes)
            return BadRequest("O vídeo excede o tamanho máximo permitido (100MB).");
 
        var url = await _cloudService.UploadVideoAsync(file);
        if (string.IsNullOrWhiteSpace(url))
            return StatusCode(500, "Não foi possível concluir o upload do vídeo.");
 
        return Ok(new { url });
    }
}