using System.ComponentModel.DataAnnotations;

namespace ApplicationTrackingSystem.API.Models.DTOs;

public class CreateJobRoleRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public bool IsTechnical { get; set; }
}

