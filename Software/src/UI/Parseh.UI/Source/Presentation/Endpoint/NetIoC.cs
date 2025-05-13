namespace Parseh.UI;

using System.Threading;
using Microsoft.Extensions.Hosting;
using static Microsoft.Extensions.Hosting.Host;

public sealed class NetIoC
{
    readonly IHost _host;
    static NetIoC _self = default!;
    readonly static Lock _lock = new();
    readonly IServiceProvider _sp;
    public static NetIoC Default
    {
        get
        {
            lock (_lock)
            {
                return _self ??= new();
            }
        }
    }

    NetIoC()
    {
        _host = Build();
        _sp = _host.Services;
    }

    public T? Get<T>() where T : notnull => _sp.GetService<T>();
    public T GetRequired<T>() where T : notnull => _sp.GetRequiredService<T>();
    public IEnumerable<T> GetAll<T>() where T : notnull => _sp.GetServices<T>();

    #region Services

    IHost Build() => CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();

    void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<GenericViewModel>();
        services.AddSingleton<Layout>();
    }

    #endregion
}
