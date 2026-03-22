namespace Sunday.Core.Models;

public class Board
{
    public required string Id { get; init; }
    public required string BusinessUnitId { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
