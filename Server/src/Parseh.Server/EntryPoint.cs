namespace Parseh.Server;

using Carter;
using Core.AppService.Shared;
using Infra.Persistence.EF.Shared;

internal static class EntryPoint
{
    internal static void Host(string[] args, bool useResponseCompression = false)
        => WebApplication
            .CreateBuilder(args)
            .ConfigureServices()
            .Configure(useResponseCompression);

    static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .ApplicationLayerDependencies()
            .InfraPersistenceLayerDependencies(configuration)
            .EndpointDependencies(configuration)
            ;

        return builder.Build();
    }

    static void Configure(this WebApplication app, bool useResponseCompression = false)
    {
        if (useResponseCompression)
            app.UseResponseCompression();


        var devEnvironment = app.Environment.IsDevelopment();

        if (devEnvironment)
            app.MapOpenApi();

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors("WindowsApp");
        app.MapCarter();

        app.Run();
    }
}
