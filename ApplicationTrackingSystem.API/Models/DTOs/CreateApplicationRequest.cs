using System.ComponentModel.DataAnnotations;

namespace ApplicationTrackingSystem.API.Models.DTOs;

public class CreateApplicationRequest
{
    [Required]
    public int JobRoleId { get; set; }
    
    [MaxLength(2000)]
    public string? Notes { get; set; }
}

