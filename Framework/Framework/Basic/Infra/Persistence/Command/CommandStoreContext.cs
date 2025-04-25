namespace Framework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class CommandStoreContext<TOwner> : StoreContext
    where TOwner : CommandStoreContext<TOwner>
{
    protected CommandStoreContext() : base() { }
    public CommandStoreContext(DbContextOptions<TOwner> options) : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder cb)
    {
        // all strings: varchar(200)
        cb.Properties<string>().AreUnicode(false).HaveMaxLength(200);
        // all enumer: varchar(100) 
        cb.Properties<Enumer>().AreUnicode(false).HaveMaxLength(100);

        base.ConfigureConventions(cb);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder ob) => base.OnConfiguring(ob);

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // TODO: ...
        base.OnModelCreating(mb);
    }

    public IEnumerable<string> RelationsGraph(Type clrType)
    {
        var entityType = Model.FindEntityType(clrType);
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
            if (stack.Count is 0) break;
            entityType = stack.Peek().Current.TargetEntityType;
        }
    }
}


