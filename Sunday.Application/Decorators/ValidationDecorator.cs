using DotResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Sunday.Application.Abstract;

namespace Sunday.Application.Decorators;

public static class ValidationDecorator
{
    public class CommandHandler<TCommand, TResult>(
        ILogger<CommandHandler<TCommand, TResult>> logger,
        ICommandHandler<TCommand, TResult> innerCommandHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var tasks = validators
                .Select(validator => validator.ValidateAsync(command, cancellationToken));
            var validationResults = await Task.WhenAll(tasks);

            if (validationResults.Length == 0)
            {
                return await innerCommandHandler.HandleAsync(command, cancellationToken);
            }
            
            var validationErrors = CreateValidationErrors(validationResults);
            logger.LogDebug("Command: {Command} failed validation with Validation Errors: {ValidationErrors}",
                typeof(TCommand).Name,
                validationResults.ToArray());
            return validationErrors;
        }
    }
    
    public class CommandHandler<TCommand>(
        ILogger<CommandHandler<TCommand>> logger,
        ICommandHandler<TCommand> innerCommandHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var tasks = validators
                .Select(validator => validator.ValidateAsync(command, cancellationToken));
            var validationResults = await Task.WhenAll(tasks);

            if (validationResults.Length == 0)
            {
                return await innerCommandHandler.HandleAsync(command, cancellationToken);
            }
        
            var validationErrors = CreateValidationErrors(validationResults);
            logger.LogDebug("Command: {Command} failed validation with Validation Errors: {ValidationErrors}",
                typeof(TCommand).Name,
                validationResults.ToArray());
            return validationErrors;
        }
    }
    
    public class QueryHandler<TQuery, TResult>(
        ILogger<QueryHandler<TQuery, TResult>> logger,
        IQueryHandler<TQuery, TResult> innerQueryHandler,
        IEnumerable<IValidator<TQuery>> validators)
        : IQueryHandler<TQuery, TResult>
    {
        public async Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            var tasks = validators
                .Select(validator => validator.ValidateAsync(query, cancellationToken));
            var validationResults = await Task.WhenAll(tasks);

            if (validationResults.Length == 0)
            {
                return await innerQueryHandler.HandleAsync(query, cancellationToken);
            }
        
            var validationErrors = CreateValidationErrors(validationResults);
            logger.LogDebug("Query: {Query} failed validation with Validation Errors: {ValidationErrors}",
                typeof(TQuery).Name,
                validationResults.ToArray());
            return validationErrors;
        }
    }
    
    private static ValidationError[] CreateValidationErrors(ValidationResult[] validationResults)
    {
        var validationErrors = validationResults.SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.AttemptedValue.ToString() ?? "",
                validationFailure.ErrorMessage))
            .ToArray();

        return validationErrors;
    }
}
