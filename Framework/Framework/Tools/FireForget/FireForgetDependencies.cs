namespace Framework;

using Microsoft.Extensions.DependencyInjection;

public static class FireForgetDependencies
{
    public static IServiceCollection FireForgetServiceDependencies(this IServiceCollection source)
        => source.AddSingleton<FireForgetProvider>();
}
