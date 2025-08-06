using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Framework;

public static class PersistenceDependecies
{
    public static IServiceCollection FrameworkPersistenceDependencies(this IServiceCollection services, Assembly assembly)
        => services.Repositories(assembly).UnitOfWork(assembly);

    public static IServiceCollection DbStores<TCommandStorContext, TQueryStoreContext>(this IServiceCollection services, IConfiguration configuration, string commandDbConnectionStringName, string queryDbConnectionStringName, params IEnumerable<IInterceptor> interceptors)
        where TCommandStorContext : CommandDbStore<TCommandStorContext>
        where TQueryStoreContext : QueryDbStore<TQueryStoreContext>
    {
        var commandConncetionstring = configuration.GetConnectionString(commandDbConnectionStringName)!;
        var queryConncetionstring = configuration.GetConnectionString(commandDbConnectionStringName)!;

        if (commandConncetionstring.IsEmpty() || queryConncetionstring.IsEmpty())
        {
            // TODO: throw new InvalidOperationException($"Connection strings for '{commandDbConnectionStringName}' or '{queryDbConnectionStringName}' are not configured.");
        }
        else
        {
            services
                  .CommandStoreContext<TCommandStorContext>(commandConncetionstring!, interceptors)
                  .QueryStoreContext<TQueryStoreContext>(queryConncetionstring!);
        }
        return services;
    }

    static IServiceCollection Repositories(this IServiceCollection services, Assembly assembly)
      => services.Dependencies([assembly], [typeof(ICommandRepository<,>), typeof(IQueryRepository)], ServiceLifetime.Transient);

    static IServiceCollection UnitOfWork(this IServiceCollection services, Assembly assembly)
        => services.Dependencies(assembly, typeof(IUnitOfWork), ServiceLifetime.Scoped);

    static IServiceCollection CommandStoreContext<TCommandStorContext>(this IServiceCollection services, string connectionstring, params IEnumerable<IInterceptor> interceptors)
        where TCommandStorContext : CommandDbStore<TCommandStorContext>
        => services.AddDbContext<TCommandStorContext>(_ =>
        {
            _
            .UseSqlServer(connectionstring)
            .AddInterceptors(interceptors)
            .LogOptions();
        });

    static IServiceCollection QueryStoreContext<TQueryStorContext>(this IServiceCollection services, string connectionstring)
        where TQueryStorContext : QueryDbStore<TQueryStorContext>
        => services.AddDbContext<TQueryStorContext>(_ =>
        {
            _.UseSqlServer(connectionstring).LogOptions();
        });

    static void LogOptions(this DbContextOptionsBuilder source)
    {
#if DEBUG
        source
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
#endif
    }
}
