using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationTrackingSystem.API.Services;

namespace ApplicationTrackingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "BotMimic")]
public class BotMimicController : ControllerBase
{
    private readonly IBotMimicService _botMimicService;
    private readonly ILogger<BotMimicController> _logger;
    
    public BotMimicController(IBotMimicService botMimicService, ILogger<BotMimicController> logger)
    {
        _botMimicService = botMimicService;
        _logger = logger;
    }
    
    /// <summary>
    /// Process all technical role applications automatically
    /// </summary>
    [HttpPost("process-all")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> ProcessAllTechnicalApplications()
    {
        try
        {
            var processedCount = await _botMimicService.ProcessTechnicalApplicationsAsync();
            
            return Ok(new 
            { 
                message = $"Processed {processedCount} technical applications",
                processedCount 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing technical applications");
            return StatusCode(500, new { message = "An error occurred while processing applications" });
        }
    }
    
    /// <summary>
    /// Process a specific technical application
    /// </summary>
    [HttpPost("process/{applicationId}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ProcessApplication(int applicationId)
    {
        try
        {
            var success = await _botMimicService.ProcessApplicationAsync(applicationId);
            
            if (!success)
            {
                return BadRequest(new { message = "Application not found, not a technical role, or already in final state" });
            }
            
            return Ok(new { message = "Application processed successfully", applicationId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing application");
            return StatusCode(500, new { message = "An error occurred while processing the application" });
        }
    }
}

