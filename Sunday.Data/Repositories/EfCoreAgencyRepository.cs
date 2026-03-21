using DotResults;
using Microsoft.EntityFrameworkCore;
using Sunday.Core.Abstracts;
using Sunday.Core.Models;

namespace Sunday.Data.Repositories;

public class EfCoreAgencyRepository : IAgencyRepository
{
    private readonly ApplicationDbContext _context;

    public EfCoreAgencyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<string> CreateAgencyAsync(Agency agency, CancellationToken cancellationToken = default)
    {
        _context.Agencies.Add(agency);
        return Task.FromResult(agency.Id);
    }

    public async Task<Result<Agency>> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var agency = await _context.Agencies
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return agency is null
            ? Result.Failure<Agency>(new Error("Agency.NotFound", $"Agency with id {id} not found", "NotFound"))
            : Result.Success(agency);
    }

    public async Task<Result<Agency>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var agency = await _context.Agencies
            .FirstOrDefaultAsync(a => a.Slug == slug, cancellationToken);

        return agency is null
            ? Result.Failure<Agency>(new Error("Agency.NotFound", $"Agency with slug {slug} not found", "NotFound"))
            : Result.Success(agency);
    }
}
