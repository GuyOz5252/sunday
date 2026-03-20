using FluentValidation;

namespace Sunday.Application.Clients.Create;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AgencyId).NotEmpty();
        RuleFor(x => x.AccountManagerId)
            .NotEmpty()
            .When(x => x.AccountManagerId is not null);
    }
}
