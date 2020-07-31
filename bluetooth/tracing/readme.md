# How to collect Bluetooth logs
 
**This information is targeted to developers, for non-developer information on troubleshooting Bluetooth, see [Fix Bluetooth problems in Windows 10](https://support.microsoft.com/en-us/help/14169/windows-10-fix-bluetooth-problems).**

This readme covers how developers can collect Bluetooth logs for bugs using [Windows Performance Recorder (WPR)](https://docs.microsoft.com/en-us/windows-hardware/test/wpt/introduction-to-wpr) using the custom Bluetooth tracing [Recording Profile](https://docs.microsoft.com/en-us/windows-hardware/test/wpt/wpr-quick-start#using-recording-profiles) found here: [BluetoothStack.wprp](./BluetoothStack.wprp). There are few ways you can collect these logs: 
* [Normal Mode (cancels on reboot)](#Normal-Mode-(cancels-on-reboot)): Tracing for a single boot session, will not continue tracing after a reboot
* [Autologger mode (collects logs across reboots)](#Autologger-mode-(collects-logs-across-reboots)): Tracing for running across reboots until you stop it manually
> Note: Only one of the above methods can be used at a time. 

More details can be found in the [Additional Information](#Additional-Information) section.
 
## Normal Mode (cancels on reboot)
1. From an administrative PowerShell session run:

    ```powershell
        wget https://github.com/Microsoft/busiotools/raw/master/bluetooth/tracing/BluetoothStack.wprp -outfile .\BluetoothStack.wprp
        wpr.exe -start BluetoothStack.wprp!BluetoothStack -filemode
    ```
    The above commands download and run the Bluetooth tracing Recording Profile ([BluetoothStack.wprp](./BluetoothStack.wprp))

1. To ensure we capture connections either:
    * Toggle the Bluetooth radio off-on via the quick action menu.
    * Force a power cycle of the remote device.
    
1. *Reproduce the issue.*

1. From an administrative PowerShell session:
    ```powershell
        wpr.exe -stop BthTracing.etl
    ````
## Autologger mode (collects logs across reboots)
1. From an adminstrative PowerShell session:
     ```powershell   
        wget https://github.com/Microsoft/busiotools/raw/master/bluetooth/tracing/BluetoothStack.wprp -UseBasicParsing -outfile .\BluetoothStack.wprp
        wpr.exe -boottrace -addboot BluetoothStack.wprp!BluetoothStack -filemode
        shutdown -r -f -t 0
    ```
      The above commands download and run the Bluetooth tracing Recording Profile ([BluetoothStack.wprp](./BluetoothStack.wprp)) in Autologger mode.
1. Reboot the machine:
    * trace is not running until you reboot the machine
    * log will keep running across reboots until you stop it manually
1. *Reproduce the issue*

1. From an adminstrative PowerShell session:
    ```powershell
     wpr.exe -boottrace -stopboot BthTracing.etl
    ```
    This command stops tracing, if you want to start tracing again you'll need to start it again.

    > Note: To see the status of an autologger trace run the following command:`
    Wpr -status -instancename wprapp_boottr`

## Additional Information

Some additional information on the log collection steps above.


### How to Collect Bluetooth Radio Information
From PowerShell execute: 
```powershell
wget https://github.com/Microsoft/busiotools/raw/master/bluetooth/tracing/GetBluetoothRadioInfo.ps1 -UseBasicParsing | iex
```

### Verbose/Non-verbose logs
The above collects Verbose logs by default. If you don't need verbose logs, replace ``BluetoothStack.wprp!BluetoothStack`` with ``BluetoothStack.wprp!BluetoothStack.Light`` in the above commands.

### Collecting logs for driver or setup issues
* Attach `c:\windows\inf\setupapi.*.log` and `c:\Windows\Panther\setupact.log` and `c:\windows\logs\windowsupdate\*` to the bug.
* Attach `Microsoft-Windows-Kernel-PnP%4Configuration.evtx` (In event viewer as `Microsoft-Windows-Kernel-PnP\Configuration`).

### Collecting logs for performance issues
1. From an administrative PowerShell session:
    ```powershell
        wget https://github.com/Microsoft/busiotools/raw/master/bluetooth/tracing/BluetoothStack.wprp -outfile .\BluetoothStack.wprp
        wpr.exe -start BluetoothStack.wprp!BluetoothStack.Light -start CPU -filemode
    ```
    The above commands download and run the Bluetooth tracing Recording Profile ([BluetoothStack.wprp](./BluetoothStack.wprp)) to collect performance logs.
    >*Note: This will not continue tracing after a reboot*
1. To ensure the device reconnects either:
    * Toggle the Bluetooth radio off-on via the quick action menu.
    * Force a power cycle of the remote device.
    
1. *Reproduce the issue.*

1. From an administrative PowerShell session:
    ```powershell
        wpr.exe -stop BthTracing_CPU.etl
    ````

### More info on Windows Performance Recorder (WPR)
`wpr.exe` is available on all version of windows. More details can be found [here.](https://docs.microsoft.com/en-us/windows-hardware/test/wpt/wpr-command-line-options)

`wpr.exe /?` will also give you more information.
