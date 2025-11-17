using Microsoft.EntityFrameworkCore;
using ApplicationTrackingSystem.API.Data;
using ApplicationTrackingSystem.API.Models;

namespace ApplicationTrackingSystem.API.Services;

public class BotMimicService : IBotMimicService
{
    private readonly ApplicationDbContext _context;
    private readonly Random _random = new();
    
    // Status workflow for technical roles
    private readonly string[] _statusWorkflow = { "Applied", "Reviewed", "Interview", "Offer", "Rejected" };
    
    private readonly Dictionary<string, string[]> _statusComments = new()
    {
        { "Applied", new[] { "Application received and queued for review.", "Your application has been successfully submitted." } },
        { "Reviewed", new[] { "Application reviewed. Moving to interview stage.", "Your qualifications have been reviewed. Interview scheduled." } },
        { "Interview", new[] { "Technical interview completed. Evaluation in progress.", "Interview conducted. Awaiting feedback." } },
        { "Offer", new[] { "Congratulations! We are pleased to extend an offer.", "Offer letter prepared. Details to follow." } },
        { "Rejected", new[] { "Thank you for your interest. We have decided to move forward with other candidates." } }
    };
    
    public BotMimicService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> ProcessTechnicalApplicationsAsync()
    {
        // Get all technical role applications that are not in final states
        var applications = await _context.Applications
            .Include(a => a.JobRole)
            .Where(a => a.JobRole != null && 
                       a.JobRole.IsTechnical && 
                       a.Status != "Offer" && 
                       a.Status != "Rejected")
            .ToListAsync();
        
        int processedCount = 0;
        
        foreach (var application in applications)
        {
            // Randomly decide if this application should be processed (70% chance)
            if (_random.Next(100) < 70)
            {
                await ProcessApplicationAsync(application.Id);
                processedCount++;
            }
        }
        
        return processedCount;
    }
    
    public async Task<bool> ProcessApplicationAsync(int applicationId)
    {
        var application = await _context.Applications
            .Include(a => a.JobRole)
            .FirstOrDefaultAsync(a => a.Id == applicationId);
        
        if (application == null || application.JobRole == null || !application.JobRole.IsTechnical)
            return false;
        
        // Get current status index
        var currentIndex = Array.IndexOf(_statusWorkflow, application.Status);
        if (currentIndex < 0 || currentIndex >= _statusWorkflow.Length - 1)
            return false;
        
        // Move to next status
        var previousStatus = application.Status;
        var newStatus = _statusWorkflow[currentIndex + 1];
        application.Status = newStatus;
        application.UpdatedAt = DateTime.UtcNow;
        
        // Get random comment for this status
        var comments = _statusComments[newStatus];
        var comment = comments[_random.Next(comments.Length)];
        
        // Create activity log
        var activityLog = new ActivityLog
        {
            ApplicationId = application.Id,
            Action = "StatusChanged",
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            Comment = comment,
            PerformedByRole = "BotMimic",
            PerformedByUserId = null, // Bot Mimic doesn't have a user ID
            CreatedAt = DateTime.UtcNow
        };
        
        _context.ActivityLogs.Add(activityLog);
        await _context.SaveChangesAsync();
        
        return true;
    }
}

