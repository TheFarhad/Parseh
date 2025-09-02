namespace Framework.WPF;

// BEST PRACTICE
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class RegisterAttribute : Attribute
{
    public RegisterAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        => Lifetime = lifetime;

    public readonly ServiceLifetime Lifetime;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class IgnoreRegisterAttribute : Attribute { }

/// <summary>
/// Automatic registration in DI container By RegisterAttribute
/// </summary>
public static class AttributeRegistrator
{
    /// <summary>
    /// Register All Services with [RegisterAttribute] In Current Assembly
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AutoAttributeRegistrator(this IServiceCollection services)
    {
        Assembly
           .GetExecutingAssembly()
           .Registrator(services);

        return services;
    }

    /// <summary>
    /// Register All Services with [RegisterAttribute] In Any Assembly
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AutoAttributeRegistrator(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            assembly.Registrator(services);
        }
        return services;
    }

    private static void Registrator(this Assembly assembly, IServiceCollection services)
    {
        assembly
            .GetTypes()
            .Where(
                   t => t.IsSubclassOf(typeof(object))
                        && t.IsClass
                        && !t.IsAbstract
                        && !t.IsInterface
                        && !t.IsGenericType
                        && t.GetCustomAttribute<RegisterAttribute>() is not null
                        && t.GetCustomAttribute<IgnoreRegisterAttribute>() is null
                       )
            .ToList()
            .ForEach(t =>
            {
                var liftime = t.GetCustomAttribute<RegisterAttribute>()!.Lifetime;
                var _ = liftime switch
                {
                    ServiceLifetime.Singleton => services.AddSingleton(t),
                    ServiceLifetime.Scoped => services.AddScoped(t),
                    ServiceLifetime.Transient => services.AddTransient(t),
                    _ => services.AddTransient(t),
                };
            });
    }
}