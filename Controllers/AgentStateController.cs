using Microsoft.AspNetCore.Mvc;
using AgentStateApi.DTOs;
using AgentStateApi.Services;
using AgentStateApi.Exceptions;

namespace AgentStateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentStateController : ControllerBase
{
    private readonly IAgentStateService _agentStateService;
    private readonly ILogger<AgentStateController> _logger;

    public AgentStateController(IAgentStateService agentStateService, ILogger<AgentStateController> logger)
    {
        _agentStateService = agentStateService;
        _logger = logger;
    }

    /// <summary>
    /// Processes call center agent events
    /// </summary>
    /// <param name="agentEvent">Agent event data</param>
    /// <returns>Processing result</returns>
    [HttpPost("events")]
    public async Task<IActionResult> ProcessAgentEvent([FromBody] AgentEventDto agentEvent)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for agent event: {ValidationErrors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                
                return BadRequest(ModelState);
            }

            await _agentStateService.ProcessAgentEventAsync(agentEvent);

            _logger.LogInformation("Agent event processed successfully for agent {AgentId}", agentEvent.AgentId);

            return Ok(new { 
                message = "Agent event processed successfully", 
                agentId = agentEvent.AgentId,
                timestamp = DateTime.UtcNow
            });
        }
        catch (LateEventException ex)
        {
            _logger.LogWarning("Late event received: {Message}", ex.Message);
            return BadRequest(new { error = "Late event", message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing agent event for agent {AgentId}", agentEvent?.AgentId);
            return StatusCode(500, new { error = "Internal server error", message = "An error occurred while processing the agent event" });
        }
    }
}
