namespace Sunday.Core.Models;

public class WorkSession
{
    public required string Id { get; init; }
    public required string TicketId { get; init; }
    public Ticket Ticket { get; init; } = null!;

    public required string UserId { get; init; }
    
    public DateTime StartTime { get; init; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    
    public string? Description { get; set; }
}
