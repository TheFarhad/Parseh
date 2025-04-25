namespace Framework;

public static class AppServiceDependenciesExtention
{
    public static IServiceCollection FrameworkAppServiceDependencies(this IServiceCollection services, Assembly assembly)
        => services.Handlers(assembly).Pipes(assembly);

    private static IServiceCollection Handlers(this IServiceCollection services, Assembly assembly)
        => services
            .Dependencies([assembly],
            [typeof(IRequestHandler<>), typeof(IRequestHandler<,>), typeof(IDomainEventHandler<>)],
            ServiceLifetime.Transient);

    private static IServiceCollection Pipes(this IServiceCollection services, Assembly assembly)
        => services.DomainEventController().RequestPipe();

    private static IServiceCollection DomainEventController(this IServiceCollection services)
        => services.AddScoped<DomainEventPipe>();

    private static IServiceCollection RequestPipe(this IServiceCollection services)
    {
        services.AddScoped<RequestValidator>();
        services.AddScoped<RequestHandler>();
        services.AddScoped<RequestPipe>(_ =>
        {
            var validator = _.GetRequiredService<RequestValidator>();
            var handler = _.GetRequiredService<RequestHandler>();
            validator.OnChain(handler);
            return validator;
        });
        return services;
    }
}
