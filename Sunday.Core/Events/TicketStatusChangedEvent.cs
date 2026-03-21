using Sunday.Core.Events.Abstract;
using Sunday.Core.Models;

namespace Sunday.Core.Events;

public record TicketStatusChangedEvent(
    Ticket Ticket,
    TicketStatus OldStatus,
    TicketStatus NewStatus,
    string ChangedById)
    : EventBase;
