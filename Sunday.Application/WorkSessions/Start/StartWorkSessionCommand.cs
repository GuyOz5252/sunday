using Sunday.Application.Abstracts;

namespace Sunday.Application.WorkSessions.Start;

public record StartWorkSessionCommand(
    string TicketId,
    string UserId,
    string? Description = null)
    : ICommand<string>;
