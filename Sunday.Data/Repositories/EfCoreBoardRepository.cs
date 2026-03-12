using DotResults;
using Microsoft.EntityFrameworkCore;
using Sunday.Core.Abstract;
using Sunday.Core.Models;

namespace Sunday.Data.Repositories;

public class EfCoreBoardRepository(ApplicationDbContext context) : IBoardRepository
{
    public Task<string> CreateBoardAsync(Board board, CancellationToken cancellationToken = default)
    {
        context.Boards.Add(board);
        return Task.FromResult(board.Id);
    }

    public Task<string> CreateTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        context.Tickets.Add(ticket);
        return Task.FromResult(ticket.Id);
    }

    public async Task<Result<Board>> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var board = await context.Boards
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        return board is null
            ? Result.Failure<Board>(BoardErrors.NotFound(id))
            : Result.Success(board);
    }

    public async Task<Result<List<Ticket>>> GetTickets(string id, int skip, int take, CancellationToken cancellationToken = default)
    {
        var tickets = await context.Tickets
            .Where(t => EF.Property<string>(t, "BoardId") == id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return Result.Success(tickets);
    }
}

public static class BoardErrors
{
    public static Error NotFound(string id) => new Error("Board.NotFound", $"Board with id {id} was not found", "NotFound");
}
