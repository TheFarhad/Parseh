namespace Framework;

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

internal static class Processor
{
    class WindowsProcess
    {
        public int ProcessId { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
    }

    public static void KillBy(int port)
    {
        var process = GetAllProcess().FirstOrDefault(_ => _.Port == port);
        if (process is not null)
        {
            Process.
                GetProcessById(process.ProcessId)
               .Kill();
        }
    }

    public static int? FindProcessIdBy(int port)
        => GetAllProcess()
            .FirstOrDefault(a => a.Port == port)?
            .ProcessId;

    private static List<WindowsProcess> GetAllProcess()
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "netstat.exe",
            Arguments = "-a -n -o",
            WindowStyle = ProcessWindowStyle.Maximized,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        var process = Process.Start(processInfo);
        var soStream = process.StandardOutput;

        var output = soStream.ReadToEnd();
        if (process.ExitCode != 0)
            throw new Exception("something broken");

        var lines = Regex.Split(output, "\r\n");
        var result = CreateProcessFromOutput(lines);
        return result;
    }

    private static List<WindowsProcess> CreateProcessFromOutput(string[] lines)
    {
        List<WindowsProcess> result = [];
        lines
            .ToList()
            .ForEach(_ =>
            {
                if (!_.Trim().StartsWith("Proto"))
                {
                    var parts = _.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var len = parts.Length;
                    if (len > 2)
                        result.Add(new WindowsProcess
                        {
                            Protocol = parts[0],
                            Port = int.Parse(parts[1].Split(':').Last()),
                            ProcessId = int.Parse(parts[len - 1])
                        });
                }
            });

        // Old
        //foreach (var _ in lines)
        //{
        //    if (_.Trim().StartsWith("Proto"))
        //        continue;

        //    var parts = _.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //    var len = parts.Length;
        //    if (len > 2)
        //        result.Add(new WindowsProcess
        //        {
        //            Protocol = parts[0],
        //            Port = int.Parse(parts[1].Split(':').Last()),
        //            ProcessId = int.Parse(parts[len - 1])
        //        });
        //}
        return result;
    }
}
