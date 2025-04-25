namespace Framework;

using Microsoft.Extensions.DependencyInjection;

public static class CompressionDependencies
{
    public static IServiceCollection GZipLargeObjectCompressionDependencies(this IServiceCollection source)
        => source.AddSingleton<IBlobCompression, GZipBlobCompression>();
}
