using System.ComponentModel.DataAnnotations;

namespace AgentStateApi.Models;

public class Skill
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<AgentSkill> AgentSkills { get; set; } = new List<AgentSkill>();
}