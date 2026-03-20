using FluentValidation;

namespace Sunday.Application.Tickets.UpdateStatus;

public class UpdateTicketStatusCommandValidator : AbstractValidator<UpdateTicketStatusCommand>
{
    public UpdateTicketStatusCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .When(x => x.Note is not null);
    }
}
