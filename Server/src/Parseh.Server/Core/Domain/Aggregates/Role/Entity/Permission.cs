namespace Parseh.Server.Core.Domain.Aggregates.Role.Entity;

using Framework;
using ValueObject;

public sealed class Permission : Entity<PermissionId>
{
    public string Title { get; private set; }
    public string Display { get; private set; }
    public string Description { get; private set; }

    Permission() { }
    Permission(string title, string display, string description)
    {
        Title = title;
        Display = display;
        Description = description;
    }

    public static Permission New(string title, string display, string description)
        => new(title, display, description);
}