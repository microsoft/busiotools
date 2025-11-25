# USB, HID and SPB Diagnostics

## Overview

This guide provides step-by-step instructions for collecting diagnostic information when troubleshooting USB, HID (Human Interface Devices), and SPB (Simple Peripheral Bus: I2C, UART, SPI) issues on Windows.

## Before You Start

**Review existing resources first** - Many common issues are already documented:
- [Microsoft USB Blog](https://techcommunity.microsoft.com/t5/Microsoft-USB-Blog/bg-p/MicrosoftUSBBlog) - Technical articles and troubleshooting guides
- [USB Driver Documentation](https://docs.microsoft.com/en-us/windows-hardware/drivers/usbcon) - Official Windows USB driver documentation
- [HLK Troubleshooting](https://docs.microsoft.com/en-us/windows-hardware/test/hlk/) - Hardware Lab Kit common issues

If these resources don't address your issue, follow the steps below to collect diagnostic data for a bug report.

---

## Step 1: Collect Traces

Traces capture detailed system activity and are essential for diagnosing bus-related issues.

### Installation Method (Recommended)

**Using winget** - Easiest method for most users:

1. Open an **elevated** Command Prompt (Run as Administrator)
2. Install the tracing package:
   ```cmd
   winget install windowsbusestracing
   ```
3. Start tracing:
   ```cmd
   startwindowsbusestracing
   ```

### Manual Method

**Direct file download** - Use when winget is unavailable:

1. Download these three files:
   - [BusesAllProfile.wprp](https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/BusesAllProfile.wprp)
   - [BusesTrace.cmd](https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/BusesTrace.cmd)
   - [UtilityCollectMiniDumps.ps1](https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/UtilityCollectMiniDumps.ps1)

2. Open an **elevated** Command Prompt in the download directory
3. Run the trace script:
   ```cmd
   BusesTrace.cmd
   ```

### Configuring the Trace

Follow the on-screen menus:

1. **First menu**: Select **"Start Tracing"**

2. **Second menu**: Choose the component to trace:
   - **"Input/HID components only"** - For keyboard, mouse, touch, or HID device issues
   - **"USB components only"** - For USB storage, connectivity, or enumeration issues
   - **"Sensor components only"** - For sensor-related problems (auto-collects minidumps)
   - **"All buses components"** - When unsure or for multi-component issues

3. **Third menu**: Choose when to start tracing:
   - **"Start tracing now"** - For issues that occur during normal operation
   - **"Start tracing on next boot"** - For issues during system startup or driver initialization

### Reproducing the Issue

1. Once tracing starts, **reproduce the problem** that you're experiencing
2. Follow the on-screen instructions to stop tracing and save the files
3. **Note the timestamp** when the issue occurred (e.g., "Issue occurred at 13:50:25" or "Problem started 5 seconds before I stopped tracing")

### Output Files

The trace files will be saved to a folder that automatically opens when tracing completes. Include all files from this folder in your bug report.

---

## Step 2: Memory Dump (For Crashes/Hangs)

If you're reporting a **system crash (BSOD)** or **system hang**, additional data is required:

### Required Files

1. **Memory dump file** - Typically located at:
   - `C:\Windows\MEMORY.DMP` (complete dump)
   - `C:\Windows\Minidump\` (minidump files)

2. **Debugger analysis output**:
   - Open the dump file in WinDbg or kernel debugger
   - Run the command: `!analyze -v`
   - Save the complete output to a text file

### Additional Resources

- [USB Kernel Debugger Extensions](https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/usb-3-extensions) - USB-specific debugging commands
- [Windows Debugging Tools](https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/) - Download and usage guide

### Automatic Minidump Collection

**Note**: Display and sensor minidumps are automatically collected when you select **"sensor components only"** in the BusesTrace.cmd menu. However, a full memory dump may still be needed for complete analysis.

---

## Need Help?

- **Documentation**: Refer to official Windows Hardware documentation
- **Community**: Post questions on the [Microsoft USB Tech Community](https://techcommunity.microsoft.com/t5/Microsoft-USB-Blog/bg-p/MicrosoftUSBBlog)
- **Issues**: See [SUPPORT.md](../SUPPORT.md) for information on reporting issues

[← Back to main README](../README.md) 

