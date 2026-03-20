namespace Sunday.Core.Models;

public class Agency
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public List<User> Users { get; init; } = [];
    public List<Client> Clients { get; init; } = [];
}
