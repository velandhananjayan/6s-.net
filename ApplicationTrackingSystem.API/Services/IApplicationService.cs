using ApplicationTrackingSystem.API.Models;
using ApplicationTrackingSystem.API.Models.DTOs;

namespace ApplicationTrackingSystem.API.Services;

public interface IApplicationService
{
    Task<Application?> CreateApplicationAsync(int userId, CreateApplicationRequest request);
    Task<List<Application>> GetUserApplicationsAsync(int userId);
    Task<Application?> GetApplicationByIdAsync(int applicationId, int? userId = null, string? role = null);
    Task<List<ActivityLog>> GetApplicationHistoryAsync(int applicationId, int? userId = null, string? role = null);
}

