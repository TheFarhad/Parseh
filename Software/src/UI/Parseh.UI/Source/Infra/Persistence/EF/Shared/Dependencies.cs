namespace Parseh.UI.Source.Infra.Persistence.EF.Shared;

using Microsoft.Extensions.Configuration;

public static class Dependencies
{
    static Assembly _assembly => typeof(Dependencies).Assembly;

    public static IServiceCollection InfraPersistenceLayerDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .FrameworkPersistenceDependencies(_assembly) // for repositories and unit of work
                                                         //.AddDbContext<ParsehUICommandDbStore>(option)
            ;

        return services;
    }
}
