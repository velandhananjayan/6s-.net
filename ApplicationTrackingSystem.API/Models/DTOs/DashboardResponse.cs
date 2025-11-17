namespace ApplicationTrackingSystem.API.Models.DTOs;

public class DashboardResponse
{
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int InProgressApplications { get; set; }
    public int CompletedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public Dictionary<string, int> StatusBreakdown { get; set; } = new();
    public Dictionary<string, int> RoleBreakdown { get; set; } = new();
    public List<RecentActivity> RecentActivities { get; set; } = new();
}

public class RecentActivity
{
    public int ApplicationId { get; set; }
    public string JobRoleTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

