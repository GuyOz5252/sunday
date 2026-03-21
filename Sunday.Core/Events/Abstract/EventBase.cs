namespace Sunday.Core.Events.Abstract;

public abstract record EventBase
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
