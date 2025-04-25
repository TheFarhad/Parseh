namespace Framework;

public interface IDomainEvent
{
    Guid Code { get; }
    DateTime OccurredOn { get; }
    DomainEventMode Mode { get; }
}

public enum DomainEventMode : byte { Added = 0, Edited = 1, Removed = 2 }