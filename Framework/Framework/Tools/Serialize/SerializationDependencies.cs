namespace Framework;

using Microsoft.Extensions.DependencyInjection;

public static class SerializationDependencies
{
    public static IServiceCollection SerializationServicesDependencies(this IServiceCollection source, SerializeFlag flag = SerializeFlag.NewtonSoft)
        => flag switch
        {
            SerializeFlag.NetJson => source.AddSingleton<ISerializeService, NetJsonSerializeService>(),
            SerializeFlag.NewtonSoft => source.AddSingleton<ISerializeService, NewtonSoftSerializeService>(),
            SerializeFlag.Json => source.AddSingleton<ISerializeService, JsonSerializeService>(),
            _ => source.AddSingleton<ISerializeService, NewtonSoftSerializeService>()
        };
}
