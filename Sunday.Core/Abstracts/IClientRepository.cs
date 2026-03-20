using DotResults;
using Sunday.Core.Models;

namespace Sunday.Core.Abstracts;

public interface IClientRepository
{
    Task<string> CreateClientAsync(Client client, CancellationToken cancellationToken = default);
    Task<Result<Client>> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<Client>>> GetByAgencyAsync(string agencyId, CancellationToken cancellationToken = default);

    Task<string> CreateBrandAsync(Brand brand, CancellationToken cancellationToken = default);
    Task<Result<Brand>> GetBrandAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<Brand>>> GetBrandsByClientAsync(string clientId, CancellationToken cancellationToken = default);

    Task<string> CreateCampaignAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task<Result<Campaign>> GetCampaignAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<Campaign>>> GetCampaignsByBrandAsync(string brandId, CancellationToken cancellationToken = default);
}
