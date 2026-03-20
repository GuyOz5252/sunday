namespace Sunday.Core.Models;

public class TicketComment
{
    public required string Id { get; init; }
    public required string TicketId { get; init; }
    public Ticket Ticket { get; init; } = null!;
    public required string AuthorId { get; init; }
    public User Author { get; init; } = null!;
    public required string Content { get; init; }
    public bool IsSystemMessage { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
