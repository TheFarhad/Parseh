namespace Framework;

public abstract class StoreContext : DbContext
{
    protected StoreContext() : base() { }
    public StoreContext(DbContextOptions options) : base(options) { }
}