param (
    [string]$outputPath,
    [string]$fileList
)

Add-Type @"
using System;
using System.IO;
using System.Runtime.InteropServices;

public static class DumpHelper
{
    public static class MINIDUMPTYPE
    {
        public const int MiniDumpNormal = 0x00000000;
        public const int MiniDumpWithDataSegs = 0x00000001;
        public const int MiniDumpWithFullMemory = 0x00000002;
        public const int MiniDumpWithHandleData = 0x00000004;
        public const int MiniDumpFilterMemory = 0x00000008;
        public const int MiniDumpScanMemory = 0x00000010;
        public const int MiniDumpWithUnloadedModules = 0x00000020;
        public const int MiniDumpWithIndirectlyReferencedMemory = 0x00000040;
        public const int MiniDumpFilterModulePaths = 0x00000080;
        public const int MiniDumpWithProcessThreadData = 0x00000100;
        public const int MiniDumpWithPrivateReadWriteMemory = 0x00000200;
        public const int MiniDumpWithoutOptionalData = 0x00000400;
        public const int MiniDumpWithFullMemoryInfo = 0x00000800;
        public const int MiniDumpWithThreadInfo = 0x00001000;
        public const int MiniDumpWithCodeSegs = 0x00002000;
    }

    [DllImport("dbghelp.dll")]
    public static extern bool MiniDumpWriteDump(IntPtr hProcess, 
                                                Int32 ProcessId, 
                                                IntPtr hFile, 
                                                int DumpType, 
                                                IntPtr ExceptionParam, 
                                                IntPtr UserStreamParam, 
                                                IntPtr CallackParam);

    public static void CreateMiniDump(String dumpFilePath, IntPtr processHandle, Int32 processId)
    {
        var stream = new System.IO.FileStream(dumpFilePath, System.IO.FileMode.Create);
        MiniDumpWriteDump(processHandle, processId, stream.SafeFileHandle.DangerousGetHandle(), MINIDUMPTYPE.MiniDumpWithFullMemory, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        stream.Close();
    }
}
"@;

function ParseAndDump([string]$desc, [string]$module) {

    Write-Host "Parse and Dump: " $desc $module

    $text = & tasklist /m $module

    foreach ($line in $text) {
        $words = -split $line

        if ($words.Count -ge 3) {
            if ($words[0] -eq 'svchost.exe') {
                Write-Host $line

                $procId = $words[1]
                
                Write-Host "Dumping process: " $procId

                $procInfo = Get-Process -id $procId

                $dumpFileName = $desc + "_" + $procId + ".dmp"
                $dumpFullPath = (Join-Path $outputPath $dumpFileName)

                [DumpHelper]::CreateMiniDump($dumpFullPath, $procInfo.Handle, $procInfo.Id);
            }
        }
    }
}

if ($outputPath -ne "" -and $fileList -ne ""){

    Start-Transcript -Path $outputPath\Output.log -Append -IncludeInvocationHeader

    $fileNames = $fileList -split " "

    foreach ($fileName in $fileNames) {
        # Trim any extra whitespace
        $fileName = $fileName.Trim()
        
        # Check if the file name is not empty
        if ([string]::IsNullOrWhiteSpace($fileName)) {
            continue
        }

        # Split the file name by periods
        $parts = $fileName -split "\."

        if ($parts.Length -ge 2) {
            ParseAndDump $parts[$parts.Length - 2]  $fileName
        } else {
            ParseAndDump $parts[0] $fileName
        }
    }
  
}
