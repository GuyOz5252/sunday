using Sunday.Application.Abstracts;

namespace Sunday.Application.WorkSessions.Stop;

public record StopWorkSessionCommand : ICommand
{
    public string UserId { get; }
    public string? Description { get; }

    public StopWorkSessionCommand(
        string userId,
        string? description = null)
    {
        UserId = userId;
        Description = description;
    }
}
