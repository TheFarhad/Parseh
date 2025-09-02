namespace Parseh.Server.Core.Domain.Aggregates.Role.Entity;

using Framework;
using ValueObject;

public sealed class Claim : Entity<ClaimId>
{
    private Claim() { }
    private Claim(string title, string display, string description)
    {
        Title = title;
        Display = display;
        Description = description;
    }

    public string Title { get; private set; }
    public string Display { get; private set; }
    public string Description { get; private set; }

    public static Claim Construct(string title, string display, string description)
        => new(title, display, description);
}