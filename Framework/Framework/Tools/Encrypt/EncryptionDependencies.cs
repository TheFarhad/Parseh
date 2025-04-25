namespace Framework;

using Microsoft.Extensions.DependencyInjection;

public static class EncryptionDependencies
{
    public static IServiceCollection EncryptionServicesDependencies(this IServiceCollection source, EncryptFlag flag = EncryptFlag.Rfc)
         => flag switch
         {
             EncryptFlag.Rfc => source.AddSingleton<IEncryptService, RfcEncryptionService>(),
             EncryptFlag.Bcrypt => source.AddSingleton<IEncryptService, BCryptEncryptionService>(),
             _ => source.AddSingleton<IEncryptService, RfcEncryptionService>()
         };
}
