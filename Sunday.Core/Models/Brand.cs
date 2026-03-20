namespace Sunday.Core.Models;

public class Brand
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string ClientId { get; init; }
    public Client Client { get; init; } = null!;
    
    public List<Campaign> Campaigns { get; init; } = [];
}
