namespace Framework;

public static class AppServiceDependenciesExtention
{
    public static IServiceCollection FrameworkAppServiceDependencies(this IServiceCollection services, Assembly assembly)
        => services
            .Handlers(assembly)
            .Pipes(assembly);

    private static IServiceCollection Handlers(this IServiceCollection services, Assembly assembly)
        => services
            .CommandHandlers(assembly)
            .QueryHandlers(assembly)
            .EventHandlers(assembly);

    private static IServiceCollection CommandHandlers(this IServiceCollection services, Assembly assembly)
      => services
          .Dependencies(
              [assembly],
              [typeof(ICommandHandler<>), typeof(ICommandHandler<,>)],
              ServiceLifetime.Transient
          );

    private static IServiceCollection QueryHandlers(this IServiceCollection services, Assembly assembly)
      => services
          .Dependencies(
              [assembly],
              [typeof(IQueryHandler<,>)],
              ServiceLifetime.Scoped
          );

    private static IServiceCollection EventHandlers(this IServiceCollection services, Assembly assembly)
      => services
          .Dependencies(
              [assembly],
              [typeof(IQueryHandler<,>)],
              ServiceLifetime.Transient
          );

    private static IServiceCollection Pipes(this IServiceCollection services, Assembly assembly)
        => services.DomainEventController().RequestPipe();

    private static IServiceCollection DomainEventController(this IServiceCollection services)
        => services.AddScoped<DomainEventPipe>();

    private static IServiceCollection RequestPipe(this IServiceCollection services)
    {
        services.AddScoped<RequestValidator>();
        services.AddScoped<RequestExecutor>();
        services.AddScoped<Responser>(_ =>
        {
            var validator = _.GetRequiredService<RequestValidator>();
            var handler = _.GetRequiredService<RequestExecutor>();
            validator.OnChain(handler);
            return validator;
        });
        return services;
    }
}
