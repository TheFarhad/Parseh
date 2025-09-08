namespace Parseh.Server.Infra.Persistence.EF.Command;

using Microsoft.EntityFrameworkCore;
using Core.Domain.Aggregates.Role.Entity;
using Core.Domain.Aggregates.User.Entity;

public sealed class ParsehCommandDbContext : CommandDbStore<ParsehCommandDbContext>
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RoleClaim> RolePermissions => Set<RoleClaim>();

    public ParsehCommandDbContext(DbContextOptions<ParsehCommandDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(GetType().Assembly);

        // -- [ set default schema for this context ] -- \\
        //mb.HasDefaultSchema("");

        base.OnModelCreating(mb);
    }
}