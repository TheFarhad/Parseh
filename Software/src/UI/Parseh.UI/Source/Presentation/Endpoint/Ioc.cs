namespace Parseh.UI;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using static Microsoft.Extensions.Hosting.Host;

public sealed class Ioc
{
    readonly IHost _host;
    static Ioc _default = default!;
    readonly static Lock _lock = new();
    readonly IServiceProvider _serviceProvider;
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

    internal CortexViewModel CortexViewModel => RequiredService<CortexViewModel>();
    internal INotifierService Notifier => RequiredService<INotifierService>();
    internal IConfiguration Configuration => RequiredService<IConfiguration>();

    Ioc()
    {
        _host = Build();
        _serviceProvider = _host.Services;
    }

    public T? Service<T>() where T : notnull => _serviceProvider.GetService<T>();
    public T RequiredService<T>() where T : notnull => _serviceProvider.GetRequiredService<T>();
    public IEnumerable<T> Services<T>() where T : notnull => _serviceProvider.GetServices<T>();

    #region Services

    IHost Build() => CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();

    void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<CortexViewModel>();
        services.AddSingleton<Layout>();
        services.AddTransient<INotifierService, NotifierService>();
    }

    #endregion
}
