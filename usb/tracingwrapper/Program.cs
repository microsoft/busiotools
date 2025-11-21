using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static int Main()
    {
        string? exeDir = Path.GetDirectoryName(GetRealExecutablePath());

        string cmd = exeDir + "\\BusesTrace.cmd";
        if (!File.Exists(cmd))
        {
            Console.Error.WriteLine("BusesTrace.cmd not found next to launcher." + cmd);
            return 1;
        }

        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c \"" + cmd + "\"")
        {
            WorkingDirectory = exeDir,
            UseShellExecute = false
        };

        Process? p = Process.Start(psi);
        p?.WaitForExit();

        return p?.ExitCode ?? 1;
    }

    public static string GetRealExecutablePath()
    {
        // .NET 6+: returns the path the process was started with (may be a symlink).
        string path = Environment.ProcessPath ?? System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName;

        FileInfo fi = new FileInfo(path);

        // If the binary path itself is a symlink, resolve the final target.
        // true => resolve chain to the final target
        FileSystemInfo? target = fi.ResolveLinkTarget(returnFinalTarget: true);

        return target?.FullName ?? fi.FullName;
    }
}