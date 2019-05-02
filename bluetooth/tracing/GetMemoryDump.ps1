$ServiceName = 'BthServ'

Trap [System.Management.Automation.ParameterBindingException] {
  Write-Host "Could not find $ServiceName, it is likely not running." -ForegroundColor Red
  Break}
$Process = Get-Process -Id (Get-WmiObject -Class Win32_Service -Filter "Name LIKE '$ServiceName'" | Select-Object -ExpandProperty ProcessId)

$DumpFilePath = Join-Path $pwd "$($ServiceName)_$($Process.Id).dmp"
$DumpFile = New-Object IO.FileStream($DumpFilePath, [IO.FileMode]::Create)

$WER = [PSObject].Assembly.GetType('System.Management.Automation.WindowsErrorReporting')
$NativeMethods = $WER.GetNestedType('NativeMethods', 'NonPublic')
$MiniDump = $NativeMethods.GetMethod('MiniDumpWriteDump', ([Reflection.BindingFlags]'NonPublic, Static'))

$Result = $MiniDump.Invoke($null, @($Process.Handle, $Process.Id, $DumpFile.SafeFileHandle, [UInt32]0x2, [IntPtr]::Zero, [IntPtr]::Zero, [IntPtr]::Zero))

$DumpFile.Close()

if(-not $Result) {
    Write-Host "Failed to write dump file for service $ServiceName with PID $Process.Id."
    Trap {Continue} Remove-Item $DumpFilePath
    return
} else {
    Write-Host "Dump successfully collected in $DumpFilePath."
}
