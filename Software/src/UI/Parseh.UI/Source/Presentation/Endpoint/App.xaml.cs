namespace Parseh.UI;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using static Microsoft.Extensions.Hosting.Host;
using Microsoft.AspNetCore.Mvc;
using Parseh.UI.Source.Infra.ExternalApi;
using Parseh.UI.Source.Core.AppService.Shared;
using Parseh.UI.Source.Presentation.Shared;

//public sealed partial class App : Application
//{
//    static Dispatcher _dispatcher => Application.Current.Dispatcher;
//    public static App Default { get; private set; } = default!;
//    public static Window Layout => Default.MainWindow;

//    private App() { }

//    public static void Start()
//    {
//        var ioc = Ioc.Default.Start();
//        if (Default is null) Default = new();

//        Setup();

//        Default.InitializeComponent();
//        Default.MainWindow = ioc.RequiredService<Layout>();
//        Default.Run(Default.MainWindow);
//    }

//    static void Setup()
//    {
//        // TODO: complete this. for register services and ...

//        Test();
//    }

//    public static void Dispatch(Action act) => _dispatcher.Invoke(act);
//    public static DispatcherOperation<Task> DispatchAsync(Func<Task> func) => _dispatcher.InvokeAsync(func);
//    public static T Resource<T>(string resource) => Application.Current.FindResource(resource).As<T>();

//    protected override async void OnExit(ExitEventArgs e)
//    {
//        await Ioc.Default.StopAsync();
//        base.OnExit(e);
//    }

//    #region Private Functionality

//    static void Test()
//    {

//    }

//    #endregion
//}



public sealed partial class App : Application
{
    private IHost _host;
    private static Dispatcher _dispatcher => Application.Current.Dispatcher;

    public static IConfiguration Configuration { get; private set; }
    public static Layout Layout;

    public App() => Init();

    #region Public Functionality

    public static void Dispatch(Action act)
        => _dispatcher.Invoke(act);

    public static DispatcherOperation<Task> DispatchAsync(Func<Task> func)
        => _dispatcher.InvokeAsync(func);

    public static T Resource<T>(string resource)
        => Application.Current.FindResource(resource).As<T>();

    #endregion

    #region App Override Functionality

    protected override async void OnStartup(StartupEventArgs e)
    {
        // سرویس های که
        // IHostedService
        // رو پیاده سازی کردن، اجرا میکنه
        await _host.StartAsync();
        RunApplication();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync();
        }
        base.OnExit(e);
    }

    #endregion

    #region Private Functionality

    private void Init()
    {
        InitHost();
        GlobalExceptionHandling();
        InitializeComponent();
    }

    private void InitHost()
    {
        Configuration = InitCofiguration();
        _host = ConfigHost();
    }

    private IConfiguration InitCofiguration()
        => new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             // Optional: false => مشخص می کند که فایل اپ ستینگ حتما باید وجود داشته باشد
                             //                    چون برنامه می خواهد از تنظیمات آن استفاده کند
                             //                    اگر نباشد، برنامه خطا میدهد
                             .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                             .AddEnvironmentVariables()
                             .Build();

    private IHost ConfigHost()
        => CreateDefaultBuilder()
                           .ConfigureAppConfiguration((context, config) =>
                           {
                               config.AddConfiguration(Configuration);
                           })
                          .ConfigureServices(ConfigureServices)
                          .Build();

    void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services
            .AppServiceLayerDependencies()
            .InfraExternalApiLayerDependencies()
            .EndpointLayerDependencies()

            // بهتر است در لایه مربوط به خوش قرار داده شود 
            .AddTransient<INotifierService, NotifierService>()
            .AddSingleton<IConfiguration>(Configuration)
            ;

#if DEBUG
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.AddDebug();
            configure.SetMinimumLevel(LogLevel.Information);
        });
#endif
    }

    private void GlobalExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException!;
    }

    private void RunApplication()
    {
        MainWindow = Layout = _host!.Services.GetRequiredService<Layout>();
        MainWindow.Show();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {

    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {

    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {

    }

    #endregion
}
