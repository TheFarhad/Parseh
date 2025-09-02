namespace Parseh.Server.Core.Domain.Aggregates.Role.ValueObject;

public sealed class RoleId : Identity
{
    private RoleId(long id)
        => Id = id;

    public readonly long Id;

    public static RoleId Construct(long id)
        => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}
