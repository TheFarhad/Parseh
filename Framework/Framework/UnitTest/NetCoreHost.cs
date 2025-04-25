namespace Framework;

using System.Diagnostics;

public class NetCoreHost : IHostProcess
{
    private readonly HostOptions _options;
    public string BaseUrl => $"https://localhost:{_options.Port}";
    private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);

    public NetCoreHost(HostOptions options) =>
           _options = options;

    public void Start()
    {
        ShutDown();
        var startInfo = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            Arguments = $"run --project \"{_options.CsProjectPath}\"",
        };
        var process = Process.Start(startInfo);

        process.ErrorDataReceived += ProcessOnErrorDataReceived;
        process.OutputDataReceived += ProcessOnOutputDataReceived;

        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        _resetEvent.WaitOne();
    }

    public void ShutDown() =>
              Processor
                .KillBy(_options.Port);

    private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is { } && e.Data.Contains("Now listening on", StringComparison.OrdinalIgnoreCase))
            _resetEvent.Set();
    }

    private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Data))
            throw new Exception(e.Data);
    }
}