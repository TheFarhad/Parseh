namespace Parseh.UI;

public sealed partial class App : Application
{
    public static App Self { get; private set; } = default!;

    private App() { }

    public static void Start()
    {
        var ico = NetIoC.Default;
        if (Self is null) Self = new();
        Self.InitializeComponent();
        Self.MainWindow = ico.GetRequired<Layout>();
        Test();
        Self.Run(Self.MainWindow);
    }

    public static void Dispatch(Action act) => Application.Current.Dispatcher.Invoke(act);
    public static DispatcherOperation<Task> Dispatch(Func<Task> func) => Application.Current.Dispatcher.InvokeAsync(func);

    static void Test()
    {
    }
}
