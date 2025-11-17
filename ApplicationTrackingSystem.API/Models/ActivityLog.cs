using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApplicationTrackingSystem.API.Models;

public class ActivityLog
{
    public int Id { get; set; }
    
    [Required]
    public int ApplicationId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // StatusChanged, CommentAdded, etc.
    
    [MaxLength(50)]
    public string? PreviousStatus { get; set; }
    
    [MaxLength(50)]
    public string? NewStatus { get; set; }
    
    [MaxLength(2000)]
    public string? Comment { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string PerformedByRole { get; set; } = string.Empty; // Applicant, BotMimic, Admin
    
    public int? PerformedByUserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [JsonIgnore]
    public Application? Application { get; set; }
    [JsonIgnore]
    public User? PerformedByUser { get; set; }
}

