# Windows Camera Tracing

### Prerequisites
- Download the Trace package as a zip file: https://github.com/Microsoft/busiotools/archive/master.zip and unpack it.
  - Camera trace scripts are located in camera/Tracing subfolder

### How to collect traces
#### Desktop:
- Launch elevated powershell, run `Trace.ps1`.
  - If encountering problems to execute the powershell script, first run `Set-ExecutionPolicy -ExecutionPolicy Bypass -Force` from the elevated powershell.
- Run your scenario when told.
- Press Enter and wait for the script to complete.
- The folder containing the traces will pop up.

#### Desktop, reboot scenario
- Launch elevated powershell, run `Trace.ps1 -StartBootTrace`.
  - If encountering problems to execute the powershell script, first run `Set-ExecutionPolicy -ExecutionPolicy Bypass -Force` from the elevated powershell.
- The script will print the trace stop command after your reboot scenario is completed `Trace.ps1 -stopbootTrace <generated trace folder path in Temp>` **NOTE** you may want to save this to temp text file for easier usage after reboot.
- Restart the Windows and run your scenario that required reboot
- Launch elevated powershell and stop the trace with the previously provided command.

#### Tshell connected device:
- Launch tshell and establish connection to device
- run `Trace.ps1`
- Run your scenario when told on a different tshell console
- Press enter and wait for script to complete
- The folder containing the traces will pop up.

#### Email traces:
Zip files (Right click > Send to > Compressed folder) and send.

View the resulting WPP traces with TraceView, available in the Windows SDK.
Click here to download the Trace package as a zip file: https://github.com/Microsoft/busiotools/archive/master.zip
