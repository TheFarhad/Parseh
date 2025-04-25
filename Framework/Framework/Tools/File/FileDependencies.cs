namespace Framework;

using Microsoft.Extensions.DependencyInjection;

public static class FileDependencies
{
    public static IServiceCollection FilingServiceDependencies(this IServiceCollection source)
        => source
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<IUploadService, UploadService>();
}
