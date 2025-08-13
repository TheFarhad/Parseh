namespace Framework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

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
        => await Includes(includes).SingleOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> SingleOrDefaultAsync(IEnumerable<Expression<Func<TAggregateRoot, object>>> includes, Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
       => await Includes(includes).SingleOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> SingleOrDefaultAsync(bool includeAll, Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
       => await IncludeAll(includeAll).SingleOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> FirstOrDefaultAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await Table.FirstOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> FirstOrDefaultAsync(IEnumerable<string> includes, Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await Includes(includes).FirstOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> FirstOrDefaultAsync(IEnumerable<Expression<Func<TAggregateRoot, object>>> includes, Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
        => await Includes(includes).FirstOrDefaultAsync(predicate, token);

    public async Task<TAggregateRoot?> FirstOrDefaultAsync(bool includeAll, Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
       => await IncludeAll(includeAll).FirstOrDefaultAsync(predicate, token);

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

    IQueryable<TAggregateRoot> Includes(IEnumerable<string> includes)
      => Table.AsQueryable().ApplyIncludes(includes);

    IQueryable<TAggregateRoot> Includes(IEnumerable<Expression<Func<TAggregateRoot, object>>> includes)
        => Table.AsQueryable().ApplyIncludes<TAggregateRoot>(includes);

    IQueryable<TAggregateRoot> IncludeAll(bool includeAll)
    {
        var result = Table.AsQueryable();
        if (includeAll)
        {
            var includes = IncludeProvider.GetNavigationPaths<TAggregateRoot>(maxDepth: 4);
            // TODO: درست کار نمی کند
            result.ApplyIncludes(includes);
        }
        return result;
    }
}

public static class IncludeProvider
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

    public static List<string> GetNavigationPaths<T>(
        int maxDepth = 3,
        HashSet<Type> visitedTypes = null!,
        string prefix = "")
    {
        // TODO: درست کار نمی کند

        visitedTypes ??= new HashSet<Type>();
        var paths = new List<string>();
        var type = typeof(T);

        if (visitedTypes.Contains(type) || maxDepth < 1)
            return paths;

        visitedTypes.Add(type);

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsNavigationProperty(p));

        foreach (var prop in properties)
        {
            var currentPath = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
            paths.Add(currentPath);

            var nestedType = GetNavigationType(prop.PropertyType);

            // جلوگیری از دنبال کردن Value Objectهایی که navigation نیستند
            if (ShouldTraverseNestedType(nestedType))
            {
                paths.AddRange(GetNavigationPathsInternal(nestedType, maxDepth - 1, visitedTypes, currentPath));
            }
        }
        return paths;
    }

    static List<string> GetNavigationPathsInternal(
        Type type,
        int depth,
        HashSet<Type> visitedTypes,
        string prefix)
    {
        var method = typeof(IncludeProvider)
            .GetMethod(nameof(GetNavigationPaths), BindingFlags.Public | BindingFlags.Static)
            .MakeGenericMethod(type);

        return (List<string>)method.Invoke(null, new object[] { depth, visitedTypes, prefix });
    }

    static bool IsNavigationProperty(PropertyInfo property)
    {
        var type = property.PropertyType;

        // رد کردن primitiveها و string
        if (type.IsPrimitive || type == typeof(string) || type.IsEnum || type.IsValueType)
            return false;

        // مجموعه‌ها مثل ICollection<T> یا List<T>
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type != typeof(string))
            return true;

        // فقط کلاس‌هایی که Entity هستن
        return IsEntity(type);
    }

    static bool IsEntity(Type type)
    {
        // Convention: داشتن Id یا [TypeName]Id
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(p =>
                String.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(p.Name, $"{type.Name}Id", StringComparison.OrdinalIgnoreCase));
    }

    static Type GetNavigationType(Type propertyType)
    {
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
        {
            return propertyType.IsGenericType ? propertyType.GetGenericArguments()[0] : typeof(object);
        }

        return propertyType;
    }

    static bool ShouldTraverseNestedType(Type type)
    {
        // جلوگیری از دنبال کردن Value Objectهایی مثل DateTime، Guid، یا Structها
        if (type.IsPrimitive || type == typeof(string) || type.IsEnum || type.IsValueType)
            return false;

        // جلوگیری از دنبال کردن کلاس‌های سیستمی
        if (type.Namespace != null && type.Namespace.StartsWith("System"))
            return false;

        return true;
    }
}

