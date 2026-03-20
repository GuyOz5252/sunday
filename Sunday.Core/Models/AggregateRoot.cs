using Sunday.Core.DomainEvents.Abstract;

namespace Sunday.Core.Models;

public abstract class AggregateRoot
{
    private readonly List<DomainEventBase> _domainEvents = [];
    
    public IReadOnlyList<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(DomainEventBase domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
