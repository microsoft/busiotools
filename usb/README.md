# USB, HID and SPB Diagnostics

## Review blog posts and documentation
Before filing a bug report, please review existing [blog posts](https://techcommunity.microsoft.com/t5/Microsoft-USB-Blog/bg-p/MicrosoftUSBBlog) and [documentation](https://docs.microsoft.com/en-us/windows-hardware/drivers/usbcon). They cover a variety of common problems including troubleshooting HLK failures.

If these topics do not cover what you're looking for, please follow these steps to collect files for your bug report.

### 1. BusesTrace.cmd
- Download both [BusesAllProfile.wprp](https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/BusesAllProfile.wprp) and [BusesTrace.cmd](https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/BusesTrace.cmd).
- Run **BusesTrace.cmd** from an elevated command prompt.
- At the first menu, select "**Start Tracing**".
- At the second menu, select "**Input/HID components only**" if you are reporting an input related issue. Otherwise, select "**All buses components**".
- At the next menu, choose to either start tracing now, or start tracing on next boot session.
- Follow the on-screen instructions to reproduce the issue and conclude the files.
- If applicable, note the approximate time the problem happened in the bug report. For example "the problem happened around 13:50:25 (hh:mm:ss)", or "the problem started about 5 seconds before tracing was stopped."

### 2. Memory Dump
If you are reporting a system crash or hang, please include a memory dump along with the output from kernel debug command "!analyze -v" in the bug report.
USB and other kernel debugger extensions are [documented here](https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/usb-3-extensions)

[Back to root](http://aka.ms/bustools) 

