namespace Sunday.Core.Models;

public class BoardMember
{
    public string UserId { get; init; }
    public string BoardId { get; init; }
    public BoardMemberRole Role { get; init; }
}
