# USB, HID and SPB Diagnostics

## Review blog posts and documentation
Before filing a bug report please review existing blog posts and documentation. They cover a variety of common problems including troubleshooting HLK failures.
[USB Blog](https://techcommunity.microsoft.com/t5/Microsoft-USB-Blog/bg-p/MicrosoftUSBBlog) and 
[Docs](https://docs.microsoft.com/en-us/windows-hardware/drivers/usbcon)
If these topics do not cover what you're looking for **please include all of the following** in your bug report.

### 1. Traces
- Run [usbtrace.cmd](https://github.com/microsoft/busiotools/blob/master/usb/tracing/usbtrace.cmd) from an elevated command prompt
- At the first menu list, select "Start Tracing"
- If you are reporting an input related issue enable HID tracing at the next prompt
- At the final menu list, choose to either start tracing now, or on the next boot
- Follow the on-screen instructions to reproduce the issue and conclude the trace
- If applicable, note the approximate time the problem reproduced and include that in the bug report.

### 2. Capture PnP state
- Run _pnputil /export-pnpstate ExportedPnPState.PnP_ from an elevated command prompt
- Include _ExportedPnPState.PnP_ in your bug report

### 3. Memory Dump
If you are reporting a system crash or hang, please include a memory dump along with output from "!analyze -v" in the bug report.
USB and other kernel debugger extensions are [documented here](https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/usb-3-extensions)

[Back to root](http://aka.ms/bustools) 

