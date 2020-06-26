$WerNativeMethods = [PSObject].Assembly.GetType('System.Management.Automation.WindowsErrorReporting').GetNestedType('NativeMethods', 'NonPublic')
$MiniDumpWriteDump = $WerNativeMethods.GetMethod('MiniDumpWriteDump', ([Reflection.BindingFlags]'NonPublic, Static'))

$ServiceNames = @('BthServ', 'Bluetooth%', 'BTAGService', 'BthAvctpSvc')

If (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(`

    [Security.Principal.WindowsBuiltInRole] “Administrator”)) {
    Write-Error "Administrator rights are required to collect dumps."
    Break
}

$DumpFolder = Join-Path ($Env:Temp) (Get-Random -Minimum 1000)
New-Item -Path $DumpFolder -Type Directory | Out-Null

foreach ( $ServiceName in $ServiceNames ) {
    Trap [System.Management.Automation.ParameterBindingException] {
        Write-Warning "Could not find $ServiceName, it is likely not running."
        Continue }
    $Process = $null #ensure null on failure below
    $Process = Get-Process -Id (Get-WmiObject -Class Win32_Service -Filter "Name LIKE '$ServiceName'" | Select-Object -ExpandProperty ProcessId)
    
    if ($Process) {
        $DumpFilePath = Join-Path $DumpFolder "$($ServiceName)_$($Process.Id).dmp"
        $DumpFile = New-Object IO.FileStream($DumpFilePath, [IO.FileMode]::Create)

        Write-Host "Dumping service $ServiceName with PID $($Process.Id)..."
        $Result = $MiniDumpWriteDump.Invoke($null, @($Process.Handle, $Process.Id, $DumpFile.SafeFileHandle, [UInt32]0x2, [IntPtr]::Zero, [IntPtr]::Zero, [IntPtr]::Zero))

        $DumpFile.Close()

        if (-not $Result) {
            Write-Error "Failed to write dump file for service $ServiceName with PID $($Process.Id)."
            Break }
    }
}

if ((gci $DumpFolder).Count -gt 1) {
  Compress-Archive $DumpFolder "$DumpFolder.zip" -CompressionLevel Optimal -Force
  Write-Host "Dumps stored in $DumpFolder.zip."
}

Trap {Continue} Remove-Item $DumpFolder -Force -Recurse
