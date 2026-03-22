using DotResults;
using Microsoft.EntityFrameworkCore;
using Sunday.Core.Abstract;
using Sunday.Core.Models;

namespace Sunday.Data.Repositories;

public class EfCoreBoardRepository : IBoardRepository
{
    private readonly ApplicationDbContext _context;

    public EfCoreBoardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Board>> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        return board is null
            ? Result.Failure<Board>(new Error("Board.NotFound", $"Board with id {id} was not found", "NotFound"))
            : Result.Success(board);
    }
}
