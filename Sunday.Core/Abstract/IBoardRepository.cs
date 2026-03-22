using DotResults;
using Sunday.Core.Models;

namespace Sunday.Core.Abstract;

public interface IBoardRepository
{
    Task<Result<Board>> GetAsync(string id, CancellationToken cancellationToken = default);
}
