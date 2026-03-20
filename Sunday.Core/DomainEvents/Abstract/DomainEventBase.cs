namespace Sunday.Core.DomainEvents.Abstract;

public record DomainEventBase
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
