using Microsoft.AspNetCore.Mvc;

namespace ConHacks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    internal readonly IConfiguration _configuration;

    public BaseController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}