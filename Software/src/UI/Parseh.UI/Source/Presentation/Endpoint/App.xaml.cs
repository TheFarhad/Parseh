namespace Parseh.UI;

public sealed partial class App : Application
{
    static Dispatcher _dispatcher => Application.Current.Dispatcher;
    public static App Default { get; private set; } = default!;
    public static Window Layout => Default.MainWindow;

    private App() { }

    public static void Start()
    {
        var ioc = NetIoC.Default;
        if (Default is null) Default = new();
        Default.InitializeComponent();
        Default.MainWindow = ioc.GetRequired<Layout>();
        Test();
        Default.Run(Default.MainWindow);
    }

    public static void Dispatch(Action act) => _dispatcher.Invoke(act);
    public static DispatcherOperation<Task> DispatchAsync(Func<Task> func) => _dispatcher.InvokeAsync(func);
    public static T Resource<T>(string resource) => Application.Current.FindResource(resource).As<T>();

    #region Private Functionality

    static void Test()
    {

    }

    #endregion
}
