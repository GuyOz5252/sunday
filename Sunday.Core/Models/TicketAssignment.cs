namespace Sunday.Core.Models;

public class TicketAssignment
{
    public required string Id { get; init; }
    public required string TicketId { get; init; }
    public Ticket Ticket { get; init; } = null!;
    
    public required string UserId { get; init; }
    public User User { get; init; } = null!;
    
    public required Role Role { get; init; }
    public DateTime AssignedAt { get; init; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
