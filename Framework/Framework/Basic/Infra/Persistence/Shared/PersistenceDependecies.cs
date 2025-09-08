namespace Framework;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class PersistenceDependecies
{
    public static IServiceCollection FrameworkPersistenceRegistery(this IServiceCollection services, Assembly assembly)
        => services
            .Repositories(assembly)
            .UnitOfWork(assembly);

    public static IServiceCollection DbContexts<TCommandStorContext, TQueryStoreContext>(this IServiceCollection services, IConfiguration configuration, string commandDbConnectionStringName, string queryDbConnectionStringName, params IEnumerable<IInterceptor> interceptors)
        where TCommandStorContext : CommandDbStore<TCommandStorContext>
        where TQueryStoreContext : QueryDbStore<TQueryStoreContext>
    {
        var commandConncetionString = configuration.GetConnectionString(commandDbConnectionStringName)!;
        var queryConncetionString = configuration.GetConnectionString(queryDbConnectionStringName)!;

        if (commandConncetionString.IsEmpty() || queryConncetionString.IsEmpty())
        {
            // TODO: throw new InvalidOperationException($"Connection strings for '{commandDbConnectionStringName}' or '{queryDbConnectionStringName}' are not configured.");
        }
        else
        {
            services
                  .CommandDbContext<TCommandStorContext>(commandConncetionString!, interceptors)
                  .QueryDbContext<TQueryStoreContext>(queryConncetionString!);
        }
        return services;
    }

    private static IServiceCollection Repositories(this IServiceCollection services, Assembly assembly)
      => services
          .Dependencies([assembly], [typeof(ICommandRepository<,>), typeof(IQueryRepository)], ServiceLifetime.Scoped);

    private static IServiceCollection UnitOfWork(this IServiceCollection services, Assembly assembly)
        => services
            .Dependencies(assembly, typeof(IUnitOfWork), ServiceLifetime.Scoped);

    private static IServiceCollection CommandDbContext<TCommandStorContext>(this IServiceCollection services, string connectionstring, params IEnumerable<IInterceptor> interceptors)
        where TCommandStorContext : CommandDbStore<TCommandStorContext>
        => services
            .AddDbContext<TCommandStorContext>(_ =>
            {
                // TODO: شرایط برای استفاده از سایر پرووایدرها هم مهیا شود
                //       مثل sqlite
                _
                .UseSqlServer(connectionstring)
                .AddInterceptors(interceptors)
                .LogOptions();
            });

    private static IServiceCollection QueryDbContext<TQueryStorContext>(this IServiceCollection services, string connectionstring)
        where TQueryStorContext : QueryDbStore<TQueryStorContext>
        => services
            .AddDbContext<TQueryStorContext>(_ =>
            {
                _
                .UseSqlServer(connectionstring)
                .LogOptions();
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
