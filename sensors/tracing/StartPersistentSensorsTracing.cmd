@ECHO OFF

SETLOCAL ENABLEDELAYEDEXPANSION

CALL WHOAMI.EXE /GROUPS | FIND.EXE /I "S-1-16-12288" >nul
IF ERRORLEVEL 1 (
    ECHO This script must be run from an elevated prompt.
    ECHO Script will exit now....
    EXIT /B 1
)
echo First stopping any duplicate traces that might already be running...
logman stop -n SensorsTrace -ets >nul
logman delete -n autosession\SensorsTrace >nul
echo Setting up traces...
cd %SystemRoot%\Tracing\
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\WUDF\Services\SensorsCx0102\Parameters" /f /v VerboseOn /t REG_DWORD /d 1
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\WUDF\Services\SensorsHIDClassDriver\Parameters" /f /v VerboseOn /t REG_DWORD /d 1
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\WUDF\Services\SdoV2\Parameters" /f /v VerboseOn /t REG_DWORD /d 1
logman create trace -n autosession\SensorsTrace -ow -o "%SystemRoot%\Tracing\SensorsTraces.etl" -ets >nul
rem SensorsHid
logman update trace -n autosession\SensorsTrace -p "{32e38bd8-ac95-4605-894e-a5d815ca0f3b}" 0xffffffffffffffff 0xff -ets >nul
rem Hidi2c
logman update trace -n autosession\SensorsTrace -p "{E742C27D-29B1-4E4B-94EE-074D3AD72836}" 0xffffffffffffffff 0xff -ets >nul
rem SensorService and class extension
logman update trace -n autosession\SensorsTrace -p "{c88b592b-6090-480f-a839-ca2434de5844}" 0xffffffffffffffff 0xff -ets >nul
rem Desktop SensrSvc
logman update trace -n autosession\SensorsTrace -p "{49DA058C-1384-4e57-8915-070F1B8C8AD2}" 0xffffffffffffffff 0xff -ets >nul
rem Desktop RotMgr
logman update trace -n autosession\SensorsTrace -p "{1ADF4F66-3AE0-43B1-BEC0-C7D891A77AC5}" 0xffffffffffffffff 0xff -ets >nul
rem Windows V1 Sensor Class Extension
logman update trace -n autosession\SensorsTrace -p "{d4575dd5-963b-43c6-8c68-8b070b8692fc}" 0xffffffffffffffff 0xff -ets >nul
rem Windows V1 Sensors API
logman update trace -n autosession\SensorsTrace -p "{b12f2c9f-d3a1-447b-92f8-dca5f6c31429}" 0xffffffffffffffff 0xff -ets >nul
rem Windows V2 Sensors API
logman update trace -n autosession\SensorsTrace -p "{096772ba-b6d9-4c54-b776-3d070efb40ec}" 0xffffffffffffffff 0xff -ets >nul
logman start -n SensorsTrace -ets
echo Tracing has been setup.  
echo ===========================================================
echo Restart your machine to start tracing. Repro your scenario. Once complete, run StopSensorsTracing.cmd to stop tracing. 
echo ===========================================================
pause