namespace Framework;

public static class AppServiceDependenciesExtention
{
    public static IServiceCollection FrameworkAppServiceDependencies(this IServiceCollection services, Assembly assembly)
        => services.Handlers(assembly).Pipes(assembly);

    static IServiceCollection Handlers(this IServiceCollection services, Assembly assembly)
        => services
            .Dependencies(
                [assembly],
                [typeof(IRequestHandler<>), typeof(IRequestHandler<,>), typeof(IDomainEventHandler<>)],
                ServiceLifetime.Transient
            );

    static IServiceCollection Pipes(this IServiceCollection services, Assembly assembly)
        => services.DomainEventController().RequestPipe();

    static IServiceCollection DomainEventController(this IServiceCollection services)
        => services.AddScoped<DomainEventPipe>();

    static IServiceCollection RequestPipe(this IServiceCollection services)
    {
        services.AddScoped<RequestValidator>();
        services.AddScoped<RequestExecutor>();
        services.AddScoped<RequestController>(_ =>
        {
            var validator = _.GetRequiredService<RequestValidator>();
            var handler = _.GetRequiredService<RequestExecutor>();
            validator.OnChain(handler);
            return validator;
        });
        return services;
    }
}
