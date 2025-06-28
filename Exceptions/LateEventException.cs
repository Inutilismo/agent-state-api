namespace AgentStateApi.Exceptions;

public class LateEventException : Exception
{
    public LateEventException() : base("Event timestamp is more than one hour old.")
    {
    }
    
    public LateEventException(string message) : base(message)
    {
    }
    
    public LateEventException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public LateEventException(DateTime eventTime, DateTime currentTime) 
        : base($"Event timestamp ({eventTime:yyyy-MM-dd HH:mm:ss} UTC) is more than one hour old. Current time: {currentTime:yyyy-MM-dd HH:mm:ss} UTC")
    {
    }
}