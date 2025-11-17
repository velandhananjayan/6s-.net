using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ApplicationTrackingSystem.API.Models.DTOs;
using ApplicationTrackingSystem.API.Services;

namespace ApplicationTrackingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;
    
    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
    
    private string GetUserRole()
    {
        return User.FindFirstValue(ClaimTypes.Role) ?? "";
    }
    
    /// <summary>
    /// Get dashboard data based on user role
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), 200)]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var role = GetUserRole();
            DashboardResponse response;
            
            switch (role)
            {
                case "Applicant":
                    var userId = GetUserId();
                    response = await _dashboardService.GetApplicantDashboardAsync(userId);
                    break;
                case "BotMimic":
                    response = await _dashboardService.GetBotMimicDashboardAsync();
                    break;
                case "Admin":
                    response = await _dashboardService.GetAdminDashboardAsync();
                    break;
                default:
                    return Unauthorized(new { message = "Invalid role" });
            }
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard data");
            return StatusCode(500, new { message = "An error occurred while retrieving dashboard data" });
        }
    }
}

