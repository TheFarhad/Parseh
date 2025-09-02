namespace Parseh.Server.Core.Domain.Aggregates.Role.ValueObject;

public sealed class ClaimId : Identity
{
    private ClaimId(long id)
        => Id = id;

    public readonly long Id;

    public static ClaimId Construct(long id)
        => new(id);

    protected override IEnumerable<object> GetEqualityProperties()
    {
        yield return Id;
    }
}
