using Microsoft.EntityFrameworkCore;
using ApplicationTrackingSystem.API.Data;
using ApplicationTrackingSystem.API.Models.DTOs;

namespace ApplicationTrackingSystem.API.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    
    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<DashboardResponse> GetApplicantDashboardAsync(int userId)
    {
        var applications = await _context.Applications
            .Include(a => a.JobRole)
            .Where(a => a.UserId == userId)
            .ToListAsync();
        
        var response = new DashboardResponse
        {
            TotalApplications = applications.Count,
            PendingApplications = applications.Count(a => a.Status == "Applied"),
            InProgressApplications = applications.Count(a => a.Status == "Reviewed" || a.Status == "Interview"),
            CompletedApplications = applications.Count(a => a.Status == "Offer"),
            RejectedApplications = applications.Count(a => a.Status == "Rejected")
        };
        
        // Status breakdown
        response.StatusBreakdown = applications
            .GroupBy(a => a.Status)
            .ToDictionary(g => g.Key, g => g.Count());
        
        // Role breakdown
        response.RoleBreakdown = applications
            .Where(a => a.JobRole != null)
            .GroupBy(a => a.JobRole!.Title)
            .ToDictionary(g => g.Key, g => g.Count());
        
        // Recent activities
        var recentLogs = await _context.ActivityLogs
            .Include(al => al.Application)
                .ThenInclude(a => a!.JobRole)
            .Where(al => al.Application != null && al.Application.UserId == userId)
            .OrderByDescending(al => al.CreatedAt)
            .Take(10)
            .ToListAsync();
        
        response.RecentActivities = recentLogs.Select(al => new RecentActivity
        {
            ApplicationId = al.ApplicationId,
            JobRoleTitle = al.Application?.JobRole?.Title ?? "Unknown",
            Status = al.NewStatus ?? al.Application?.Status ?? "Unknown",
            Action = al.Action,
            PerformedBy = al.PerformedByRole,
            CreatedAt = al.CreatedAt
        }).ToList();
        
        return response;
    }
    
    public async Task<DashboardResponse> GetBotMimicDashboardAsync()
    {
        var applications = await _context.Applications
            .Include(a => a.JobRole)
            .Where(a => a.JobRole != null && a.JobRole.IsTechnical)
            .ToListAsync();
        
        var response = new DashboardResponse
        {
            TotalApplications = applications.Count,
            PendingApplications = applications.Count(a => a.Status == "Applied"),
            InProgressApplications = applications.Count(a => a.Status == "Reviewed" || a.Status == "Interview"),
            CompletedApplications = applications.Count(a => a.Status == "Offer"),
            RejectedApplications = applications.Count(a => a.Status == "Rejected")
        };
        
        response.StatusBreakdown = applications
            .GroupBy(a => a.Status)
            .ToDictionary(g => g.Key, g => g.Count());
        
        response.RoleBreakdown = applications
            .Where(a => a.JobRole != null)
            .GroupBy(a => a.JobRole!.Title)
            .ToDictionary(g => g.Key, g => g.Count());
        
        var recentLogs = await _context.ActivityLogs
            .Include(al => al.Application)
                .ThenInclude(a => a!.JobRole)
            .Where(al => al.PerformedByRole == "BotMimic" && al.Application != null && al.Application.JobRole != null && al.Application.JobRole.IsTechnical)
            .OrderByDescending(al => al.CreatedAt)
            .Take(10)
            .ToListAsync();
        
        response.RecentActivities = recentLogs.Select(al => new RecentActivity
        {
            ApplicationId = al.ApplicationId,
            JobRoleTitle = al.Application?.JobRole?.Title ?? "Unknown",
            Status = al.NewStatus ?? al.Application?.Status ?? "Unknown",
            Action = al.Action,
            PerformedBy = al.PerformedByRole,
            CreatedAt = al.CreatedAt
        }).ToList();
        
        return response;
    }
    
    public async Task<DashboardResponse> GetAdminDashboardAsync()
    {
        var applications = await _context.Applications
            .Include(a => a.JobRole)
            .Where(a => a.JobRole != null && !a.JobRole.IsTechnical)
            .ToListAsync();
        
        var response = new DashboardResponse
        {
            TotalApplications = applications.Count,
            PendingApplications = applications.Count(a => a.Status == "Applied"),
            InProgressApplications = applications.Count(a => a.Status == "Reviewed" || a.Status == "Interview"),
            CompletedApplications = applications.Count(a => a.Status == "Offer"),
            RejectedApplications = applications.Count(a => a.Status == "Rejected")
        };
        
        response.StatusBreakdown = applications
            .GroupBy(a => a.Status)
            .ToDictionary(g => g.Key, g => g.Count());
        
        response.RoleBreakdown = applications
            .Where(a => a.JobRole != null)
            .GroupBy(a => a.JobRole!.Title)
            .ToDictionary(g => g.Key, g => g.Count());
        
        var recentLogs = await _context.ActivityLogs
            .Include(al => al.Application)
                .ThenInclude(a => a!.JobRole)
            .Where(al => al.PerformedByRole == "Admin" && al.Application != null && al.Application.JobRole != null && !al.Application.JobRole.IsTechnical)
            .OrderByDescending(al => al.CreatedAt)
            .Take(10)
            .ToListAsync();
        
        response.RecentActivities = recentLogs.Select(al => new RecentActivity
        {
            ApplicationId = al.ApplicationId,
            JobRoleTitle = al.Application?.JobRole?.Title ?? "Unknown",
            Status = al.NewStatus ?? al.Application?.Status ?? "Unknown",
            Action = al.Action,
            PerformedBy = al.PerformedByRole,
            CreatedAt = al.CreatedAt
        }).ToList();
        
        return response;
    }
}

