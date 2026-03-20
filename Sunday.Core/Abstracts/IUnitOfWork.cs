using DotResults;

namespace Sunday.Core.Abstracts;

public interface IUnitOfWork
{
    Task<Result> CommitAsync(CancellationToken cancellationToken = default);
}
