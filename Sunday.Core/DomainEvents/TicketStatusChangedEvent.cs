using Sunday.Core.DomainEvents.Abstract;
using Sunday.Core.Models;

namespace Sunday.Core.DomainEvents;

public record TicketStatusChangedEvent(
    Ticket Ticket,
    TicketStatus OldStatus,
    TicketStatus NewStatus,
    string ChangedById)
    : DomainEventBase;
