# Bus I/O Tools for Windows

A comprehensive collection of diagnostic tools and scripts for troubleshooting Windows peripheral devices and buses. These tools help developers debug and diagnose issues with USB, Bluetooth, sensors, cameras, and other hardware components.

For detailed technical information and best practices, visit our [Microsoft USB Blog](https://techcommunity.microsoft.com/t5/Microsoft-USB-Blog/bg-p/MicrosoftUSBBlog).

## 📦 Quick Start

Download the complete toolkit:
```powershell
# Download as ZIP
https://github.com/Microsoft/busiotools/archive/master.zip

# Or clone with git
git clone https://github.com/Microsoft/busiotools.git
```

## 🔧 Tools Overview

### USB & Simple Peripheral Buses
**[USB, HID, I2C, UART, and SPI Tools](usb/README.md)**
- Comprehensive tracing for USB and HID devices
- Support for I2C, UART, and SPI peripheral buses
- Built-in diagnostics and troubleshooting utilities
- Install via winget: `winget install windowsbusestracing`

### Bluetooth
**[Bluetooth Tracing & Diagnostics](bluetooth/tracing/readme.md)**
- Capture Bluetooth stack logs using Windows Performance Recorder (WPR)
- Support for both single-session and cross-reboot tracing
- Radio information and connection diagnostics

### Sensors
**[Sensor Tools & Testing](sensors/README.md)**
- **SensorExplorer**: Real-time sensor monitoring application ([Store App](http://aka.ms/sensorexplorer))
- **MonitorBrightnessApp**: Visualize brightness values in real-time
- **BrightnessTests**: Automated brightness testing scripts
- **MALT**: Screen brightness measurement and testing hardware
- Sensor tracing and logging utilities

### Camera
**[Camera Tracing](camera/Tracing/Readme.md)**
- PowerShell-based camera tracing scripts
- Support for desktop and device scenarios
- Performance profiling capabilities
- Boot trace support for initialization issues

### HMD Validation Kit
**[HMD (Head-Mounted Display) Testing](hmdvalidationkit/README.md)**
- Managed C# libraries for HMD hardware validation
- Automated testing for USB, HDMI, display, and audio
- COM port-based device control

### Simple Peripheral Buses
**[SPB (I2C & SPI) Tracing](spb/README.md)**
- SpbCx driver tracing utilities
- Support for both live and autologger modes
- Offline hive analysis

## 📚 Additional Resources

- [Official Documentation](https://docs.microsoft.com/en-us/windows-hardware/drivers/)
- [Microsoft USB Blog](https://techcommunity.microsoft.com/t5/Microsoft-USB-Blog/bg-p/MicrosoftUSBBlog)
- [Windows Hardware Dev Center](https://developer.microsoft.com/en-us/windows/hardware/)

## 🤝 Contributing

This project is maintained by Microsoft. For issues, questions, or contributions, please refer to [SUPPORT.md](SUPPORT.md) and [SECURITY.md](SECURITY.md).

## 📄 License

This project is licensed under the terms specified in [LICENSE](LICENSE).
