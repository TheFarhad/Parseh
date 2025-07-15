namespace Parseh.UI;

public sealed partial class App : Application
{
    static Dispatcher _dispatcher => Application.Current.Dispatcher;
    public static App Default { get; private set; } = default!;
    public static Window Layout => Default.MainWindow;

    private App() { }

    public static void Start()
    {
        var ioc = Ioc.Default.Start();
        if (Default is null) Default = new();

        Setup();

        Default.InitializeComponent();
        Default.MainWindow = ioc.RequiredService<Layout>();
        Default.Run(Default.MainWindow);
    }

    static void Setup()
    {
        // TODO: complete this. for register services and ...

        Test();
    }

    public static void Dispatch(Action act) => _dispatcher.Invoke(act);
    public static DispatcherOperation<Task> DispatchAsync(Func<Task> func) => _dispatcher.InvokeAsync(func);
    public static T Resource<T>(string resource) => Application.Current.FindResource(resource).As<T>();

    protected override async void OnExit(ExitEventArgs e)
    {
        await Ioc.Default.StopAsync();
        base.OnExit(e);
    }

    #region Private Functionality

    static void Test()
    {
    }

    #endregion
}
