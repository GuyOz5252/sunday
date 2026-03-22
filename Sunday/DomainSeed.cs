using Microsoft.EntityFrameworkCore;
using Sunday.Core.Models;
using Sunday.Data;

namespace Sunday;

public static class DomainSeed
{
    public const string DefaultBusinessUnitId = "00000000-0000-4000-8000-000000000001";
    public const string DefaultBoardStudioId = "00000000-0000-4000-8000-000000000002";

    public static async Task EnsureDefaultBusinessUnitAndBoardAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        if (!await db.BusinessUnits.AnyAsync(bu => bu.Id == DefaultBusinessUnitId, cancellationToken))
        {
            db.BusinessUnits.Add(new BusinessUnit
            {
                Id = DefaultBusinessUnitId,
                Name = "Default",
                Slug = "default"
            });
            await db.SaveChangesAsync(cancellationToken);
        }

        if (!await db.Boards.AnyAsync(b => b.Id == DefaultBoardStudioId, cancellationToken))
        {
            db.Boards.Add(new Board
            {
                Id = DefaultBoardStudioId,
                BusinessUnitId = DefaultBusinessUnitId,
                Name = "Studio",
                Slug = "studio",
                SortOrder = 0
            });
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
