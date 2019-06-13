@ECHO OFF

SETLOCAL ENABLEDELAYEDEXPANSION

CALL WHOAMI.EXE /GROUPS | FINDSTR.EXE /I "S-1-16-12288" >nul
IF ERRORLEVEL 1 (
    ECHO This script must be run from an elevated prompt.
    ECHO Script will exit now....
    EXIT /B 1
)

echo First stopping any duplicate traces that might already be running...
logman stop SensorsTrace -ets >nul
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\WUDF\Services\SensorsCx0102\Parameters" /f /v VerboseOn /t REG_DWORD /d 1
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\WUDF\Services\SensorsHIDClassDriver\Parameters" /f /v VerboseOn /t REG_DWORD /d 1
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\WUDF\Services\SdoV2\Parameters" /f /v VerboseOn /t REG_DWORD /d 1
logman start SensorsTrace -ets -o %SystemRoot%\Tracing\SensorsTrace.etl -nb 128 640 -bs 128 -f bincirc -max 250 >nul
rem SensorsHid
logman update trace SensorsTrace -p "{32e38bd8-ac95-4605-894e-a5d815ca0f3b}" 0xffffffffffffffff 0xff -ets >nul
rem Hidi2c
logman update trace SensorsTrace -p "{E742C27D-29B1-4E4B-94EE-074D3AD72836}" 0xffffffffffffffff 0xff -ets >nul
rem SensorService and class extension
logman update trace SensorsTrace -p "{c88b592b-6090-480f-a839-ca2434de5844}" 0xffffffffffffffff 0xff -ets >nul
rem Desktop SensrSvc
logman update trace SensorsTrace -p "{49DA058C-1384-4e57-8915-070F1B8C8AD2}" 0xffffffffffffffff 0xff -ets >nul
rem Desktop RotMgr
logman update trace SensorsTrace -p "{1ADF4F66-3AE0-43B1-BEC0-C7D891A77AC5}" 0xffffffffffffffff 0xff -ets >nul
rem Windows V1 Sensor Class Extension
logman update trace SensorsTrace -p "{d4575dd5-963b-43c6-8c68-8b070b8692fc}" 0xffffffffffffffff 0xff -ets >nul
rem Windows V1 Sensors API
logman update trace SensorsTrace -p "{b12f2c9f-d3a1-447b-92f8-dca5f6c31429}" 0xffffffffffffffff 0xff -ets >nul
rem Windows V2 Sensors API
logman update trace SensorsTrace -p "{096772ba-b6d9-4c54-b776-3d070efb40ec}" 0xffffffffffffffff 0xff -ets >nul
rem DES
logman update trace SensorsTrace -p "{5875532E-B0C2-4954-A77E-5319D71F97B8}" 0xffffffffffffffff 0xff -ets >nul
rem DEM
logman update trace SensorsTrace -p "{DD902827-DD80-4244-ABFD-0289ADB05F31}" 0xffffffffffffffff 0xff -ets >nul
rem DEO
logman update trace SensorsTrace -p "{7AE1974C-262A-43C6-B1FA-58CB4E36084B}" 0xffffffffffffffff 0xff -ets >nul
rem UMDF tracing (available in %ProgramData%\Microsoft\WDF\WudfTrace.etl)
reg add "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /f /v LogEnable /t REG_DWORD /d 1
reg add "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /f /v LogFlushPeriodSeconds /t REG_DWORD /d 1
echo Tracing has been started.  
echo ===========================
echo Repro your scenario now. Once complete, press any key to stop tracing.
echo ===========================
pause
echo Stopping trace...
logman stop SensorsTrace -ets >nul
echo Now collecting version info
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v BuildLabEX >> %SystemRoot%\Tracing\BuildNumber.txt
powershell "(dir %SYSTEMROOT%\system32\drivers\UMDF\SensorsHid.dll).VersionInfo | fl" >> %SystemRoot%\Tracing\BuildNumber.txt
dir /s %SystemRoot%\LiveKernelReports\* >> %SystemRoot%\Tracing\BuildNumber.txt

echo Now collecting DispDiag
rem Collecting DispDiag
dispdiag

rem copying the WUDF traces
copy %ProgramData%\Microsoft\WDF\WudfTrace.etl %SystemRoot%\Tracing >nul 2>&1
copy %ProgramData%\Microsoft\WDF\*.dmp %SystemRoot%\Tracing >nul 2>&1
copy %SYSTEMROOT%\system32\DispDiag*.dat %SystemRoot%\Tracing >nul 2>&1

start %SystemRoot%\Tracing
