using Sunday.Core.DomainEvents.Abstract;
using Sunday.Core.Models;

namespace Sunday.Core.DomainEvents;

public record TicketAssignedEvent(Ticket Ticket, TicketAssignment Assignment, string AssignedById) : DomainEventBase;
