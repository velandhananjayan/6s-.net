using ApplicationTrackingSystem.API.Models;
using ApplicationTrackingSystem.API.Models.DTOs;

namespace ApplicationTrackingSystem.API.Services;

public interface IAdminService
{
    Task<JobRole?> CreateJobRoleAsync(int adminUserId, CreateJobRoleRequest request);
    Task<List<JobRole>> GetAllJobRolesAsync();
    Task<bool> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationRequest request, int adminUserId);
    Task<List<Application>> GetNonTechnicalApplicationsAsync();
}

