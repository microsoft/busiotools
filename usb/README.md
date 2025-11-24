# USB, HID and SPB Diagnostics

## Review blog posts and documentation
Before filing a bug report, please review existing [blog posts](https://techcommunity.microsoft.com/t5/Microsoft-USB-Blog/bg-p/MicrosoftUSBBlog) and [documentation](https://docs.microsoft.com/en-us/windows-hardware/drivers/usbcon). They cover a variety of common problems including troubleshooting HLK failures.

If these topics do not cover what you're looking for, please follow these steps to collect files for your bug report.

### 1. Collect Traces
- Install/deploy tracing files; either (choose (i) OR (ii), not both):-
  1. Install tracing files from [winget](https://learn.microsoft.com/en-us/windows/package-manager/winget/)
     - Run `winget install windowsbusestracing` from an elevated command prompt
     - Run `startwindowsbusestracing` from an elevated command prompt
  2. OR Use tracing files directly
     - Download the following three files:
     [BusesAllProfile.wprp](https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/BusesAllProfile.wprp), [BusesTrace.cmd](https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/BusesTrace.cmd), and [UtilityCollectMiniDumps.ps1](https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/UtilityCollectMiniDumps.ps1)
     - Run **BusesTrace.cmd** from an elevated command prompt.
- At the first menu, select "**Start Tracing**".
- At the second menu select the component in which tracing is desired: for example select "**Input/HID components only**" if you are reporting an input related issue. If unsure, you can also select "**All buses components**".
- At the next menu, choose to either start tracing now, or start tracing on next boot session.
- Follow the on-screen instructions to reproduce the issue and conclude the files.
- If applicable, note the approximate time the problem happened in the bug report. For example "the problem happened around 13:50:25 (hh:mm:ss)", or "the problem started about 5 seconds before tracing was stopped."

### 2. Memory Dump
If you are reporting a system crash or hang, please include a memory dump along with the output from kernel debug command "!analyze -v" in the bug report.
USB and other kernel debugger extensions are [documented here](https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/usb-3-extensions)

Display/sensor minidumps are automatically collected if you select "sensor components only" in the second menu **BusesTrace.cmd**. A full dump might still be required.

[Back to root](http://aka.ms/bustools) 

