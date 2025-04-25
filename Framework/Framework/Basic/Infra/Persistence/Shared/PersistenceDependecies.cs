using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Framework;

public static class PersistenceDependecies
{
    public static IServiceCollection FrameworkPersistenceDependencies(this IServiceCollection services, Assembly assembly)
        => services.Repositories(assembly).UnitOfWork(assembly);

    private static IServiceCollection Repositories(this IServiceCollection services, Assembly assembly)
        => services.Dependencies([assembly], [typeof(ICommandRepository<,>), typeof(IQueryRepository)], ServiceLifetime.Transient);

    private static IServiceCollection UnitOfWork(this IServiceCollection services, Assembly assembly)
        => services.Dependencies(assembly, typeof(UnitOfWork<>), ServiceLifetime.Scoped);

    public static IServiceCollection StoreContexts<TCommandStorContext, TQueryStoreContext>(this IServiceCollection services, IConfiguration configuration, string connectionStringSection, params IEnumerable<IInterceptor> interceptors)
        where TCommandStorContext : CommandStoreContext<TCommandStorContext>
        where TQueryStoreContext : QueryStoreContext<TQueryStoreContext>
    {
        var conncetionstring = configuration.GetConnectionString(connectionStringSection);

        return services
                .CommandStoreContext<TCommandStorContext>(conncetionstring!, interceptors)
                .QueryStoreContext<TQueryStoreContext>(conncetionstring!);
    }

    private static IServiceCollection CommandStoreContext<TCommandStorContext>(this IServiceCollection services, string connectionstring, params IEnumerable<IInterceptor> interceptors)
        where TCommandStorContext : CommandStoreContext<TCommandStorContext>
        => services.AddDbContext<TCommandStorContext>(_ =>
        {
            _.UseSqlServer(connectionstring).AddInterceptors(interceptors).LogOptions();
        });

    private static IServiceCollection QueryStoreContext<TQueryStorContext>(this IServiceCollection services, string connectionstring)
        where TQueryStorContext : QueryStoreContext<TQueryStorContext>
        => services.AddDbContext<TQueryStorContext>(_ =>
        {
            _.UseSqlServer(connectionstring).LogOptions();
        });

    private static void LogOptions(this DbContextOptionsBuilder source)
    {
#if DEBUG
        source
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
#endif
    }
}
