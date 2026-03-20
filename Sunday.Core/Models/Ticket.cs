namespace Sunday.Core.Models;

public class Ticket : AggregateRoot
{
    public required string Id { get; init; }
    public required string AgencyId { get; init; }
    public Agency Agency { get; init; } = null!;

    public required string ClientId { get; init; }
    public Client Client { get; init; } = null!;
    
    public string? BrandId { get; init; }
    public Brand? Brand { get; init; }
    
    public string? CampaignId { get; init; }
    public Campaign? Campaign { get; init; }

    public required string Title { get; set; }
    public required string Brief { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Draft;
    
    public int RevisionRound { get; set; }
    
    public string? HandoffNote { get; set; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }

    public List<TicketAssignment> Assignments { get; init; } = [];
    public List<TicketWatcher> Watchers { get; init; } = [];
    public List<WorkSession> WorkSessions { get; init; } = [];
    public List<TicketComment> Comments { get; init; } = [];
}
