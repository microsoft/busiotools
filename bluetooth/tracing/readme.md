# How to collect Bluetooth logs

## Collecting logs
From an adminstrative PowerShell session:
    
    wget https://github.com/Microsoft/busiotools/raw/master/bluetooth/tracing/BluetoothStack.wprp -outfile .\BluetoothStack.wprp
    wpr.exe -start BluetoothStack.wprp!BluetoothStack -filemode

Ensure the device reconnects either:
    * Toggle the Bluetooth radio off-on via the quick action menu.
    * Force a power cycle of the remote device.
*Reproduce the issue.*
From an adminstrative PowerShell session:
   
    wpr.exe -stop BthTracing.etl

*Note: This will not continue tracing after a reboot*

## Collecting logs across reboots
From an adminstrative PowerShell session:
    
    wget https://github.com/Microsoft/busiotools/raw/master/bluetooth/tracing/BluetoothStack.wprp -outfile .\BluetoothStack.wprp
    wpr.exe -boottrace BluetoothStack.wprp!BluetoothStack -filemode
    shutdown -r -f -t 0
  
*Reproduce the issue*
- trace is not running until you reboot the machine
- log will keep running across reboots until you stop it manually

From an adminstrative PowerShell session:
   
    wpr.exe -stop BthTracing.etl

## Verbose/Non-verbose logs
The above collects Verbose logs by default. If you don't need verbose logs, replace "BluetoothStack.wprp!BluetoothStack" with "BluetoothStack.wprp!BluetoothStack.Light" in the above commands.

## Collecting logs for driver or setup issues
* Attach c:\windows\inf\setupapi.*.log and c:\Windows\Panther\setupact.log and c:\windows\logs\windowsupdate\* to the bug.
* Attach Microsoft-Windows-Kernel-PnP%4Configuration.evtx (In event viewer as Microsoft-Windows-Kernel-PnP\Configuration).

## More info on WPR
wpr.exe is available on all version of windows. More details can be found [here.](https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-8.1-and-8/hh448229%28v%3dwin.10%29)

wpr.exe /? will also give you more information.

# How to collect radio info

From PowerShell execute: 
  
    wget https://github.com/Microsoft/busiotools/raw/master/bluetooth/tracing/GetBluetoothRadioInfo.ps1 | iex
