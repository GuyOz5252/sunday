namespace Sunday.Core.Models;

public class TicketWatcher
{
    public required string TicketId { get; init; }
    public Ticket Ticket { get; init; } = null!;
    
    public required string UserId { get; init; }
    public User User { get; init; } = null!;
}
