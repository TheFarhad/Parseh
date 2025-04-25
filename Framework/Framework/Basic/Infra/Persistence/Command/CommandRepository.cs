namespace Framework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public abstract class CommandRepository<TAggregateRoot, TId, TStoreContext> : ICommandRepository<TAggregateRoot, TId>
    where TId : Identity
    where TAggregateRoot : AggregateRoot<TId>
    where TStoreContext : CommandStoreContext<TStoreContext>
{
    private readonly TStoreContext _context;
    protected readonly DbSet<TAggregateRoot> Set;

    protected CommandRepository(TStoreContext context)
    {
        _context = context;
        Set = context.Set<TAggregateRoot>();
    }

    public void Add(TAggregateRoot source) => Set.Add(source);

    public async ValueTask<EntityEntry<TAggregateRoot>> AddAsync(TAggregateRoot source, CancellationToken token = default)
        => await Set.AddAsync(source, token);

    public async Task<bool> AnyAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await Set.AnyAsync(predicate, token);

    public void BulkAdd(IEnumerable<TAggregateRoot> source) => Set.AddRange(source);

    public async Task BulkAddAsync(IEnumerable<TAggregateRoot> source, CancellationToken token = default)
        => await Set.AddRangeAsync(source, token);

    public async ValueTask<TAggregateRoot?> FindByIdAsync(TId id, CancellationToken token = default)
        => await Set.FindAsync(id, token);

    public async Task<TAggregateRoot?> FirstOrDefaultAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await Set.FirstOrDefaultAsync(predicate, token);

    // To bulk insert and update
    public async Task<List<TAggregateRoot>?> ListAsync(Expression<Func<TAggregateRoot, bool>> predicate = default!, CancellationToken token = default) =>
        predicate is not null ? await Set.Where(predicate).ToListAsync(token)
        : await Set.ToListAsync(token);

    public async Task<TAggregateRoot?> GetGraphAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await IncludeGraph().FirstOrDefaultAsync(predicate, token);

    public void Remove(TAggregateRoot source) => Set.Remove(source);

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

    private IQueryable<TAggregateRoot> IncludeGraph()
    {
        var result = Set.AsQueryable();
        _context
            .RelationsGraph(typeof(TAggregateRoot))
            .ToList()
            .ForEach(_ =>
            {
                result.Include(_);
            });
        return result;
    }
}