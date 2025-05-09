namespace Parseh.UI;

public sealed partial class App : Application
{
    public static App Self { get; private set; } = default!;

    private App() { }

    public static void Start()
    {
        Test();
        if (Self is null) Self = new();
        Self.InitializeComponent();
        Self.MainWindow = NetCoreIoC.Self.GetRequired<Layout>();
        Self.Run(Self.MainWindow);
    }

    public static void Dispatch(Action act) => Application.Current.Dispatcher.Invoke(act);
    public static DispatcherOperation<Task> Dispatch(Func<Task> func) => Application.Current.Dispatcher.InvokeAsync(func);

    private static void Test()
    {

    }
}
