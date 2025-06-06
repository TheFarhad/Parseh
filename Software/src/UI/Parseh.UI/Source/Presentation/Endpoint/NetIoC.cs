namespace Parseh.UI;

using System.Threading;
using Microsoft.Extensions.Hosting;
using static Microsoft.Extensions.Hosting.Host;

public sealed class NetIoC
{
    readonly IHost _host;
    static NetIoC _default = default!;
    readonly static Lock _lock = new();
    readonly IServiceProvider _sp;
    public static NetIoC Default
    {
        get
        {
            if (_default.IsNull())
            {
                lock (_lock)
                {
                    if (_default.IsNull()) _default = new();
                }
            }
            return _default;
        }
    }

    internal CortexViewModel CortexViewModel => GetRequired<CortexViewModel>();
    internal INotifierService Notifier => GetRequired<INotifierService>();

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
        services.AddSingleton<CortexViewModel>();
        services.AddSingleton<Layout>();
        services.AddSingleton<INotifierService, NotifierService>();
    }

    #endregion
}
