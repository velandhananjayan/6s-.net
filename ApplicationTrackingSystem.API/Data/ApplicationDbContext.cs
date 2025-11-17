using Microsoft.EntityFrameworkCore;
using ApplicationTrackingSystem.API.Models;

namespace ApplicationTrackingSystem.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<JobRole> JobRoles { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure relationships
        modelBuilder.Entity<Application>()
            .HasOne(a => a.User)
            .WithMany(u => u.Applications)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Application>()
            .HasOne(a => a.JobRole)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobRoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ActivityLog>()
            .HasOne(al => al.Application)
            .WithMany(a => a.ActivityLogs)
            .HasForeignKey(al => al.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ActivityLog>()
            .HasOne(al => al.PerformedByUser)
            .WithMany()
            .HasForeignKey(al => al.PerformedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<JobRole>()
            .HasOne(j => j.CreatedByUser)
            .WithMany()
            .HasForeignKey(j => j.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

