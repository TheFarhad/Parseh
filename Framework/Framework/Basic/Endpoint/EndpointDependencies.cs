namespace Framework;

using System.IO.Compression;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

public static class EndpointDependencies
{
    public static IServiceCollection FrameworkEndpointDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpContextAccessor()
            .FilingServiceDependencies()
            .GZipLargeObjectCompressionDependencies()
            .SerializationServicesDependencies()
            .EncryptionServicesDependencies()
            .IdentityServicesDependencies(configuration)
            .Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            })
           .Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            })
           .AddResponseCompression(_ =>
            {
                _.EnableForHttps = true;
                _.Providers.Add<GzipCompressionProvider>();
                _.Providers.Add<BrotliCompressionProvider>();
            })
           //.AddExceptionHandler<ExceptionManager>()

           //.AddHostedService<RegistryHostedService>()

           .AddSwaggerGen()
           .AddEndpointsApiExplorer()
           .AddControllers()
           .AddJsonOptions(_ =>
           {
               var options = _.JsonSerializerOptions;
               options.Converters.Add(new JsonStringEnumConverter());
               options.PropertyNameCaseInsensitive = false; // case insensitive
               options.PropertyNamingPolicy = null; // set proerty in Pascal Case mode
               options.WriteIndented = true;
           });

        // OR => to config json http response 
        //service.Configure<JsonOptions>(_ =>
        //{
        //    _.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
        //    _.JsonSerializerOptions.PropertyNamingPolicy = null; // set proerty in Pascal Case mode
        //    _.JsonSerializerOptions.WriteIndented = true; // تو رفتگی ها رعایت شود
        //});

        /*
         بحث فشرده سازی ریسپانس در دات نت ۹ به صورت پیش فرض و بهینه تر اعمال شده است

        استفاده از روش جیزیپ و براتلی به این روش، بر روی تمامی ریکوئست ها اعمال می شود و سربار زیادی روی سی پی یو ایجا می کند

        شاید نخواستیم برای تمامی ریکوئست ها چنین کاری کنیم

        این مساله در دات نت ۹ حل شده است
        با استفاده از app.MapStaticAssets()
        با این کار فقط فایل های استاتیک به صورت فشرده ارسال می شوند
         */

        return services;
    }

    public static async Task DbContextAutoMigrator<TCommandStoreContext>(this WebApplication app)
       where TCommandStoreContext : CommandStoreContext<TCommandStoreContext>
    {
        using var scope = app.Services.CreateScope();
        await scope
                .ServiceProvider
                .GetRequiredService<TCommandStoreContext>()
                .Database
                .MigrateAsync();
    }
}
