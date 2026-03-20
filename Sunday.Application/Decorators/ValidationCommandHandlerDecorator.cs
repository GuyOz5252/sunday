using DotResults;
using FluentValidation;
using Sunday.Application.Abstracts;

namespace Sunday.Application.Decorators;

public class ValidationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _inner;
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public ValidationCommandHandlerDecorator(ICommandHandler<TCommand> inner, IEnumerable<IValidator<TCommand>> validators)
    {
        _inner = inner;
        _validators = validators;
    }

    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var context = new ValidationContext<TCommand>(command);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
        {
            var firstFailure = failures[0];
            return Result.Failure(new Error("Validation.Error", firstFailure.ErrorMessage, "UnprocessableEntity"));
        }

        return await _inner.HandleAsync(command, cancellationToken);
    }
}

public class ValidationCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _inner;
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public ValidationCommandHandlerDecorator(ICommandHandler<TCommand, TResult> inner, IEnumerable<IValidator<TCommand>> validators)
    {
        _inner = inner;
        _validators = validators;
    }

    public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var context = new ValidationContext<TCommand>(command);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
        {
            var firstFailure = failures[0];
            return Result.Failure<TResult>(new Error("Validation.Error", firstFailure.ErrorMessage, "UnprocessableEntity"));
        }

        return await _inner.HandleAsync(command, cancellationToken);
    }
}
