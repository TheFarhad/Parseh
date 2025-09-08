namespace Framework.Presentation;

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}

public static class EndpointExtension
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        assembly
           .GetTypes()
           .Where(t =>
                   t.IsAssignableTo(typeof(IEndpoint))
                   && t.IsClass
                   && !t.IsAbstract
                   && !t.IsInterface
                   && !t.IsGenericType
                  )
            .ToList()
            .ForEach(_ => services.AddTransient(typeof(IEndpoint), _));

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        app
           .Services
           .GetServices<IEndpoint>()
           .ToList()
           .ForEach(_ => _.Map(app));

        return app;
    }
}
