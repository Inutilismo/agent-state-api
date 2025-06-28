using AgentStateApi.Data;
using AgentStateApi.DTOs;
using AgentStateApi.Exceptions;
using AgentStateApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AgentStateApi.Services;

public interface IAgentStateService
{
    Task ProcessAgentEventAsync(AgentEventDto agentEvent);
}

public class AgentStateService : IAgentStateService
{
    private readonly AgentStateDbContext _context;
    private readonly ILogger<AgentStateService> _logger;
    
    public AgentStateService(AgentStateDbContext context, ILogger<AgentStateService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task ProcessAgentEventAsync(AgentEventDto agentEvent)
    {
        ValidateEventTimestamp(agentEvent.TimestampUtc);
        
        var newState = CalculateAgentState(agentEvent.Action, agentEvent.TimestampUtc);
        
        var agent = await GetOrCreateAgentAsync(agentEvent.AgentId, agentEvent.AgentName);
        
        agent.State = newState;
        agent.Name = agentEvent.AgentName;
        agent.LastUpdated = DateTime.UtcNow;
        
        await SynchronizeAgentSkillsAsync(agent.Id, agentEvent.QueueIds);
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Agent {AgentId} state updated to {State} with {SkillCount} skills",
            agentEvent.AgentId, newState, agentEvent.QueueIds.Count);
    }
    
    private void ValidateEventTimestamp(DateTime eventTimestamp)
    {
        var currentTime = DateTime.UtcNow;
        var timeDifference = currentTime - eventTimestamp;
        
        if (timeDifference.TotalHours > 1)
        {
            throw new LateEventException(eventTimestamp, currentTime);
        }
    }
    
    private string CalculateAgentState(string action, DateTime timestamp)
    {
        if (action == "START_DO_NOT_DISTURB")
        {
            var hour = timestamp.Hour;
            if (hour >= 11 && hour < 13)
            {
                return "ON_LUNCH";
            }
            
            return "DO_NOT_DISTURB";
        }
        
        if (action == "CALL_STARTED")
        {
            return "ON_CALL";
        }
        
        return "AVAILABLE";
    }
    
    private async Task<Agent> GetOrCreateAgentAsync(Guid agentId, string agentName)
    {
        var agent = await _context.Agents.FindAsync(agentId);
        
        if (agent == null)
        {
            agent = new Agent
            {
                Id = agentId,
                Name = agentName,
                State = "AVAILABLE"
            };
            
            _context.Agents.Add(agent);
        }
        
        return agent;
    }
    
    private async Task SynchronizeAgentSkillsAsync(Guid agentId, List<Guid> queueIds)
    {
        var currentSkills = await _context.AgentSkills
            .Where(x => x.AgentId == agentId)
            .ToListAsync();
        
        var skillsToRemove = currentSkills.Where(x => !queueIds.Contains(x.SkillId)).ToList();
        
        var currentSkillIds = currentSkills.Select(x => x.SkillId).ToList();
        var skillsToAdd = queueIds.Where(x => !currentSkillIds.Contains(x)).ToList();
        
        if (skillsToRemove.Any())
        {
            _context.AgentSkills.RemoveRange(skillsToRemove);
        }
        
        foreach (var skillId in skillsToAdd)
        {
            await GetOrCreateSkillAsync(skillId);
            
            var agentSkill = new AgentSkill
            {
                AgentId = agentId,
                SkillId = skillId,
                AssignedAt = DateTime.UtcNow
            };
            
            _context.AgentSkills.Add(agentSkill);
        }
    }
    
    private async Task<Skill> GetOrCreateSkillAsync(Guid skillId)
    {
        var skill = await _context.Skills.FindAsync(skillId);
        
        if (skill == null)
        {
            skill = new Skill
            {
                Id = skillId,
                Name = $"Queue-{skillId.ToString()[..8]}",
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Skills.Add(skill);
        }
        
        return skill;
    }
}