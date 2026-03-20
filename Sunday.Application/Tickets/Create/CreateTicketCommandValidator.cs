using FluentValidation;

namespace Sunday.Application.Tickets.Create;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.AgencyId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.BrandId)
            .NotEmpty()
            .When(x => x.BrandId is not null);
        RuleFor(x => x.CampaignId)
            .NotEmpty()
            .When(x => x.CampaignId is not null);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Brief).NotEmpty();
        RuleFor(x => x.CreatorUserId).NotEmpty();
    }
}
