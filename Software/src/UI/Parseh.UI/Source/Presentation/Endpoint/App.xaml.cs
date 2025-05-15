namespace Parseh.UI;

public sealed partial class App : Application
{
    public static App Default { get; private set; } = default!;
    static Dispatcher _dispatcher => Application.Current.Dispatcher;

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
    public static DispatcherOperation<Task> Dispatch(Func<Task> func) => _dispatcher.InvokeAsync(func);

    static void Test()
    {
    }
}
