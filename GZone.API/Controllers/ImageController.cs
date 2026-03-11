using Asp.Versioning;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GZone.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ImageController : Controller
{
    //Dependency Injection
    private readonly IImageService _imageService;

    //Constructor
    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string fileName, [FromForm] string category)
    {
        try
        {
            var relativePath = await _imageService.SaveImageAsync(file, fileName, category);
            return Ok(new { success = true, path = relativePath });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{*relativePath}")]
    public IActionResult GetImage(string relativePath)
    {
        try
        {
            var (fileStream, contentType) = _imageService.GetImageFile(relativePath);
            // Trả về luồng file để trình duyệt có thể render trực tiếp hình ảnh
            return File(fileStream, contentType);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{*relativePath}")]
    public IActionResult DeleteImage(string relativePath)
    {
        try
        {
            bool isDeleted = _imageService.DeleteImage(relativePath);

            if (isDeleted)
                return Ok(new { success = true, message = "Xóa hình ảnh thành công." });

            return NotFound(new { success = false, message = "Hình ảnh không tồn tại." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}