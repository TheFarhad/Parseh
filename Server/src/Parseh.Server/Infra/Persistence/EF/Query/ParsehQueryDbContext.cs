namespace Parseh.Server.Infra.Persistence.EF.Query;

using Microsoft.EntityFrameworkCore;

public sealed class ParsehQueryDbContext : QueryDbStore<ParsehQueryDbContext>
{
    private ParsehQueryDbContext() { }
    public ParsehQueryDbContext(DbContextOptions<ParsehQueryDbContext> options) : base(options) { }
}
