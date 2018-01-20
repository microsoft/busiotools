# Sensors Diagnostics

1) Open an elevated command prompt
2) Run StartPersistentSensorsTracing.cmd
3) Reboot the system when instructed
4) After reboot, reproduce the problem
5) Run StopPersistentSensorsTracing.cmd from an elevated command prompt
6) Collect the traces and build information from C:\Windows\tracing

Click here to download the BUS IO Tools package as a zip file: https://github.com/Microsoft/busiotools/archive/master.zip

# Brightness
The following are only supported on systems which support changing display brightness.

## Reactivity testing
1. Open an elevated PowerShell window
2. Run ReactivityScript.ps1
3. The script will toggle your screen between bright and dim. Observe and make sure that the reaction time is quick.

### Known limitations
1. The reactivity testing script assumes that the device supports 101 levels of backlight. The script may fail if you do not. It is recommended that you support all 101 backlight levels (0 to 100 inclusive) for the best brightness experience.
