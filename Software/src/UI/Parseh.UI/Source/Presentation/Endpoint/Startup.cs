using Microsoft.Extensions.Hosting;

namespace Parseh.UI;

internal static class Startup
{
    [STAThread]
    internal static void Main(string[] args)
    {
        App.Start();
    }
}
