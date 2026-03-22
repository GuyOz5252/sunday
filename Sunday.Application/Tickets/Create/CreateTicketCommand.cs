using Sunday.Application.Abstract;

namespace Sunday.Application.Tickets.Create;

public record CreateTicketCommand(
    string AgencyId,
    string CampaignId,
    string BoardId,
    string Title,
    string Brief,
    string CreatorUserId,
    DateTime? DueDate = null) : ICommand<string>;
