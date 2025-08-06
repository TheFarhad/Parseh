namespace Framework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

public abstract class CommandRepository<TAggregateRoot, TId, TDbStore> : ICommandRepository<TAggregateRoot, TId>
    where TId : Identity
    where TAggregateRoot : AggregateRoot<TId>
    where TDbStore : CommandDbStore<TDbStore>
{
    protected readonly TDbStore Context;
    protected readonly DatabaseFacade Database;
    protected readonly DbSet<TAggregateRoot> Table;

    protected CommandRepository(TDbStore context)
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

    public async Task<TAggregateRoot?> SingleOrDefaultAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
       => await Table.SingleOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> SingleOrDefaultAsync(IEnumerable<string> includes, Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await IncludeGraph(includes).SingleOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> SingleOrDefaultAsync(IEnumerable<Expression<Func<TAggregateRoot, object>>> includes, Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
       => await IncludeGraph(includes).SingleOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> FirstOrDefaultAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await Table.FirstOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> FirstOrDefaultAsync(IEnumerable<string> includes, Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await IncludeGraph(includes).FirstOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> FirstOrDefaultAsync(IEnumerable<Expression<Func<TAggregateRoot, object>>> includes, Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await IncludeGraph(includes).FirstOrDefaultAsync(predicate, token);

    // To bulk insert and update
    public async Task<List<TAggregateRoot>?> ListAsync(Expression<Func<TAggregateRoot, bool>> predicate = default!, CancellationToken token = default) =>
        predicate is not null ? await Table.Where(predicate).ToListAsync(token)
        : await Table.ToListAsync(token);

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
        await Task.Delay(1);
    }

    IQueryable<TAggregateRoot> IncludeGraph(IEnumerable<string> includes)
      => Table.AsQueryable().ApplyIncludes(includes);

    IQueryable<TAggregateRoot> IncludeGraph(IEnumerable<Expression<Func<TAggregateRoot, object>>> includes)
        => Table.AsQueryable().ApplyIncludes<TAggregateRoot>(includes);

    IQueryable<TAggregateRoot> IncludeAll()
    {
        var result = Table.AsQueryable();
        Context
         .ApplyAllIncludes(Context.Model, typeof(TAggregateRoot))
         .ToList()
         .ForEach(_ => result.Include(_));

        return result;
    }
}

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplyIncludes<T>(
        this IQueryable<T> query,
        IEnumerable<Expression<Func<T, object>>> includes)
        where T : class
    {
        foreach (var include in includes)
            query = query.Include(include);

        return query;
    }

    public static IQueryable<T> ApplyIncludes<T>(
        this IQueryable<T> query,
        IEnumerable<string> includes)
        where T : class
    {
        foreach (var include in includes)
            query = query.Include(include);

        return query;
    }

    public static IEnumerable<string> ApplyAllIncludes(this DbContext context, IModel model, Type clrType)
    {
        var entityType = model.FindEntityType(clrType);
        var includedNavigations = new HashSet<INavigation>();
        var stack = new Stack<IEnumerator<INavigation>>();
        while (true)
        {
            var navigations = new List<INavigation>();
            var entityNavigations = entityType.GetNavigations();
            foreach (var item in entityNavigations)
            {
                if (includedNavigations.Add(item))
                    navigations.Add(item);
            }
            if (navigations.Count == 0)
            {
                if (stack.Count > 0)
                    yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
            }
            else
            {
                foreach (var navigation in navigations)
                {
                    var inverseNavigation = navigation.Inverse;
                    if (inverseNavigation is { })
                        includedNavigations.Add(inverseNavigation);
                }
                stack.Push(navigations.GetEnumerator());
            }

            while (stack.Count > 0 && !stack.Peek().MoveNext())
                stack.Pop();

            if (stack.Count is 0)
                break;

            entityType = stack.Peek().Current.TargetEntityType;
        }
    }
}

