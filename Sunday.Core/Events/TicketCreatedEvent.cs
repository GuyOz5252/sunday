using Sunday.Core.Events.Abstract;
using Sunday.Core.Models;

namespace Sunday.Core.Events;

public record TicketCreatedEvent(Ticket Ticket, string CreatedById) : EventBase;
