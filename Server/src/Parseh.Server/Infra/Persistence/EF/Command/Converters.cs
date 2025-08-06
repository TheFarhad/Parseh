namespace Parseh.Server.Infra.Persistence.EF.Command;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Core.Domain.Aggregates.Role.ValueObject;
using Core.Domain.Aggregates.User.ValueObject;

internal sealed class UserIdConverter()
    : ValueConverter<UserId, long>(_ => _.Id, _ => UserId.New(_))
{ }

internal sealed class RoleIdConverter()
    : ValueConverter<RoleId, long>(_ => _.Id, _ => RoleId.New(_))
{ }

internal sealed class PermissionIdConverter()
    : ValueConverter<PermissionId, long>(_ => _.Id, _ => PermissionId.New(_))
{ }

internal sealed class RefreshTokenIdConverter()
    : ValueConverter<RefreshTokenId, long>(_ => _.Id, _ => RefreshTokenId.New(_))
{ }
