namespace Sunday.Core.Models;

public class User
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required List<string> Roles { get; init; } = [];
}
