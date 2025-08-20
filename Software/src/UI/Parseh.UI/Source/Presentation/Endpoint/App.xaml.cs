namespace Parseh.UI;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using static Microsoft.Extensions.Hosting.Host;
using Source.Presentation.Shared;
using Source.Core.AppService.Shared;
using Source.Infra.ExternalApi.Shared;
using Source.Infra.Persistence.EF.Shared;

public sealed partial class App : Application
{
    private IHost _host = default!;
    private IConfiguration _configuration = default!;
    private static IServiceProvider _provider = default!;
    private static Dispatcher _dispatcher => Application.Current.Dispatcher;

    public static Layout Layout => Application.Current.MainWindow.As<Layout>();
    public static CortexViewModel Cortex => _provider.GetRequiredService<CortexViewModel>();

    public App()
        => Init();

    #region Public Functionality

    public static void Dispatch(Action act)
        => _dispatcher.Invoke(act);

    public static DispatcherOperation<Task> DispatchAsync(Func<Task> func)
        => _dispatcher.InvokeAsync(func);

    public static T Resource<T>(string resource)
        => Application.Current.FindResource(resource).As<T>();

    public static T? Service<T>() where T : notnull => _provider.GetService<T>();
    public static IEnumerable<T> Services<T>() where T : notnull => _provider.GetServices<T>();
    public static T RequiredService<T>() where T : notnull => _provider.GetRequiredService<T>();

    #endregion

    #region App Override Functionality

    protected override async void OnStartup(StartupEventArgs e)
    {
        // سرویس های که
        // IHostedService
        // رو پیاده سازی کردن، اجرا میکنه
        await _host.StartAsync();
        Application.Current.Resources["CortexViewModel"] = App.Cortex;
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
        InitializeComponent();
        _configuration = BuildCofiguration();
        _host = BuildHost();
        _provider = _host.Services;
        GlobalExceptionHandling();
    }

    private IConfiguration BuildCofiguration()
        => new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             // Optional: false => مشخص می کند که فایل اپ ستینگ حتما باید وجود داشته باشد
                             //                    چون برنامه می خواهد از تنظیمات آن استفاده کند
                             //                    اگر نباشد، برنامه خطا میدهد
                             .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                             .AddEnvironmentVariables()
                             .Build();

    private IHost BuildHost()
        => CreateDefaultBuilder()
                           .ConfigureAppConfiguration((context, config) =>
                           {
                               config.AddConfiguration(_configuration);
                           })
                          .ConfigureServices(ConfigureServices)
                          .Build();

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services
            .AddSingleton<IConfiguration>(_configuration)

            .AppServiceLayerDependencies()
            .InfraExternalApiLayerDependencies()
            .InfraPersistenceLayerDependencies(_configuration)
            .EndpointLayerDependencies()

            // بهتر است در لایه مربوط به خوش قرار داده شود 
            .AddTransient<INotifierService, NotifierService>()
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
        MainWindow = RequiredService<Layout>();
        MainWindow.Show();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // برای گرفتن تمام اکسپشن های کنترل نشده در هر ترد 
        var logger = RequiredService<ILogger<App>>();
        var exception = e.ExceptionObject as Exception;
        logger.LogCritical(exception, "An unhandled exception occurred across all threads: {Message}", exception?.Message);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // برای گرفتن تمام اکسپشن های کنترل نشده در ترد اصلی 
        var logger = RequiredService<ILogger<App>>();
        logger.LogError(e.Exception, "An unhandled exception occurred on the UI thread: {Message}", e.Exception.Message);

        // جلوگیری از کرش فوری برنامه
        e.Handled = true;

        var notifier = RequiredService<INotifierService>();
        notifier.MessageboxNotifyAsync(new MessageboxViewModel
        {
            Title = "Error",
            Message = $"An unexpected error occurred: {e.Exception.Message}\n\nApplication might become unstable. Please restart.",
            OkText = "Ok",
            Type = DialogMessageType.Error
        });
    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        // برای گرفتن اکسپشن های مشاهده نشده از تسک های ناهمزمان
        var logger = RequiredService<ILogger<App>>();
        logger.LogWarning(e.Exception, "An unobserved task exception occurred: {Message}", e.Exception.Message);

        // استثنا را به عنوان مشاهده شده علامت‌گذاری می‌کند تا از خاتمه فرآیند جلوگیری شود
        e.SetObserved();
    }

    #endregion
}

// TODO:
// یک کلاس یوزر با معماری دیدیدی
// یک یوزر ریپوزیتوری
// کامندهای مورد نیاز برای ثبت یوزر
// کوئری های مورد نیاز برای واکشی اطلاعات یوزر 
// کامن هندلر ها و کوئری هندلرهای مورد نیاز
// فقط یک دی بی کانتکست
// اینجکت کردن ریکويست کنترلر در ویومدل های مورد نیاز
