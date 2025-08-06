namespace Parseh.Server.Core.Domain.Aggregates.Role.Entity;

using Framework;
using ValueObject;

public sealed class Role : AggregateRoot<RoleId>
{
    public const string PermissionsBackingField = nameof(_permissions);

    readonly List<RolePermission> _permissions = [];

    public string Title { get; private set; }
    public string Display { get; private set; }
    public string Description { get; private set; }

    // -- [ نباید از طریق رول، به یوزر دسترسی داشته باشیم ] -- \\

    public IReadOnlyList<RolePermission> Permissions => _permissions.AsReadOnly();

    Role() { }
    Role(string title, string display, string description)
    {
        Title = title;
        Display = display;
        Description = description;
    }

    public static Role New(string title, string display, string description)
        => new(title, display, description);
}