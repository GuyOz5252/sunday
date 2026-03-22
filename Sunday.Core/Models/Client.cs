namespace Sunday.Core.Models;

public class Client
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string AgencyId { get; init; }
    public string? AccountManagerId { get; init; }

    public List<Brand> Brands { get; init; } = [];
}
