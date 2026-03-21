using Sunday.Core.Events.Abstract;
using Sunday.Core.Models;

namespace Sunday.Core.Events;

public record TicketAssignedEvent(Ticket Ticket, TicketAssignment Assignment, string AssignedById) : EventBase;
