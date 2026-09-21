using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login()
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return NoContent();
    }
}
