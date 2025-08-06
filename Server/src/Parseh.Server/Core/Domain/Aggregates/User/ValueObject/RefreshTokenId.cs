namespace Parseh.Server.Core.Domain.Aggregates.User.ValueObject;

public sealed class RefreshTokenId : Identity
{
    public long Id { get; }

    RefreshTokenId(long id) => Id = id;

    public static RefreshTokenId New(long id) => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}
