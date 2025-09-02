namespace Parseh.Server.Core.Domain.Aggregates.Role.Entity;

using Framework;
using ValueObject;

public sealed class Role : AggregateRoot<RoleId>
{
    public const string ClaimsBackingField = nameof(_claims);

    private readonly List<RoleClaim> _claims;

    private Role() { }
    private Role(string title, string display, string description)
    {
        Title = title;
        Display = display;
        Description = description;
        _claims = [];
    }

    public string Title { get; private set; }
    public string Display { get; private set; }
    public string Description { get; private set; }

    // -- [ نباید از طریق رول، به یوزر دسترسی داشته باشیم ] -- \\

    public IReadOnlyList<RoleClaim> Cliams => [.. _claims];

    public static Role Construct(string title, string display, string description)
        => new(title, display, description);

    public void AddClaim(Claim claim)
    {
        if (claim is null)
            return;

        if (_claims.Any(_ => _.ClaimId == claim.Id))
        {
            // TODO:
            return;
        }

        var roleClaim = RoleClaim.Construct(Id, claim.Id);
        _claims.Add(roleClaim);
    }
}