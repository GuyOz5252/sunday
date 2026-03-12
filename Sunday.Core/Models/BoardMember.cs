namespace Sunday.Core.Models;

public class BoardMember
{
    public required string UserId { get; init; }
    public required BoardMemberRole Role { get; init; }
}
