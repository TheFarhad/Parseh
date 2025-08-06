namespace Parseh.UI;

using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using static Microsoft.Extensions.Hosting.Host;

public sealed class Ioc
{
    readonly IHost _host;
    static Ioc _default = default!;
    readonly static Lock _lock = new();
    public static Ioc Default
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

    internal IServiceProvider Provider { get; private set; }
    internal IConfiguration Configuration => RequiredService<IConfiguration>();
    internal INotifierService Notifier => RequiredService<INotifierService>();
    internal CortexViewModel CortexViewModel => RequiredService<CortexViewModel>();

    Ioc()
    {
        _host = Build();
        Provider = _host.Services;
    }

    public T? Service<T>() where T : notnull => Provider.GetService<T>();
    public T RequiredService<T>() where T : notnull => Provider.GetRequiredService<T>();
    public IEnumerable<T> Services<T>() where T : notnull => Provider.GetServices<T>();

    #region Services

    IHost Build() => CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();

    void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<CortexViewModel>();

        //services.AddSingleton<Layout>();
        services.AddTransient<Layout>();

        services.AddTransient<INotifierService, NotifierService>();
    }

    public Ioc Start()
    {
        _host.Start();
        return this;
    }
    public async Task<Ioc> StopAsync()
    {
        await _host.StopAsync();
        return this;
    }

    #endregion
}
