namespace Parseh.Server.Core.Contract.Infra.Persistence.Command.User;

using System.Linq.Expressions;
using Core.Domain.Aggregates.User.Entity;
using Core.Domain.Aggregates.User.ValueObject;

public interface IUserCommandRepository : ICommandRepository<User, UserId>
{
    Task<User?> SingleOrDefaultAsync(Expression<Func<User, bool>> predicate, CancellationToken token = default!);
    Task<User?> SingleOrDefaultAsync(IEnumerable<string> includes, Expression<Func<User, bool>> predicate, CancellationToken token = default!);
    Task<User?> SingleOrDefaultAsync(IEnumerable<Expression<Func<User, object>>> includes, Expression<Func<User, bool>> predicate, CancellationToken token = default!);
    Task<User?> SingleOrDefaultAsync(bool includeAll, Expression<Func<User, bool>> predicate, CancellationToken token = default!);
}