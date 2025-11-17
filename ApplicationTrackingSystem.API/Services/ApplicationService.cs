using Microsoft.EntityFrameworkCore;
using ApplicationTrackingSystem.API.Data;
using ApplicationTrackingSystem.API.Models;
using ApplicationTrackingSystem.API.Models.DTOs;

namespace ApplicationTrackingSystem.API.Services;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;
    
    public ApplicationService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Application?> CreateApplicationAsync(int userId, CreateApplicationRequest request)
    {
        // Verify job role exists
        var jobRole = await _context.JobRoles.FindAsync(request.JobRoleId);
        if (jobRole == null || !jobRole.IsActive)
            return null;
        
        var application = new Application
        {
            UserId = userId,
            JobRoleId = request.JobRoleId,
            Status = "Applied",
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
        
        // Create activity log
        var activityLog = new ActivityLog
        {
            ApplicationId = application.Id,
            Action = "Created",
            NewStatus = "Applied",
            Comment = "Application submitted",
            PerformedByRole = "Applicant",
            PerformedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.ActivityLogs.Add(activityLog);
        await _context.SaveChangesAsync();
        
        return application;
    }
    
    public async Task<List<Application>> GetUserApplicationsAsync(int userId)
    {
        return await _context.Applications
            .Include(a => a.JobRole)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<Application?> GetApplicationByIdAsync(int applicationId, int? userId = null, string? role = null)
    {
        var query = _context.Applications
            .Include(a => a.JobRole)
            .Include(a => a.User)
            .AsQueryable();
        
        // Role-based access control
        if (role == "Applicant" && userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }
        else if (role == "Admin")
        {
            // Admin can see all applications
        }
        else if (role == "BotMimic")
        {
            // Bot Mimic can only see technical role applications
            query = query.Where(a => a.JobRole != null && a.JobRole.IsTechnical);
        }
        
        return await query.FirstOrDefaultAsync(a => a.Id == applicationId);
    }
    
    public async Task<List<ActivityLog>> GetApplicationHistoryAsync(int applicationId, int? userId = null, string? role = null)
    {
        // Verify access
        var application = await GetApplicationByIdAsync(applicationId, userId, role);
        if (application == null)
            return new List<ActivityLog>();
        
        return await _context.ActivityLogs
            .Include(al => al.PerformedByUser)
            .Where(al => al.ApplicationId == applicationId)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync();
    }
}

