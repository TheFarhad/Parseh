namespace Parseh.Server.Core.Domain.Aggregates.User.ValueObject;

public sealed class UserId : Identity
{
    public long Id { get; }

    UserId(long id) => Id = id;

    public static UserId New(long id) => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}
