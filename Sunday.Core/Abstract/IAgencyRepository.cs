using DotResults;
using Sunday.Core.Models;

namespace Sunday.Core.Abstract;

public interface IAgencyRepository
{
    Task<string> CreateAgencyAsync(Agency agency, CancellationToken cancellationToken = default);
    Task<Result<Agency>> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<Agency>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
