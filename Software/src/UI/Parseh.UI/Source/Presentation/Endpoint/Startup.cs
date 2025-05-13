namespace Parseh.UI;

internal static class Startup
{
    [STAThread]
    internal static void Main(string[] args)
    {
        var host = NetIoC.Default;
        App.Start();
    }
}
