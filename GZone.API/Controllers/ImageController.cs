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
    private readonly IImageService _service;

    //Constructor
    public ImageController(IImageService service)
    {
        _service = service;
    }
}