using DotResults;
using Sunday.Application.Abstracts;

namespace Sunday.Application.Decorators;

public class TransactionCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _inner;

    public TransactionCommandHandlerDecorator(ICommandHandler<TCommand> inner)
    {
        _inner = inner;
    }

    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        return await _inner.HandleAsync(command, cancellationToken);
    }
}

public class TransactionCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _inner;

    public TransactionCommandHandlerDecorator(ICommandHandler<TCommand, TResult> inner)
    {
        _inner = inner;
    }

    public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        return await _inner.HandleAsync(command, cancellationToken);
    }
}
