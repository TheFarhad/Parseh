namespace Framework;

using Microsoft.Extensions.DependencyInjection;

public static class CompressionDependencies
{
    public static IServiceCollection GZipLargeObjectCompressionRegistery(this IServiceCollection source)
        => source.AddSingleton<IBlobCompression, GZipBlobCompression>();
}
