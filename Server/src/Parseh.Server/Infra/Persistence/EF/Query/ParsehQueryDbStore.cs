namespace Parseh.Server.Infra.Persistence.EF.Query;

using Microsoft.EntityFrameworkCore;

public sealed class ParsehQueryDbStore : QueryDbStore<ParsehQueryDbStore>
{

    ParsehQueryDbStore() { }
    public ParsehQueryDbStore(DbContextOptions<ParsehQueryDbStore> options) : base(options) { }
}
