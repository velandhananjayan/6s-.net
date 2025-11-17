using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ApplicationTrackingSystem.API.Data;
using ApplicationTrackingSystem.API.Models;
using ApplicationTrackingSystem.API.Models.DTOs;
using BCrypt.Net;

namespace ApplicationTrackingSystem.API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    
    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);
        
        if (user == null)
            return null;
        
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;
        
        var token = GenerateJwtToken(user);
        
        return new LoginResponse
        {
            Token = token,
            Role = user.Role,
            UserId = user.Id,
            Username = user.Username
        };
    }
    
    public string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!");
        var issuer = _configuration["Jwt:Issuer"] ?? "ApplicationTrackingSystem";
        var audience = _configuration["Jwt:Audience"] ?? "ApplicationTrackingSystem";
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

