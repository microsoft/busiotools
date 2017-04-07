Sensors Diagnostics

For tracing in the current boot session:
1) Open an elevated command prompt
2) Run CollectSensorsTraces.cmd
3) Reproduce the problem
4) Hit any key once complete
5) Collect the traces and build information from C:\Windows\tracing

For enabling persistent tracing in the next boot session:
1) Open an elevated command prompt
2) Run StartPersistentSensorsTracing.cmd
3) Reboot the system when instructed
4) After reboot, reproduce the problem
5) Run StopPersistentSensorsTracing.cmd from an elevated command prompt
6) Collect the traces and build information from C:\Windows\tracing

Click here to download the BUS IO Tools package as a zip file: https://github.com/Microsoft/busiotools/archive/master.zip
