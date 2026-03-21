using Sunday.Core.Events.Abstract;
using Sunday.Core.Models;

namespace Sunday.Core.Events;

public record WorkSessionStartedEvent(Ticket Ticket, WorkSession Session) : EventBase;
