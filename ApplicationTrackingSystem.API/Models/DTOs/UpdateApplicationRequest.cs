using System.ComponentModel.DataAnnotations;

namespace ApplicationTrackingSystem.API.Models.DTOs;

public class UpdateApplicationRequest
{
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Comment { get; set; }
}

