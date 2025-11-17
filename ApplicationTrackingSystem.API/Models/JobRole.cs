using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApplicationTrackingSystem.API.Models;

public class JobRole
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public bool IsTechnical { get; set; } // Technical roles have automated tracking
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int CreatedByUserId { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public User? CreatedByUser { get; set; }
    [JsonIgnore]
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

