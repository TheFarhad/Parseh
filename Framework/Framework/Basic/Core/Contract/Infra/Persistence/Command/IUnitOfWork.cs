namespace Framework;

using Microsoft.EntityFrameworkCore.Storage;

public interface IUnitOfWork : IAsyncDisposable
{
    Task<bool> SaveAsync(CancellationToken token = default);
    Task CommitAsync(Func<IDbContextTransaction, Task> onCommit, Action<DbContext, IDbContextTransaction, Exception>? onFailure = null, CancellationToken token = default);
}
