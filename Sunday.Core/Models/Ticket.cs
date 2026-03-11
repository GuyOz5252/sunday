namespace Sunday.Core.Models;

public class Ticket
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Details { get; init; }
    public required TicketAssignee Assignee { get; init; }
    // TODO: Attachments
}
