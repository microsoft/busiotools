using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

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

        string?[] embeddedHashes = typeof(Program).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Where(attribute => attribute.Key == "BusesTraceSha256")
            .Select(attribute => attribute.Value)
            .ToArray();

        if (embeddedHashes.Length != 1 ||
            embeddedHashes[0] is not string expectedHash ||
            expectedHash.Length != 64 ||
            !expectedHash.All(Uri.IsHexDigit))
        {
            Console.Error.WriteLine("Missing or invalid embedded BusesTraceSha256; refusing to run.");
            return 1;
        }

        try
        {
            // Keep the handle open through execution to deny writes and deletion.
            using FileStream script = new FileStream(cmd, FileMode.Open, FileAccess.Read, FileShare.Read);

            string actualHash = Convert.ToHexString(SHA256.HashData(script));

            if (!string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
            {
                Console.Error.WriteLine("BusesTrace.cmd SHA-256 mismatch; refusing to run.");
                return 1;
            }

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = System.Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\cmd.exe"),
                Arguments = "/c \"" + cmd + "\"",
                WorkingDirectory = exeDir,
                UseShellExecute = false
            };

            using Process? p = Process.Start(psi);
            if (p is null)
            {
                Console.Error.WriteLine("Failed to start BusesTrace.cmd.");
                return 1;
            }

            p.WaitForExit();
            return p.ExitCode;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or CryptographicException)
        {
            Console.Error.WriteLine($"Unable to validate or run BusesTrace.cmd: {ex.Message}");
            return 1;
        }
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