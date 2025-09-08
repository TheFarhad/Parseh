namespace Framework;

using Microsoft.Extensions.DependencyInjection;

public static class FileDependencies
{
    public static IServiceCollection FilingServiceRegistery(this IServiceCollection source)
        => source
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<IUploadService, UploadService>();
}
