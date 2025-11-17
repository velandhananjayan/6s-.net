using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ApplicationTrackingSystem.API.Models;
using ApplicationTrackingSystem.API.Models.DTOs;
using ApplicationTrackingSystem.API.Services;

namespace ApplicationTrackingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ApplicationsController> _logger;
    
    public ApplicationsController(IApplicationService applicationService, ILogger<ApplicationsController> logger)
    {
        _applicationService = applicationService;
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
    /// Create a new application (Applicant only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Applicant")]
    [ProducesResponseType(typeof(Application), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        try
        {
            var userId = GetUserId();
            var application = await _applicationService.CreateApplicationAsync(userId, request);
            
            if (application == null)
            {
                return BadRequest(new { message = "Invalid job role or job role is not active" });
            }
            
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application");
            return StatusCode(500, new { message = "An error occurred while creating the application" });
        }
    }
    
    /// <summary>
    /// Get all applications for the current user (Applicant) or all applications (Admin/BotMimic)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<Application>), 200)]
    public async Task<IActionResult> GetApplications()
    {
        try
        {
            var role = GetUserRole();
            var userId = GetUserId();
            
            if (role == "Applicant")
            {
                var applications = await _applicationService.GetUserApplicationsAsync(userId);
                return Ok(applications);
            }
            
            // Admin and BotMimic can see applications through their respective services
            return Ok(new { message = "Use Admin or BotMimic endpoints for viewing all applications" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applications");
            return StatusCode(500, new { message = "An error occurred while retrieving applications" });
        }
    }
    
    /// <summary>
    /// Get a specific application by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Application), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetApplication(int id)
    {
        try
        {
            var userId = GetUserId();
            var role = GetUserRole();
            
            var application = await _applicationService.GetApplicationByIdAsync(id, userId, role);
            
            if (application == null)
            {
                return NotFound(new { message = "Application not found or access denied" });
            }
            
            return Ok(application);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application");
            return StatusCode(500, new { message = "An error occurred while retrieving the application" });
        }
    }
    
    /// <summary>
    /// Get activity history for a specific application
    /// </summary>
    [HttpGet("{id}/history")]
    [ProducesResponseType(typeof(List<ActivityLog>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetApplicationHistory(int id)
    {
        try
        {
            var userId = GetUserId();
            var role = GetUserRole();
            
            var history = await _applicationService.GetApplicationHistoryAsync(id, userId, role);
            
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application history");
            return StatusCode(500, new { message = "An error occurred while retrieving the application history" });
        }
    }
}

