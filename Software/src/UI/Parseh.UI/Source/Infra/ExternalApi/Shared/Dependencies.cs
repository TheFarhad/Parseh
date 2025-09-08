namespace Parseh.UI.Source.Infra.ExternalApi.Shared;
using Parseh.UI.Source.Infra.ExternalApi;

using System.Reflection;

public static class Dependencies
{
    static Assembly _assembly => typeof(Dependencies).Assembly;

    public static IServiceCollection InfraExternalApiLayerDependencies(this IServiceCollection services)
    {
        services
            .FrameworkPersistenceRegistery(_assembly)
            .AddScoped<UserService>();

        // TODO: other dependencies can be added here

        return services;
    }
}
