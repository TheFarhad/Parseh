namespace Parseh.Server.Infra.Persistence.EF.Shared;

using System.Reflection;
using Query;
using Command;
using Core.Contract.Infra.Persistence.Command;

public static class Dependencies
{
    static Assembly _assembly => typeof(Dependencies).Assembly;

    public static IServiceCollection InfraPersistenceLayerDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.FrameworkPersistenceDependencies(_assembly);

        services.AddScoped<ITokenService, TokenService>();
        services.Configure<JwtOption>(configuration.GetSection("JwtOptions"));

        const string CommandConnectionString = "ParsehCommandDbConnectionString";
        const string QueryConnectionString = "ParsehQueryDbConnectionString";
        services
            .DbStores<ParsehCommandDbStore, ParsehQueryDbStore>(configuration, CommandConnectionString, QueryConnectionString, []);

        return services;
    }
}
