using ApplicationTrackingSystem.API.Models;

namespace ApplicationTrackingSystem.API.Services;

public interface IBotMimicService
{
    Task<int> ProcessTechnicalApplicationsAsync();
    Task<bool> ProcessApplicationAsync(int applicationId);
}

