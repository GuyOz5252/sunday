using DotResults;
using Microsoft.EntityFrameworkCore;
using Sunday.Core.Abstracts;
using Sunday.Core.Models;

namespace Sunday.Data.Repositories;

public class EfCoreClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public EfCoreClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<string> CreateClientAsync(Client client, CancellationToken cancellationToken = default)
    {
        _context.Clients.Add(client);
        return Task.FromResult(client.Id);
    }

    public async Task<Result<Client>> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return client is null
            ? Result.Failure<Client>(new Error("Client.NotFound", $"Client with id {id} not found", "NotFound"))
            : Result.Success(client);
    }

    public async Task<Result<List<Client>>> GetByAgencyAsync(string agencyId, CancellationToken cancellationToken = default)
    {
        var clients = await _context.Clients
            .Where(c => c.AgencyId == agencyId)
            .ToListAsync(cancellationToken);

        return Result.Success(clients);
    }

    public Task<string> CreateBrandAsync(Brand brand, CancellationToken cancellationToken = default)
    {
        _context.Brands.Add(brand);
        return Task.FromResult(brand.Id);
    }

    public async Task<Result<Brand>> GetBrandAsync(string id, CancellationToken cancellationToken = default)
    {
        var brand = await _context.Brands
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        return brand is null
            ? Result.Failure<Brand>(new Error("Brand.NotFound", $"Brand with id {id} not found", "NotFound"))
            : Result.Success(brand);
    }

    public async Task<Result<List<Brand>>> GetBrandsByClientAsync(string clientId, CancellationToken cancellationToken = default)
    {
        var brands = await _context.Brands
            .Where(b => b.ClientId == clientId)
            .ToListAsync(cancellationToken);

        return Result.Success(brands);
    }

    public Task<string> CreateCampaignAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        _context.Campaigns.Add(campaign);
        return Task.FromResult(campaign.Id);
    }

    public async Task<Result<Campaign>> GetCampaignAsync(string id, CancellationToken cancellationToken = default)
    {
        var campaign = await _context.Campaigns
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return campaign is null
            ? Result.Failure<Campaign>(new Error("Campaign.NotFound", $"Campaign with id {id} not found", "NotFound"))
            : Result.Success(campaign);
    }

    public async Task<Result<List<Campaign>>> GetCampaignsByBrandAsync(string brandId, CancellationToken cancellationToken = default)
    {
        var campaigns = await _context.Campaigns
            .Where(c => c.BrandId == brandId)
            .ToListAsync(cancellationToken);

        return Result.Success(campaigns);
    }
}
