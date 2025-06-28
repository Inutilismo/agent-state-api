using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AgentStateApi.DTOs;

public class AgentEventDto
{
    [Required]
    [JsonPropertyName("agentId")]
    public Guid AgentId { get; set; }
    
    [Required]
    [MaxLength(100)]
    [JsonPropertyName("agentName")]
    public string AgentName { get; set; } = string.Empty;
    
    [Required]
    [JsonPropertyName("timestampUtc")]
    public DateTime TimestampUtc { get; set; }
    
    [Required]
    [MaxLength(50)]
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;
    
    [JsonPropertyName("queueIds")]
    public List<Guid> QueueIds { get; set; } = new List<Guid>();
}