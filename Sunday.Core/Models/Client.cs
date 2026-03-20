namespace Sunday.Core.Models;

public class Client
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string AgencyId { get; init; }
    public Agency Agency { get; init; } = null!;
    public string? AccountManagerId { get; init; }
    public User? AccountManager { get; init; }
    
    public List<Brand> Brands { get; init; } = [];
}
