namespace Parseh.Server.Core.Domain.Aggregates.Role.Entity;

using ValueObject;

public sealed class RoleClaim
{
    private RoleClaim() { }
    private RoleClaim(RoleId roleId, ClaimId permissionId)
    {
        RoleId = roleId;
        ClaimId = permissionId;
    }

    public RoleId RoleId { get; private set; }
    public ClaimId ClaimId { get; private set; }

    public Role Role { get; private set; }
    public Claim Cliam { get; private set; }

    public static RoleClaim Construct(RoleId roleId, ClaimId permissionId)
        => new(roleId, permissionId);
}