using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgentStateApi.Models;

public class AgentSkill
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public Guid AgentId { get; set; }
    
    [Required]
    public Guid SkillId { get; set; }
    
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("AgentId")]
    public virtual Agent Agent { get; set; } = null!;
    
    [ForeignKey("SkillId")]
    public virtual Skill Skill { get; set; } = null!;
}