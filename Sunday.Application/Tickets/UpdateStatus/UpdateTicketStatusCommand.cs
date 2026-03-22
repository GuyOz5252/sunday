using Sunday.Application.Abstract;
using Sunday.Core.Models;

namespace Sunday.Application.Tickets.UpdateStatus;

public record UpdateTicketStatusCommand(
    string TicketId,
    TicketStatus NewStatus,
    string UserId,
    string? Note = null,
    string? NextAssigneeId = null)
    : ICommand;
