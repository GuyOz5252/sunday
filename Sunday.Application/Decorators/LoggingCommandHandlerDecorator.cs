using DotResults;
using Microsoft.Extensions.Logging;
using Sunday.Application.Abstracts;

namespace Sunday.Application.Decorators;

public class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _inner;
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand>> _logger;

    public LoggingCommandHandlerDecorator(ICommandHandler<TCommand> inner, ILogger<LoggingCommandHandlerDecorator<TCommand>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        _logger.LogInformation("Handling command {CommandName}", commandName);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await _inner.HandleAsync(command, cancellationToken);
            stopwatch.Stop();
            _logger.LogInformation("Handled command {CommandName} in {ElapsedMilliseconds}ms", commandName, stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unexpected error occurred during handling of command {CommandName} after {ElapsedMilliseconds}ms", typeof(TCommand).Name, stopwatch.ElapsedMilliseconds);
            return Result.Failure(new Error("UnexpectedError", $"An unexpected error occurred while processing the command {typeof(TCommand).Name}.", "InternalServerError"));
        }
    }
}

public class LoggingCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _inner;
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand, TResult>> _logger;

    public LoggingCommandHandlerDecorator(ICommandHandler<TCommand, TResult> inner, ILogger<LoggingCommandHandlerDecorator<TCommand, TResult>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        _logger.LogInformation("Handling command {CommandName}", commandName);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await _inner.HandleAsync(command, cancellationToken);
            stopwatch.Stop();
            _logger.LogInformation("Handled command {CommandName} in {ElapsedMilliseconds}ms", commandName, stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unexpected error occurred during handling of command {CommandName} after {ElapsedMilliseconds}ms", typeof(TCommand).Name, stopwatch.ElapsedMilliseconds);
            return Result.Failure<TResult>(new Error("UnexpectedError", $"An unexpected error occurred while processing the command {typeof(TCommand).Name}.", "InternalServerError"));
        }
    }
}
