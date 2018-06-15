# How to collect bluetooth logs

## More info
wpr.exe is available on all version of windows. More details can be found [here.](https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-8.1-and-8/hh448229%28v%3dwin.10%29)

wpr.exe /? will also give you more information.

## Collecting logs

From an administrator command prompt: 
1. Run as admin: "wpr.exe -start BluetoothStack.wprp -filemode"
2. Toggle the Bluetooth radio off-on via the quick action menu or force a power cycle of the remote device (we want the connection information).
3. Reproduce the issue.
4. Run as admin: "wpr.exe -stop BthTracing.etl"

*Note: This will not continue tracing after a reboot*

## Collecting logs across reboots
From an administrator command prompt: 
1. Run as admin: "wpr.exe -boottrace -addboot BluetoothStack.wprp -filemode"
2. Reboot machine 
  
  *Note: Trace is not running until you reboot the machine*

3. Reproduce the issue - log will keep running across reboots until you stop it manually
4. Run as admin: "wpr.exe -boottrace -stopboot BthTracing.etl"

## Verbose/Non-verbose logs
1. The above collects Verbose logs by default.
2. If you don't need verbose logs, replace start tracing commands above with: "wpr.exe -start BluetoothStack.wprp.Light"

## Collecting logs for driver or setup issues
* Attach c:\windows\inf\setupapi.*.log and c:\Windows\Panther\setupact.log and c:\windows\logs\windowsupdate\* to the bug.
* Attach Microsoft-Windows-Kernel-PnP%4Configuration.evtx (In event viewer as Microsoft-Windows-Kernel-PnP\Configuration).