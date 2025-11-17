using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ApplicationTrackingSystem.API.Models;
using ApplicationTrackingSystem.API.Models.DTOs;
using ApplicationTrackingSystem.API.Services;

namespace ApplicationTrackingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;
    
    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
    
    /// <summary>
    /// Create a new job role
    /// </summary>
    [HttpPost("job-roles")]
    [ProducesResponseType(typeof(JobRole), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateJobRole([FromBody] CreateJobRoleRequest request)
    {
        try
        {
            var adminUserId = GetUserId();
            var jobRole = await _adminService.CreateJobRoleAsync(adminUserId, request);
            
            if (jobRole == null)
            {
                return BadRequest(new { message = "Failed to create job role" });
            }
            
            return CreatedAtAction(nameof(GetJobRoles), new { id = jobRole.Id }, jobRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job role");
            return StatusCode(500, new { message = "An error occurred while creating the job role" });
        }
    }
    
    /// <summary>
    /// Get all job roles
    /// </summary>
    [HttpGet("job-roles")]
    [ProducesResponseType(typeof(List<JobRole>), 200)]
    public async Task<IActionResult> GetJobRoles()
    {
        try
        {
            var jobRoles = await _adminService.GetAllJobRolesAsync();
            return Ok(jobRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job roles");
            return StatusCode(500, new { message = "An error occurred while retrieving job roles" });
        }
    }
    
    /// <summary>
    /// Get all non-technical applications
    /// </summary>
    [HttpGet("applications")]
    [ProducesResponseType(typeof(List<Application>), 200)]
    public async Task<IActionResult> GetNonTechnicalApplications()
    {
        try
        {
            var applications = await _adminService.GetNonTechnicalApplicationsAsync();
            return Ok(applications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applications");
            return StatusCode(500, new { message = "An error occurred while retrieving applications" });
        }
    }
    
    /// <summary>
    /// Update application status (non-technical roles only)
    /// </summary>
    [HttpPut("applications/{applicationId}/status")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateApplicationStatus(int applicationId, [FromBody] UpdateApplicationRequest request)
    {
        try
        {
            var adminUserId = GetUserId();
            var success = await _adminService.UpdateApplicationStatusAsync(applicationId, request, adminUserId);
            
            if (!success)
            {
                return BadRequest(new { message = "Application not found, is a technical role, or update failed" });
            }
            
            return Ok(new { message = "Application status updated successfully", applicationId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application status");
            return StatusCode(500, new { message = "An error occurred while updating the application status" });
        }
    }
}

