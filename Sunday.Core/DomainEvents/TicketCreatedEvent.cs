using Sunday.Core.DomainEvents.Abstract;
using Sunday.Core.Models;

namespace Sunday.Core.DomainEvents;

public record TicketCreatedEvent(Ticket Ticket, string CreatedById) : DomainEventBase;
