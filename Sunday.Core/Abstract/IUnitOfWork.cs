using DotResults;

namespace Sunday.Core.Abstract;

public interface IUnitOfWork
{
    Task<Result> CommitAsync(CancellationToken cancellationToken = default);
}
