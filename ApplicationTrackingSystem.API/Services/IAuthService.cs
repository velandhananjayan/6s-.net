using ApplicationTrackingSystem.API.Models;
using ApplicationTrackingSystem.API.Models.DTOs;

namespace ApplicationTrackingSystem.API.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    string GenerateJwtToken(User user);
}

