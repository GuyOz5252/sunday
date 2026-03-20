using Sunday.Application.Abstracts;

namespace Sunday.Application.WorkSessions.Start;

public record StartWorkSessionCommand : ICommand<string>
{
    public string TicketId { get; }
    public string UserId { get; }
    public string? Description { get; }

    public StartWorkSessionCommand(
        string ticketId,
        string userId,
        string? description = null)
    {
        TicketId = ticketId;
        UserId = userId;
        Description = description;
    }
}
