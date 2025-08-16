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
        const string CommandConnectionString = "ParsehCommandDbConnectionString";
        const string QueryConnectionString = "ParsehQueryDbConnectionString";

        services
            .FrameworkPersistenceDependencies(_assembly)
            .DbStores<ParsehCommandDbStore, ParsehQueryDbStore>(configuration, CommandConnectionString, QueryConnectionString, [new SaveInterceptor()])
            .AddScoped<ITokenService, TokenService>()
            .Configure<JwtOption>(configuration.GetSection("JwtOptions"))
            ;

        return services;
    }
}
