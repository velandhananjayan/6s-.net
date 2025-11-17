using Microsoft.AspNetCore.Mvc;
using ApplicationTrackingSystem.API.Models.DTOs;
using ApplicationTrackingSystem.API.Services;

namespace ApplicationTrackingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    /// <summary>
    /// User login endpoint
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }
}

