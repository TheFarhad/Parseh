namespace Parseh.UI.Source.Presentation.Shared;

public static class Dependencies
{
    public static IServiceCollection EndpointLayerDependencies(this IServiceCollection services)
    {
        // TODO: در صورت نیاز، سرویس های فریمورک نیز اضافه شود

        services
            .AddSingleton<CortexViewModel>()
            .AddSingleton<Layout>() // TODO: اگر بهتر است ویومدل آن را هم به صورت سینگلتون از طریق کانستراکتور اضافه کن
            .AddTransient<SigninViewModel>()
            .AddTransient<Signin>()
            .AddTransient<ChatViewModel>()
            .AddTransient<Chat>()
            ;

        return services;
    }
}
