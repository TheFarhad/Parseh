using Parseh.Server.Core.Domain.Aggregates.Role.ValueObject;

namespace Parseh.Server.Core.Domain.Aggregates.Role.Entity;

public sealed class RolePermission
{
    public RoleId RoleId { get; private set; }
    public PermissionId PermissionId { get; private set; }

    public Role Role { get; private set; }
    public Permission Permission { get; private set; }

    RolePermission() { }
    RolePermission(RoleId roleId, PermissionId permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
    public static RolePermission New(RoleId roleId, PermissionId permissionId)
        => new(roleId, permissionId);
}