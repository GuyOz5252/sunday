using Sunday.Core.Events.Abstract;
using Sunday.Core.Models;

namespace Sunday.Core.Events;

public record WorkSessionEndedEvent(Ticket Ticket, WorkSession Session) : EventBase;
