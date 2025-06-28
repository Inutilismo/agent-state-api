using Microsoft.EntityFrameworkCore;
using AgentStateApi.Models;

namespace AgentStateApi.Data;

public class AgentStateDbContext : DbContext
{
    public AgentStateDbContext(DbContextOptions<AgentStateDbContext> options) : base(options)
    {
    }
    
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<AgentSkill> AgentSkills { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastUpdated).IsRequired();
            
            entity.HasIndex(e => e.Name);
        });
        
        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasIndex(e => e.Name).IsUnique();
        });
        
        modelBuilder.Entity<AgentSkill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AgentId).IsRequired();
            entity.Property(e => e.SkillId).IsRequired();
            entity.Property(e => e.AssignedAt).IsRequired();
            
            entity.HasOne(e => e.Agent)
                  .WithMany(a => a.AgentSkills)
                  .HasForeignKey(e => e.AgentId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Skill)
                  .WithMany(s => s.AgentSkills)
                  .HasForeignKey(e => e.SkillId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.AgentId, e.SkillId }).IsUnique();
        });
    }
}
