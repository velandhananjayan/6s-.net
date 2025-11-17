using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ApplicationTrackingSystem.API.Data;
using ApplicationTrackingSystem.API.Models;
using ApplicationTrackingSystem.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Application Tracking System API",
        Version = "v1",
        Description = "Hybrid Application Tracking System with role-based access control",
        Contact = new OpenApiContact
        {
            Name = "Application Tracking System",
            Email = "support@ats.com"
        }
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Database - Supports both PostgreSQL and SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var useSqlServer = builder.Configuration.GetValue<bool>("Database:UseSqlServer", false);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlServer)
    {
        // Use SQL Server
        options.UseSqlServer(connectionString ?? "Server=localhost;Database=ApplicationTrackingSystem;Trusted_Connection=True;TrustServerCertificate=True;");
    }
    else
    {
        // Use PostgreSQL (default)
        options.UseNpgsql(connectionString ?? "Host=localhost;Database=ApplicationTrackingSystem;Username=postgres;Password=postgres;Port=5432");
    }
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ApplicationTrackingSystem";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ApplicationTrackingSystem";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IBotMimicService, BotMimicService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Register Background Service for scheduled Bot Mimic processing
builder.Services.AddHostedService<BotMimicBackgroundService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Application Tracking System API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    
    // Seed initial data
    await SeedDataAsync(dbContext);
}

app.Run();

// Seed initial data
static async Task SeedDataAsync(ApplicationDbContext context)
{
    if (context.Users.Any())
        return; // Data already seeded
    
    // Create sample users
    var applicant = new User
    {
        Username = "applicant",
        Email = "applicant@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("applicant123"),
        Role = "Applicant",
        CreatedAt = DateTime.UtcNow
    };
    
    var botMimic = new User
    {
        Username = "botmimic",
        Email = "botmimic@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("botmimic123"),
        Role = "BotMimic",
        CreatedAt = DateTime.UtcNow
    };
    
    var admin = new User
    {
        Username = "admin",
        Email = "admin@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
        Role = "Admin",
        CreatedAt = DateTime.UtcNow
    };
    
    context.Users.AddRange(applicant, botMimic, admin);
    await context.SaveChangesAsync();
    
    // Create sample job roles
    var technicalRole1 = new JobRole
    {
        Title = "Senior Software Engineer",
        Description = "Full-stack development role requiring 5+ years of experience",
        IsTechnical = true,
        IsActive = true,
        CreatedByUserId = admin.Id,
        CreatedAt = DateTime.UtcNow
    };
    
    var technicalRole2 = new JobRole
    {
        Title = "DevOps Engineer",
        Description = "Cloud infrastructure and CI/CD pipeline management",
        IsTechnical = true,
        IsActive = true,
        CreatedByUserId = admin.Id,
        CreatedAt = DateTime.UtcNow
    };
    
    var nonTechnicalRole1 = new JobRole
    {
        Title = "HR Manager",
        Description = "Human resources management and recruitment",
        IsTechnical = false,
        IsActive = true,
        CreatedByUserId = admin.Id,
        CreatedAt = DateTime.UtcNow
    };
    
    var nonTechnicalRole2 = new JobRole
    {
        Title = "Marketing Specialist",
        Description = "Digital marketing and brand management",
        IsTechnical = false,
        IsActive = true,
        CreatedByUserId = admin.Id,
        CreatedAt = DateTime.UtcNow
    };
    
    context.JobRoles.AddRange(technicalRole1, technicalRole2, nonTechnicalRole1, nonTechnicalRole2);
    await context.SaveChangesAsync();
    
    // Create sample applications
    var app1 = new Application
    {
        UserId = applicant.Id,
        JobRoleId = technicalRole1.Id,
        Status = "Applied",
        Notes = "Interested in this position",
        CreatedAt = DateTime.UtcNow.AddDays(-5)
    };
    
    var app2 = new Application
    {
        UserId = applicant.Id,
        JobRoleId = nonTechnicalRole1.Id,
        Status = "Applied",
        Notes = "Looking forward to contributing",
        CreatedAt = DateTime.UtcNow.AddDays(-3)
    };
    
    context.Applications.AddRange(app1, app2);
    await context.SaveChangesAsync();
    
    // Create initial activity logs
    var log1 = new ActivityLog
    {
        ApplicationId = app1.Id,
        Action = "Created",
        NewStatus = "Applied",
        Comment = "Application submitted",
        PerformedByRole = "Applicant",
        PerformedByUserId = applicant.Id,
        CreatedAt = app1.CreatedAt
    };
    
    var log2 = new ActivityLog
    {
        ApplicationId = app2.Id,
        Action = "Created",
        NewStatus = "Applied",
        Comment = "Application submitted",
        PerformedByRole = "Applicant",
        PerformedByUserId = applicant.Id,
        CreatedAt = app2.CreatedAt
    };
    
    context.ActivityLogs.AddRange(log1, log2);
    await context.SaveChangesAsync();
}
