using System.ComponentModel.DataAnnotations;

namespace AgentStateApi.Models;

public class Agent
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string State { get; set; } = "AVAILABLE";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<AgentSkill> AgentSkills { get; set; } = new List<AgentSkill>();
}
