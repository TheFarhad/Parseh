namespace Framework;

using System.IO.Compression;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

public static class EndpointDependencies
{
    public static IServiceCollection FrameworkEndpointDependencies(this IServiceCollection services, IConfiguration configuration, bool useController = true)
    {
        if (useController)
            services.AddControllers();

        services
            .AddHttpContextAccessor()
            .FilingServiceDependencies()
            .GZipLargeObjectCompressionDependencies()
            .SerializationServicesDependencies()
            .EncryptionServicesDependencies()
            .IdentityServicesDependencies(configuration)
            .Configure<GzipCompressionProviderOptions>(options =>
            {
                // مصرف بالاتر cpu
                // زمان بیشتر فشرده سازی
                // حجم خروجی کوچکتر
                // مناسب برای محتواهای حجیم یا وقتی که پهنای‌باند محدود است
                options.Level = CompressionLevel.Optimal;

                // مصرف پائین تر cpu
                // زمان کمتر فشرده سازی
                // حجم خروجی بزرگتر
                // مناسب برای APIهای با درخواست‌‌های پرتعداد و حساس به تأخیر
                //options.Level = CompressionLevel.Fastest;
            })
           .Configure<BrotliCompressionProviderOptions>(options =>
            {
                // مصرف بالاتر cpu
                // زمان بیشتر فشرده سازی
                // حجم خروجی کوچکتر
                // مناسب برای محتواهای حجیم یا وقتی که پهنای‌باند محدود است
                options.Level = CompressionLevel.Optimal;

                // مصرف پائین تر cpu
                // زمان کمتر فشرده سازی
                // حجم خروجی بزرگتر
                // مناسب برای APIهای با درخواست‌‌های پرتعداد و حساس به تأخیر
                //options.Level = CompressionLevel.Fastest;
            })
           .AddResponseCompression(_ =>
            {
                _.EnableForHttps = true;
                _.Providers.Add<GzipCompressionProvider>();
                _.Providers.Add<BrotliCompressionProvider>();
                _.MimeTypes = ResponseCompressionDefaults
                                  .MimeTypes
                                  .Concat(new[] { "application/json", "text/event-stream" });

            })
           .AddExceptionHandler<ExceptionManager>()

           //.AddHostedService<RegistryHostedService>()

           .AddSwaggerGen()
           .AddEndpointsApiExplorer()
           ;

        services
            .Configure<JsonOptions>(_ =>
            {
                _.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                _.JsonSerializerOptions.PropertyNamingPolicy = null; // set proerty in Pascal Case mode
                _.JsonSerializerOptions.WriteIndented = true; // تو رفتگی ها رعایت شود
                _.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
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
       where TCommandStoreContext : CommandDbStore<TCommandStoreContext>
    {
        using var scope = app.Services.CreateScope();
        await scope
                .ServiceProvider
                .GetRequiredService<TCommandStoreContext>()
                .Database
                .MigrateAsync();
    }
}
