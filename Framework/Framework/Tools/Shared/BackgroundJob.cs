namespace Framework;

public abstract class BackgroundJob : BackgroundService { }

public interface IHostedLifespanService : IHostedLifecycleService
{
    async Task IHostedLifecycleService.StartingAsync(CancellationToken token)
         => await Task.CompletedTask;

    async Task IHostedService.StartAsync(CancellationToken token)
       => await Task.CompletedTask;

    async Task IHostedLifecycleService.StartedAsync(CancellationToken token)
         => await Task.CompletedTask;

    async Task IHostedLifecycleService.StoppingAsync(CancellationToken token)
        => await Task.CompletedTask;

    async Task IHostedService.StopAsync(CancellationToken token)
      => await Task.CompletedTask;

    async Task IHostedLifecycleService.StoppedAsync(CancellationToken token)
         => await Task.CompletedTask;
}

public static class Registerer
{
    public static IServiceCollection Dependencies(this IServiceCollection services, Assembly assembly, Type assignableTo, ServiceLifetime lifetime)
         => services.Dependencies([assembly], [assignableTo], lifetime);

    public static IServiceCollection Dependencies(this IServiceCollection services, List<Assembly> assemblies, List<Type> assignableTo, ServiceLifetime lifetime)
         => services.ScrutorRegisterer(assemblies, assignableTo, lifetime);

    private static IServiceCollection ScrutorRegisterer(this IServiceCollection source, List<Assembly> assemblies, List<Type> assignableTo, ServiceLifetime lifeTime)
        => source
             .Scan(typeSelector =>
                 typeSelector.FromAssemblies(assemblies)
                 .AddClasses(typeFilter => typeFilter.AssignableToAny(assignableTo))
                 .AsImplementedInterfaces()
                 .WithLifetime(lifeTime)
             );
}
