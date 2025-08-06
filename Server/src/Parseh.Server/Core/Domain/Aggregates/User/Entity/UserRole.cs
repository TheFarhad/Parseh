namespace Parseh.Server.Core.Domain.Aggregates.User.Entity;

using Role.Entity;
using Role.ValueObject;
using Aggregates.User.ValueObject;

public sealed class UserRole
{
    public UserId UserId { get; private set; }
    public RoleId RoleId { get; private set; }

    public User User { get; private set; }
    public Role Role { get; private set; }

    UserRole() { }
    UserRole(UserId userId, RoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public static UserRole New(UserId userId, RoleId roleId)
        => new(userId, roleId);
}