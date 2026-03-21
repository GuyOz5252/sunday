using Sunday.Core.Events.Abstract;

namespace Sunday.Core.Models;

public abstract class AggregateRoot
{
    private readonly List<EventBase> _domainEvents = [];
    
    public IReadOnlyList<EventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(EventBase @event)
    {
        _domainEvents.Add(@event);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
