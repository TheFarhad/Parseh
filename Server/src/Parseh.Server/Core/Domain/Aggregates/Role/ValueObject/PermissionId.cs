namespace Parseh.Server.Core.Domain.Aggregates.Role.ValueObject;

public sealed class PermissionId : Identity
{
    public long Id { get; }

    PermissionId(long id) => Id = id;

    public static PermissionId New(long id) => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}
