namespace Parseh.Server.Core.Domain.Aggregates.Role.ValueObject;

public sealed class RoleId : Identity
{
    public long Id { get; }

    RoleId(long id) => Id = id;

    public static RoleId New(long id) => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}
