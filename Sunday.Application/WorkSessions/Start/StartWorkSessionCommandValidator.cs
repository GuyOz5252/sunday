using FluentValidation;

namespace Sunday.Application.WorkSessions.Start;

public class StartWorkSessionCommandValidator : AbstractValidator<StartWorkSessionCommand>
{
    public StartWorkSessionCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}
