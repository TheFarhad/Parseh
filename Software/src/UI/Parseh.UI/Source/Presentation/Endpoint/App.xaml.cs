using Microsoft.Windows.Themes;

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
        //var pass = "فرهاد کاظمی 123۶۷۵";
        ////var hash = "712770E29CE029994376A35833E8F1672D62FF6DE5B192A4F2070CA55A5B81EAD75E398D380198F71809A3C9A8CC0FD775CDCB64F37B4111A6D31064476B4B4F";
        ////var salt = "94D43D938D58DF754772BDF95674D2CA";

        //var encoder = new ArgonEncyptionService();

        //var result = encoder.Hash(pass);
    }

    #endregion
}
