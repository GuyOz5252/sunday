using DotResults;
using Sunday.Core.Models;

namespace Sunday.Core.Abstracts;

public interface ITicketRepository
{
    Task<string> CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task<Result<Ticket>> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<Ticket>>> GetByAgencyAsync(string agencyId, int skip, int take, CancellationToken cancellationToken = default);

    Task<string> CreateWorkSessionAsync(WorkSession session, CancellationToken cancellationToken = default);
    Task<Result<WorkSession>> GetActiveWorkSessionAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<List<WorkSession>>> GetWorkSessionsByTicketAsync(string ticketId, CancellationToken cancellationToken = default);
}
