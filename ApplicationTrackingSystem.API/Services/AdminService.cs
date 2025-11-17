using Microsoft.EntityFrameworkCore;
using ApplicationTrackingSystem.API.Data;
using ApplicationTrackingSystem.API.Models;
using ApplicationTrackingSystem.API.Models.DTOs;

namespace ApplicationTrackingSystem.API.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    
    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<JobRole?> CreateJobRoleAsync(int adminUserId, CreateJobRoleRequest request)
    {
        var jobRole = new JobRole
        {
            Title = request.Title,
            Description = request.Description,
            IsTechnical = request.IsTechnical,
            IsActive = true,
            CreatedByUserId = adminUserId,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.JobRoles.Add(jobRole);
        await _context.SaveChangesAsync();
        
        return jobRole;
    }
    
    public async Task<List<JobRole>> GetAllJobRolesAsync()
    {
        return await _context.JobRoles
            .Include(j => j.CreatedByUser)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<bool> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationRequest request, int adminUserId)
    {
        var application = await _context.Applications
            .Include(a => a.JobRole)
            .FirstOrDefaultAsync(a => a.Id == applicationId);
        
        if (application == null)
            return false;
        
        // Admin can only update non-technical applications
        if (application.JobRole != null && application.JobRole.IsTechnical)
            return false;
        
        var previousStatus = application.Status;
        application.Status = request.Status;
        application.UpdatedAt = DateTime.UtcNow;
        
        if (!string.IsNullOrEmpty(request.Comment))
        {
            application.Notes = string.IsNullOrEmpty(application.Notes) 
                ? request.Comment 
                : $"{application.Notes}\n\n{request.Comment}";
        }
        
        // Create activity log
        var activityLog = new ActivityLog
        {
            ApplicationId = application.Id,
            Action = "StatusChanged",
            PreviousStatus = previousStatus,
            NewStatus = request.Status,
            Comment = request.Comment ?? "Status updated by admin",
            PerformedByRole = "Admin",
            PerformedByUserId = adminUserId,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.ActivityLogs.Add(activityLog);
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<List<Application>> GetNonTechnicalApplicationsAsync()
    {
        return await _context.Applications
            .Include(a => a.JobRole)
            .Include(a => a.User)
            .Where(a => a.JobRole != null && !a.JobRole.IsTechnical)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}

