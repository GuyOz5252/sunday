namespace Sunday.Core.Models;

public class Campaign
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string BrandId { get; init; }
    public Brand Brand { get; init; } = null!;
}
