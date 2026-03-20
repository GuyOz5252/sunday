using Sunday.Application.Abstracts;

namespace Sunday.Application.Tickets.Create;

public record CreateTicketCommand(
    string AgencyId,
    string ClientId,
    string? BrandId,
    string? CampaignId,
    string Title,
    string Brief,
    string CreatorUserId,
    DateTime? DueDate = null) : ICommand<string>;
