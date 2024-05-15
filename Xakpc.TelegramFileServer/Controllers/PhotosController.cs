using Microsoft.AspNetCore.Mvc;
using Xakpc.TelegramFileServer.Services;

namespace Xakpc.TelegramFileServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotosController : ControllerBase
{
    private readonly IStorageService _storageService;
    private readonly IHttpClientFactory _httpClientFactory;

    public PhotosController(IHttpClientFactory httpClientFactory, IStorageService storageService)
    {
        _storageService = storageService;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile photo,
        [FromHeader(Name = "Chat-Id")] string chatId,
        [FromHeader(Name = "Bot-Token")] string botToken,        
        CancellationToken cancellationToken)
    {
        if (photo == null || photo.Length == 0)
            return BadRequest("No image uploaded.");

        var fileId = await _storageService.UploadImageAsync(
            new TelegramCredentials(botToken, chatId),
            photo, cancellationToken);

        return Created($"api/v1/photos/{fileId}", fileId);
    }

    [HttpGet("{fileId}")]
    public async Task<IActionResult> DownloadImage(string fileId, 
        [FromHeader(Name = "Bot-Token")] string? botToken,
        [FromQuery(Name = "botToken")] string? botTokenQuery)
    {
        botToken ??= botTokenQuery;
        
        if (botToken == null)
        {
            return BadRequest("Bot token is required.");
        }

        var imageUrl = await _storageService.GetImageUrlAsync(
            new TelegramCredentials(botToken, default), fileId);

        if (imageUrl == null)
            return NotFound();

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(imageUrl);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        var contentType = GetContentType(Path.GetExtension(imageUrl));
        var imageStream = await response.Content.ReadAsStreamAsync();
        return File(imageStream, contentType);
    }

    private string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".tiff" => "image/tiff",
            ".svg" => "image/svg+xml",

            ".webp" => "image/webp",
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            ".ogg" => "video/ogg",

            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            _ => "application/octet-stream",
        };
    }
}

