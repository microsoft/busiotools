# Prerequisites
Click here to download the sensors tracing package (A.K.A. Bus IO tool) as a zip file: https://github.com/Microsoft/busiotools/archive/master.zip

Unzip the package locally and browse to the sensors\tracing subfolder

# Sensors Diagnostics

1) Open an elevated command prompt
2) Run StartPersistentSensorsTracing.cmd
2a) If you would like to capture the device driver details Run StartPersistentSensorsTracing.cmd -pnp
3) Reboot the system when instructed
4) After reboot, reproduce the problem
5) Run StopPersistentSensorsTracing.cmd from an elevated command prompt
6) Collect the traces and build information from C:\Windows\tracing
7) Place the file SensorsTraces.etl at the top level of the bug attachments (outside of any folder or compressed .zip folder)

The steps above are the recommended steps. Alternatively, if you think rebooting the system will lose the repro, do the following instead

1) Open an elevated command prompt
2) Run CollectSensorsTraces.cmd
2a) If you would like to capture the device driver details Run CollectSensorsTraces.cmd -pnp
3) When prompted, reproduce the problem
4) Press any key on the command prompt where you ran CollectSensorsTraces.cmd
5) Collect the traces and build information from C:\Windows\tracing
6) Place the file SensorsTraces.etl at the top level of the bug attachments (outside of any folder or compressed .zip folder)

# Brightness
The following are only supported on systems which support changing display brightness.

## Reactivity testing
1. Open an elevated PowerShell window
2. Run ReactivityScript.ps1
3. The script will toggle your screen between bright and dim. Observe and make sure that the reaction time is quick.

### Known limitations
1. The reactivity testing script assumes that the device supports 101 levels of backlight. The script may fail if you do not. It is recommended that you support all 101 backlight levels (0 to 100 inclusive) for the best brightness experience.
2. If SensorsTraces.etl isn't placed in the attachments outside of a folder or compressed .zip folder our parser will not be able to pull out insights.
