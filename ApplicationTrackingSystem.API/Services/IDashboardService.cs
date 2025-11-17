using ApplicationTrackingSystem.API.Models.DTOs;

namespace ApplicationTrackingSystem.API.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetApplicantDashboardAsync(int userId);
    Task<DashboardResponse> GetBotMimicDashboardAsync();
    Task<DashboardResponse> GetAdminDashboardAsync();
}

