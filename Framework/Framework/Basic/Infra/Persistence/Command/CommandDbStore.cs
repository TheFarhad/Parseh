namespace Framework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class CommandDbStore<TOwner> : StoreContext
    where TOwner : CommandDbStore<TOwner>
{
    protected CommandDbStore() : base() { }
    public CommandDbStore(DbContextOptions<TOwner> options) : base(options) { }

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
}


