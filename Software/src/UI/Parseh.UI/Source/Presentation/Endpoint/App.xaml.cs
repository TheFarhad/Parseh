namespace Parseh.UI;

public sealed partial class App : Application
{
    public static App Self { get; private set; } = default!;

    private App() { }

    public static App Start()
    {
        Test();
        if (Self is null) Self = new();
        Self.InitializeComponent();
        Self.MainWindow = new Layout();
        Self.Run(Self.MainWindow);
        return Self;
    }

    private static void Test()
    {

    }
}
