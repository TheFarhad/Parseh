namespace Framework;

public record struct HostOptions
{
    public string CsProjectPath { get; init; }
    public int Port { get; init; }
    public string Endpoint { get; }

    public HostOptions(string ssProjectPath, int port)
    {
        CsProjectPath = ssProjectPath;
        Port = port;
        Endpoint = $"https://localhost:{Port}/api";
    }
}


static class HostOptionsExample
{
    // TODO: change based-on current project
    public static int Port = 5001;
    public static readonly string Endpoint = $"https://localhost:{Port}/api";

    // TODO: change based-on current project
    public const string CsProjectPath =
        @"C:\Hossein\Packages\Test (TDD&BDD)\examples\12-TDD-Academy\Academy.Presentation\Academy.Presentation.csproj";
}
