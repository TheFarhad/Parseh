namespace Framework;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class EncryptionRegiteryExtension
{
    //public static IServiceCollection EncryptionServicesRegistery(this IServiceCollection services, EncryptFlag flag = EncryptFlag.Rfc)
    //     => flag switch
    //     {
    //         EncryptFlag.Rfc => services.AddSingleton<IEncryptService, RfcEncryptionService>(),
    //         EncryptFlag.Bcrypt => services.AddSingleton<IEncryptService, BCryptEncryptionService>(),
    //         _ => services.AddSingleton<IEncryptService, RfcEncryptionService>()
    //     };

    public static IServiceCollection EncryptionServicesRegistery(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<ArgonEncryptionOption>(configuration.GetSection("ArgonEncryptionOption"))
            .AddSingleton<IEncryptionService, ArgonEncryptionService>();

        return services;
    }
}
