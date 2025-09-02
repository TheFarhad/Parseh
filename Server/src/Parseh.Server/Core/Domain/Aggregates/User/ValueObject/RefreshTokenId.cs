namespace Parseh.Server.Core.Domain.Aggregates.User.ValueObject;

public sealed class RefreshTokenId : Identity
{
    private RefreshTokenId(long id)
        => Id = id;

    public readonly long Id;

    public static RefreshTokenId Construct(long id)
        => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}
