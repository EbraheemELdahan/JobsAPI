using Microsoft.AspNetCore.Mvc;
using JobsAPI.Models;
using JobsAPI.Services;

namespace JobsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _auth.ValidateCredentialsAsync(req.Email, req.Password);
        if (user == null) return Unauthorized(new { message = "Invalid credentials" });

        var token = _auth.GenerateToken(user);
        return Ok(new { token });
    }
}