namespace Framework;

using Microsoft.EntityFrameworkCore.Storage;

public abstract class UnitOfWork<TStoreContext>(TStoreContext storeContext) : IUnitOfWork
    where TStoreContext : CommandStoreContext<TStoreContext>
{
    private readonly TStoreContext _context = storeContext;

    public async Task CommitAsync(Func<IDbContextTransaction, Task> onCommit, Action<DbContext, IDbContextTransaction, Exception>? onFailure = null, CancellationToken token = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(token);
        try
        {
            if (onCommit is { })
            {
                await onCommit.Invoke(transaction);
                await _context.Database.CommitTransactionAsync(token);
                // or ??
                //await transaction.CommitAsync(token);
            }
        }
        catch (Exception e)
        {
            if (onFailure is { })
            {
                onFailure.Invoke(_context, transaction, e);
            }
            else
            {
                await _context.Database.RollbackTransactionAsync(token);
                // or ??
                // await transaction.RollbackAsync(token);
            }
        }
    }

    public async Task<bool> SaveAsync(CancellationToken token = default) => await _context.SaveChangesAsync(token) > 0;

    /// <summary>
    /// Used when we work with in-memory databases
    /// </summary>
    /// <returns></returns>
    public bool Save() => _context.SaveChanges() > 0;

    public async ValueTask DisposeAsync() => await _context.DisposeAsync();
}

