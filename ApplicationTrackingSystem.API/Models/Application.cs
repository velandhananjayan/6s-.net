using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationTrackingSystem.API.Models;

public class Application
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public int JobRoleId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Applied"; // Applied, Reviewed, Interview, Offer, Rejected
    
    [MaxLength(2000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User? User { get; set; }
    public JobRole? JobRole { get; set; }
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}

