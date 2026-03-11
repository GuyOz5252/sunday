namespace Sunday.Core.Models;

public class Board
{
    public string Id { get; init; }
    public required string Name { get; init; }
    public List<BoardMember> BoardMembers { get; init; } = [];
    public List<Ticket> Tickets { get; init; } = [];
}
