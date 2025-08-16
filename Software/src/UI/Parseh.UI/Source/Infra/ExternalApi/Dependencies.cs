namespace Parseh.UI.Source.Infra.ExternalApi;

using System.Reflection;

public static class Dependencies
{
    static Assembly _assembly => typeof(Dependencies).Assembly;

    public static IServiceCollection InfraExternalApiLayerDependencies(this IServiceCollection services)
    {
        services
            .FrameworkPersistenceDependencies(_assembly)
            .AddScoped<UserService>();

        // TODO: other dependencies can be added here

        return services;
    }
}
