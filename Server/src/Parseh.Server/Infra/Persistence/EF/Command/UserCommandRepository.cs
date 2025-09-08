namespace Parseh.Server.Infra.Persistence.EF.Command;

using Core.Domain.Aggregates.User.Entity;
using Core.Domain.Aggregates.User.ValueObject;
using Core.Contract.Infra.Persistence.Command.User;

public sealed class UserCommandRepository(ParsehCommandDbContext db)
    : CommandRepository<User, UserId, ParsehCommandDbContext>(db), IUserCommandRepository
{ }