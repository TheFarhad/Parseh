namespace Parseh.Server.Core.Domain.Aggregates.User.ValueObject;

public sealed class UserId : Identity
{
    public long Id { get; }

    private UserId(long id) => Id = id;

    public static UserId New(long id) => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}

public sealed class RefreshTokenId : Identity
{
    public long Id { get; }

    private RefreshTokenId(long id) => Id = id;

    public static RefreshTokenId New(long id) => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}

public sealed class RoleId : Identity
{
    public long Id { get; }

    private RoleId(long id) => Id = id;

    public static RoleId New(long id) => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}

public sealed class PermissionId : Identity
{
    public long Id { get; }

    private PermissionId(long id) => Id = id;

    public static PermissionId New(long id) => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}
