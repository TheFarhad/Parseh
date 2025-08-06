namespace Framework;

// ENTITY
public interface IEntity { }
public abstract class Entity : IEntity { }

public abstract class Entity<TId> : Entity, /*IEntity,*/ IEquatable<Entity<TId>>
    where TId : Identity
{
    public TId Id { get; private set; }
    public Code Code { get; private set; } = Code.New(Guid.NewGuid());

    protected Entity() { }

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        var result = false;
        if (left is null && right is null) result = true;
        else if (left is { } && right is { }) result = left.Equals(right);
        return result;
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right) => !(left == right);

    public bool Equals(Entity<TId>? other) => Id == other?.Id;

    public override bool Equals(object? obj) => obj is Entity<TId> other && Id == other.Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public object? CallMethod(string methodName, Type type, params object[] parameters)
        => GetType()
           .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, [type])
           ?.Invoke(this, parameters);
}

// AGGREGATE ROOT
public interface IAggregateRoot : IEntity
{
    int Version { get; }
    IReadOnlyCollection<IDomainEvent> Events { get; }
    void ClearEvents();
}

public abstract class AggregateRoot : IAggregateRoot
{
    public abstract int Version { get; }
    public abstract IReadOnlyCollection<IDomainEvent> Events { get; }
    public abstract void ClearEvents();
}

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : Identity
{
    public int Version { get; protected set; }

    private readonly List<IDomainEvent> _events = [];
    public IReadOnlyCollection<IDomainEvent> Events => [.. _events];

    protected AggregateRoot() { }
    protected AggregateRoot(IEnumerable<IDomainEvent> events) => LastState(events);

    public void ClearEvents() => _events.Clear();

    protected void Apply<T>(T source) where T : IDomainEvent
    {
        SetEvent(source);
        Mutate(source);
        //Version++;
    }

    private void LastState(IEnumerable<IDomainEvent> events)
    {
        if (events?.Any() is true)
        {
            ClearEvents();
            foreach (var item in events)
            {
                Mutate(item);
                //Version++;
            }
            _events.AddRange(events);
        }
    }

    private void SetEvent<T>(T source) where T : IDomainEvent
        => _events.Add(source);

    private void Mutate<T>(T source) where T : IDomainEvent
        => CallMethod("On", source.GetType(), [source]);
}

// AGGREGATE ROOT (SOFT DELETE)
public interface ISoftDelete
{
    bool Deleted { get; }
    void Delete();
    void Restore();
}

public abstract class AggregateRootDeleteable<TId> : AggregateRoot<TId>, ISoftDelete
    where TId : Identity
{
    public bool Deleted { get; private set; } = false;

    protected AggregateRootDeleteable() : base() { }
    protected AggregateRootDeleteable(IEnumerable<IDomainEvent> events) : base(events) { }

    public void Delete() => Deleted = true;
    public void Restore() => Deleted = false;
}