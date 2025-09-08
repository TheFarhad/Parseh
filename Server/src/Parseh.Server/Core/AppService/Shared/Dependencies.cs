namespace Parseh.Server.Core.AppService.Shared;

using System.Reflection;

public static class Dependencies
{
    static readonly Assembly assembly = typeof(Dependencies).Assembly;

    public static IServiceCollection ApplicationLayerRegistery(this IServiceCollection services)
    {
        services.FrameworkAppServiceDependencies(assembly);

        // -- [ current dependencies + framework's app-service dependencies ] -- \\
        // ...

        return services;
    }
}
