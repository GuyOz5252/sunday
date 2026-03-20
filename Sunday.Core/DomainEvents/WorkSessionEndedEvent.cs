using Sunday.Core.DomainEvents.Abstract;
using Sunday.Core.Models;

namespace Sunday.Core.DomainEvents;

public record WorkSessionEndedEvent(Ticket Ticket, WorkSession Session) : DomainEventBase;
