namespace Sunday.Core.Models;

public class User
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
    public string? AgencyId { get; init; }
    public Agency? Agency { get; init; }
    public required List<Role> Roles { get; init; } = [];
}
