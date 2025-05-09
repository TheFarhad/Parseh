namespace Framework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

public abstract class CommandRepository<TAggregateRoot, TId, TStoreContext> : ICommandRepository<TAggregateRoot, TId>
    where TId : Identity
    where TAggregateRoot : AggregateRoot<TId>
    where TStoreContext : CommandStoreContext<TStoreContext>
{
    protected readonly TStoreContext Context;
    protected readonly DatabaseFacade Database;
    protected readonly DbSet<TAggregateRoot> Table;

    protected CommandRepository(TStoreContext context)
    {
        Context = context;
        Database = context.Database;
        Table = context.Set<TAggregateRoot>();
    }

    public void Add(TAggregateRoot source)
        => Table.Add(source);

    public async ValueTask<EntityEntry<TAggregateRoot>> AddAsync(TAggregateRoot source, CancellationToken token = default)
        => await Table.AddAsync(source, token);

    /// <summary>
    /// Used when we have value generation for Id field 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> AnyAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await Table.AnyAsync(predicate, token);

    public void BulkAdd(IEnumerable<TAggregateRoot> source)
        => Table.AddRange(source);

    public async Task BulkAddAsync(IEnumerable<TAggregateRoot> source, CancellationToken token = default)
        => await Table.AddRangeAsync(source, token);

    public async ValueTask<TAggregateRoot?> FindByIdAsync(TId id, CancellationToken token = default)
        => await Table.FindAsync(id, token);

    public async Task<TAggregateRoot?> FirstOrDefaultAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await Table.FirstOrDefaultAsync(predicate, token);

    // To bulk insert and update
    public async Task<List<TAggregateRoot>?> ListAsync(Expression<Func<TAggregateRoot, bool>> predicate = default!, CancellationToken token = default) =>
        predicate is not null ? await Table.Where(predicate).ToListAsync(token)
        : await Table.ToListAsync(token);

    public async Task<TAggregateRoot?> GetGraphAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await IncludeGraph().FirstOrDefaultAsync(predicate, token);

    public void Remove(TAggregateRoot source)
        => Table.Remove(source);

    public async ValueTask Remove(TId id)
    {
        var entity = await FindByIdAsync(id);
        if (entity is not null) Remove(entity);
    }

    public async Task RemoveGraph(TId id)
    {
        //var entity = await GetGraphAsync(_ => _.Id == id);
        //Delete(entity);
    }

    IQueryable<TAggregateRoot> IncludeGraph()
    {
        var result = Table.AsQueryable();
        Context
         .RelationsGraph(typeof(TAggregateRoot))
         .ToList()
         .ForEach(_ =>
         {
             result.Include(_);
         });
        return result;
    }
}