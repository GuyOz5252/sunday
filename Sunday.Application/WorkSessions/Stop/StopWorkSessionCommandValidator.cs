using FluentValidation;

namespace Sunday.Application.WorkSessions.Stop;

public class StopWorkSessionCommandValidator : AbstractValidator<StopWorkSessionCommand>
{
    public StopWorkSessionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}
