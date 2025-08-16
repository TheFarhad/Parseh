namespace Parseh.UI.Source.Presentation.Shared;

public static class Dependencies
{
    public static IServiceCollection EndpointLayerDependencies(this IServiceCollection services)
    {
        // TODO: در صورت نیاز، سرویس های فریمورک نیز اضافه شود

        services
            .AddSingleton<Layout>()
            .AddScoped<SigninViewModel>();

        return services;
    }
}
