using FluentValidation;

namespace Sunday.Application.WorkSessions.Start;

public class StartWorkSessionCommandValidator : AbstractValidator<StartWorkSessionCommand>
{
    public StartWorkSessionCommandValidator()
    {
        RuleFor(command => command.TicketId).NotEmpty();
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.Description)
            .MaximumLength(1000)
            .When(command => command.Description is not null);
    }
}
