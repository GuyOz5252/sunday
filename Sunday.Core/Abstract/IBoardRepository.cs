using DotResults;
using Sunday.Core.Models;

namespace Sunday.Core.Abstract;

public interface IBoardRepository
{
    Task<string> CreateBoardAsync(Board board, CancellationToken cancellationToken = default);
    Task<string> CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task<Result<Board>> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<List<Ticket>>> GetTickets(string id, int skip, int take, CancellationToken cancellationToken = default);
}
