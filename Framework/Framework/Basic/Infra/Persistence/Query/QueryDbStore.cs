namespace Framework;

public abstract class QueryDbStore<TOwner> : StoreContext
    where TOwner : QueryDbStore<TOwner>
{
    protected QueryDbStore() : base() { }
    public QueryDbStore(DbContextOptions<TOwner> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        // در هر کوئری که نخواستیم، می توانیم این قابلیت را ایگنور کنیم
        // _context.Users.AsTracking()...

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: set config to SoftDeletable Entities
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges() => 0;
    public override int SaveChanges(bool acceptAllChangesOnSuccess) => 0;
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Task.FromResult(0);
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        => await Task.FromResult(0);
}

public interface IQueryMidel { }


