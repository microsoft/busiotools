
@ECHO OFF

SETLOCAL ENABLEDELAYEDEXPANSION

IF EXIST c:\windows\system32\logman.exe (
echo Running logging for desktop systems...

CALL WHOAMI.EXE /GROUPS | FIND.EXE /I "S-1-16-12288" >nul
IF ERRORLEVEL 1 (
    ECHO This script must be run from an elevated prompt.
    ECHO Script will exit now....
    EXIT /B 1
)

echo Stopping trace...
logman stop -n SensorsTrace -ets >nul
logman delete -n autosession\SensorsTrace >nul
echo Now collecting version info
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v BuildLabEX >> %SystemRoot%\Tracing\BuildNumber.txt
powershell "(dir %SYSTEMROOT%\system32\drivers\UMDF\SensorsHid.dll).VersionInfo | fl" >> %SystemRoot%\Tracing\BuildNumber.txt
dir /s %SystemRoot%\LiveKernelReports\* >> %SystemRoot%\Tracing\BuildNumber.txt

rem copying the WUDF traces
copy %ProgramData%\Microsoft\WDF\WudfTrace.etl %SystemRoot%\Tracing >nul 2>&1
copy %ProgramData%\Microsoft\WDF\*.dmp %SystemRoot%\Tracing >nul 2>&1

start %SystemRoot%\Tracing

) ELSE (

echo Running logging for other systems...

echo Stopping trace...
tracelog -stop SensorsTrace >nul
tracelog -remove SensorsTrace >nul
echo Now collecting version info
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v BuildLabEX >> %SystemRoot%\Tracing\BuildNumber.txt
dir /s %SystemRoot%\LiveKernelReports\* >> %SystemRoot%\Tracing\BuildNumber.txt

rem copying the WUDF traces
copy %ProgramData%\Microsoft\WDF\WudfTrace.etl %SystemRoot%\Tracing >nul 2>&1
copy %ProgramData%\Microsoft\WDF\*.dmp %SystemRoot%\Tracing >nul 2>&1

)

echo Now collecting DispDiag
rem Collecting DispDiag
dispdiag

copy %SYSTEMROOT%\system32\DispDiag*.dat %SystemRoot%\Tracing >nul 2>&1
