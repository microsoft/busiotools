@echo off
setlocal
set scriptDirectory=%~dp0
set wprpFileName=BusesAllProfile.wprp
set traceFilesOutputPath=%SystemRoot%\Tracing
set etlFileName=Buses-MachineInfo.etl
set pnpStatePreReproFileName=Buses-PnpStatePreRepro.pnp
set pnpStatePostReproFileName=Buses-PnpStatePostRepro.pnp
set systemEventLogsFileName=Buses-System.evtx
set applicationEventLogsFileName=Buses-Application.evtx
set pnpLogsFileName=Buses-DriverWatchdog.evtx
set kseLogsFileName=Buses-KernelShimEngine.evtx
set ucmUcsiCxLogsFileName=Buses-UcmUcsiCx.evtx
set sleepStudyReportFileName=Buses-SleepStudyReport.html
set busesTraceInfoFileName=Buses-TraceInfo.txt
set miniDumpCollectionScript=UtilityCollectMiniDumps.ps1
set Buses_Backup_LogMinidumpType=0x1120
set Buses_Backup_LogEnable=0
set Buses_Backup_LogFlushPeriodSeconds=300
set collectPnpStates=1

if not exist %wprpFileName% (
    echo.
    echo #########################################################################################################
    echo.
    echo ERROR: %wprpFileName% is NOT found in the current directory.
    echo.
    echo.
    echo.Please open the following link in a browser and then save it as %wprpFileName% to the current directory.
    echo.
    echo   https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/BusesAllProfile.wprp
    echo.
    echo Alternatively, use this command in PowerShell to download the file.
    echo.
    echo   wget https://raw.githubusercontent.com/microsoft/busiotools/master/usb/tracing/BusesAllProfile.wprp -outfile .\BusesAllProfile.wprp
    echo.
    echo For more information, please refer to http://aka.ms/usbtrace.
    echo.
    echo #########################################################################################################
    goto End
)

if exist %SystemRoot%\system32\WHOAMI.EXE (
    %SystemRoot%\system32\WHOAMI.EXE /GROUPS | FIND.EXE /I "S-1-16-12288" >nul
    IF ERRORLEVEL 1 (
        echo.
        echo #########################################################################################################
        echo.
        ECHO ERROR: This script must be run from an elevated command prompt.
        echo.
        echo #########################################################################################################
        goto End
    )
)

cls
if /I "%~1"=="-NoPnpState" set collectPnpStates=0

:MainMenu
set selection=
echo ###########################
echo        BUSES TRACING
echo ###########################
echo.
echo 1) Start Tracing
echo 2) Stop Boot Session Trace
echo 3) Cleanup Previous Session
echo 4) Exit
echo.
set /p selection=Enter selection number:
if "%selection%"=="1" goto BasicProfilesMenu
if "%selection%"=="2" goto StopBootTrace
if "%selection%"=="3" goto Cleanup
if "%selection%"=="4" goto End
echo "%selection%" is not a valid option.  Please try again.
echo.
goto MainMenu

:BasicProfilesMenu
set selection=
set profileName=
echo.
echo ---------------------------------------
echo Which components to collect trace from?
echo ---------------------------------------
echo 1) All buses components
echo 2) USB4 components
echo 3) Input/HID components only (select for HID problems - keyboard, mouse, touch input, buttons etc.)
echo 4) Sensors components only
echo 5) Other options...
echo 6) Back
echo.
set /p selection=Enter selection number:
if "%selection%"=="1" set profileName=BusesAllProfile
if "%selection%"=="2" set profileName=Usb4WithTunnelsProfile
if "%selection%"=="3" set profileName=InputOnlyProfile
if "%selection%"=="4" set profileName=SensorsOnlyProfile
if "%selection%"=="5" goto OtherProfilesMenu
if "%selection%"=="6" goto MainMenu
if not "%profileName%"=="" goto StartOptionsMenu
echo.
echo "%selection%" is not a valid option.  Please try again.
echo.
goto BasicProfilesMenu

:OtherProfilesMenu
set selection=
set profileName=
echo.
echo ----------------------
echo Which profile to use?
echo ----------------------
echo 1) UsbOnlyProfile (USB2, USB3 and USB4)
echo 2) UsbAndPnPProfile (USB2, USB3, USB4 and PnP)
echo 3) UsbCProfile (UCM, URS and USBFN)
echo 4) LowPowerBusesProfile (SerCx2 and SpbCx)
echo 5) InputOnlyWithVerboseWppProfile (Input)
echo 6) Back
echo.
set /p selection=Enter selection number:
if "%selection%"=="1" set profileName=UsbOnlyProfile
if "%selection%"=="2" set profileName=UsbAndPnpProfile
if "%selection%"=="3" set profileName=UsbCProfile
if "%selection%"=="4" set profileName=LowPowerBUsesProfile
if "%selection%"=="5" set profileName=InputOnlyWithVerboseWppProfile
if "%selection%"=="6" goto BasicProfilesMenu
if not "%profileName%"=="" goto StartOptionsMenu
echo.
echo "%selection%" is not a valid option.  Please try again.
echo.
goto OtherProfilesMenu

:StartOptionsMenu
set startTime=%Time%
set startDate=%Date%
set selection=
echo.
echo --------------
echo Start options:
echo --------------
echo 1) Start Now
echo 2) Start From Next Boot Session
echo 3) Back
echo.
set /p selection=Enter selection number:
if "%selection%"=="1" goto CommonStartSteps
if "%selection%"=="2" goto CommonStartSteps
if "%selection%"=="3" goto BasicProfilesMenu
echo.
echo "%selection%" is not a valid option.  Please try again.
echo.
goto StartOptionsMenu

:CommonStartSteps
if "%collectPnpStates%"=="1" (
    rem Collect pre-repro PnP state (the echo text below cannot use parentheses, or it will end the if statement prematurely.
    echo Collecting pre-repro PnP state... [If it doesn't complete within a few minutes, use CTRL-C to interrupt it.]
    if exist %traceFilesOutputPath%\%pnpStatePreReproFileName% del %traceFilesOutputPath%\%pnpStatePreReproFileName%
    pnputil.exe /export-pnpstate %traceFilesOutputPath%\%pnpStatePreReproFileName%
)

rem Put message into dispdiag's ring buffer, note this does not trigger collection of the .dat file
dispdiag.exe -msg "Buses Trace: Starting trace with profile %profileName% at %startTime% %startDate%"

rem Backup and changing UMDF settings
echo Updating UMDF trace and dump settings...
FOR /F "tokens=3" %%v IN ('reg.exe query "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogMinidumpType') DO set Buses_Backup_LogMinidumpType=%%v
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /v Buses_Backup_LogMinidumpType /t REG_DWORD /d %Buses_Backup_LogMinidumpType% /f
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogMinidumpType /t REG_DWORD /d 0x1122 /f
FOR /F "tokens=3" %%v IN ('reg.exe query "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogEnable') DO set Buses_Backup_LogEnable=%%v
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /v Buses_Backup_LogEnable /t REG_DWORD /d %Buses_Backup_LogEnable% /f
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogEnable /t REG_DWORD /d 1 /f
FOR /F "tokens=3" %%v IN ('reg.exe query "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogFlushPeriodSeconds') DO set Buses_Backup_LogFlushPeriodSeconds=%%v
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /v Buses_Backup_LogFlushPeriodSeconds /t REG_DWORD /d %Buses_Backup_LogFlushPeriodSeconds% /f
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogFlushPeriodSeconds /t REG_DWORD /d 1 /f
if "%selection%"=="2" goto ConfigureBootTrace
rem else it's "Start Now"

:StartNow
rem Start tracing now
echo.
echo Starting Tracing Now... (%wprpFileName%!%profileName%)
wpr.exe -start %wprpFileName%!%profileName% -filemode -recordTempTo %traceFilesOutputPath%\
if not %ERRORLEVEL%==0 goto End
echo.
echo ----------------------------------------------------------------------
echo Tracing started. Reproduce the issue and hit any key to stop tracing.
echo ----------------------------------------------------------------------
pause
echo Saving WPR status to %busesTraceInfoFileName%...
wpr.exe -status profiles collectors -details > %traceFilesOutputPath%\%busesTraceInfoFileName%
echo Stopping tracing...
wpr.exe -stop %traceFilesOutputPath%\%etlFileName%
goto CollectMoreInfo

:ConfigureBootTrace
rem Configure boot trace
echo Configuring Boot Session Trace... (%wprpFileName%!%profileName%)
wpr.exe -addboot %wprpFileName%!%profileName% -filemode -recordTempTo %traceFilesOutputPath%\
if not %ERRORLEVEL%==0 goto End

echo.
echo ###############################################################################
echo Please reboot your PC to start tracing. After reproducing the issue, run this
echo script again and select "Stop Boot Session Trace" to stop tracing.
echo ###############################################################################
echo.
goto End

:StopBootTrace
echo Saving WPR status to %busesTraceInfoFileName%...
wpr.exe -status profiles collectors -details > %traceFilesOutputPath%\%busesTraceInfoFileName%
echo Stopping boot session tracing...
wpr.exe -stopboot %traceFilesOutputPath%\%etlFileName%
if not %ERRORLEVEL%==0 goto End
goto CollectMoreInfo

:CollectMoreInfo
echo.
rem Restore UMDF settings
echo Restoring UMDF trace and dump settings...
FOR /F "tokens=3" %%v IN ('reg.exe query "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /v Buses_Backup_LogMinidumpType') DO set Buses_Backup_LogMinidumpType=%%v
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogMinidumpType /t REG_DWORD /d %Buses_Backup_LogMinidumpType% /f
FOR /F "tokens=3" %%v IN ('reg.exe query "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /v Buses_Backup_LogEnable') DO set Buses_Backup_LogEnable=%%v
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogEnable /t REG_DWORD /d %Buses_Backup_LogEnable% /f
FOR /F "tokens=3" %%v IN ('reg.exe query "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /v Buses_Backup_LogFlushPeriodSeconds') DO set Buses_Backup_LogFlushPeriodSeconds=%%v
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogFlushPeriodSeconds /t REG_DWORD /d %Buses_Backup_LogFlushPeriodSeconds% /f
REG.EXE DELETE "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /f
echo.
echo Collecting more info...
rem OS Build Numbers
echo - OS build numbers...
echo Tracing Start Time: %startTime% %startDate% > %traceFilesOutputPath%\%busesTraceInfoFileName%
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v BuildLabEX >> %traceFilesOutputPath%\%busesTraceInfoFileName%
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v CurrentBuildNumber >> %traceFilesOutputPath%\%busesTraceInfoFileName%
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v DisplayVersion >> %traceFilesOutputPath%\%busesTraceInfoFileName%
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v UBR >> %traceFilesOutputPath%\%busesTraceInfoFileName%
powershell -command "REG QUERY \"HKLM\Software\Microsoft\Windows NT\CurrentVersion\Update\TargetingInfo\Installed\" /s | Select-string Client.OS -context 0,5" >> %traceFilesOutputPath%\%busesTraceInfoFileName%
dir /s %SystemRoot%\LiveKernelReports\* >> %traceFilesOutputPath%\%busesTraceInfoFileName%

rem Put message into dispdiag's ring buffer, note this does not trigger collection of the .dat file
dispdiag.exe -msg "Buses Trace: stopped trace with profile %profileName%"

if "%collectPnpStates%"=="1" (
    rem PnP State
    echo - Post-repro PnP state... [If it doesn't complete within a few minutes, use CTRL-C to interrupt it.]
    if exist %traceFilesOutputPath%\%pnpStatePostReproFileName% del %traceFilesOutputPath%\%pnpStatePostReproFileName%
    pnputil.exe /export-pnpstate %traceFilesOutputPath%\%pnpStatePostReproFileName%
)

rem Event Logs
echo - Event logs...
wevtutil.exe export-log "System" /ow:true "%traceFilesOutputPath%\%systemEventLogsFileName%"
wevtutil.exe export-log "Application" /ow:true "%traceFilesOutputPath%\%applicationEventLogsFileName%"
wevtutil.exe export-log "Microsoft-Windows-Kernel-PnP/Driver Watchdog" /ow:true "%traceFilesOutputPath%\%pnpLogsFileName%"
wevtutil.exe export-log "Microsoft-Windows-Kernel-ShimEngine/Operational" /ow:true "%traceFilesOutputPath%\%kseLogsFileName%"
wevtutil.exe export-log "Microsoft-Windows-USB-UCMUCSICX/Operational" /ow:true "%traceFilesOutputPath%\%ucmUcsiCxLogsFileName%"
rem Sleep Study Report
echo - Sleep study report...
powercfg.exe /sleepstudy /OUTPUT "%traceFilesOutputPath%\%sleepStudyReportFileName%"
echo - UMDF trace and dumps...
if exist %ProgramData%\Microsoft\WDF\WudfTrace.etl copy /y %ProgramData%\Microsoft\WDF\WudfTrace.etl %traceFilesOutputPath%\ >nul 2>&1
if exist %ProgramData%\Microsoft\WDF\*.*dmp copy /y %ProgramData%\Microsoft\WDF\*.*dmp %traceFilesOutputPath%\ >nul 2>&1
rem USB Live Kernel Reports
echo.
echo - Live kernel reports...
if exist %SystemRoot%\LiveKernelReports\USB* (
    echo Found these reports for USB. They may be related.
    echo ==================================================
    dir %SystemRoot%\LiveKernelReports\USB*
    echo.
)


rem Collecting DispDiag and if availiable the DES mini dump
if  "%profileName%"=="SensorsOnlyProfile" goto CollectDispDiag
if  "%profileName%"=="Usb4WithTunnelsProfile" goto CollectDispDiag
if  "%profileName%"=="BusesAllProfile" goto CollectDispDiag
goto SkipCollectDispDiag
:CollectDispDiag
    echo.
    echo Now collecting DispDiag
    dispdiag.exe
    move "%scriptDirectory%*.dat" %traceFilesOutputPath%

    if exist %miniDumpCollectionScript% (
        echo Now collecting sensor process minidumps
        pushd "%~dp0"
        powershell -ExecutionPolicy bypass -file "%miniDumpCollectionScript%" -FileList "Microsoft.Graphics.Display.DisplayEnhancementService.dll umpoext.dll sensorservice.dll SensorsCx.dll" -OutputPath "%traceFilesOutputPath%"
        copy %SYSTEMROOT%\system32\DispDiag*.dat %traceFilesOutputPath% >nul 2>&1
        popd
    )
:SkipCollectDispDiag

echo Tracing End Time: %Time% %Date%>> %traceFilesOutputPath%\%busesTraceInfoFileName%

rem Summary
echo.
echo ######################################################################################
echo Please collect the following files under %traceFilesOutputPath% for further analysis.
echo.
echo   %etlFileName%
echo   WPR_initiated_WprApp_*.etl
echo   %busesTraceInfoFileName%
echo   %pnpStatePreReproFileName%
echo   %pnpStatePostReproFileName%
echo   %systemEventLogsFileName%
echo   %applicationEventLogsFileName%
echo   %pnpLogsFileName%
echo   %kseLogsFileName%
echo   %ucmUcsiCxLogsFileName%
echo   %sleepStudyReportFileName%

if exist %ProgramData%\Microsoft\WDF\WudfTrace.etl (
  echo   WudfTrace.etl
)
if exist %ProgramData%\Microsoft\WDF\*.*dmp (
  for /F %%f in ('dir /b %ProgramData%\Microsoft\WDF\*.*dmp) do (
    echo   %%f
  )
)

echo   All *.dat and *.dmp files (for display diagnose)

echo.
if exist %SystemRoot%\LiveKernelReports\USB* (
  echo Please also collect the LiveKernelReports found above.
)
echo.
echo ######################################################################################
goto End

:Cleanup
echo.
echo Cleaning up previous session.  Use this if the trace script
echo was interrupted unexpectedly and a previous trace session is
echo still active. You may see errors for the trace session that
echo is not currently active.
echo.
wpr.exe -cancel
wpr.exe -cancelboot
rem Restore UMDF settings
FOR /F "tokens=3" %%v IN ('reg.exe query "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /v Buses_Backup_LogMinidumpType') DO set Buses_Backup_LogMinidumpType=%%v
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogMinidumpType /t REG_DWORD /d %Buses_Backup_LogMinidumpType% /f
FOR /F "tokens=3" %%v IN ('reg.exe query "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /v Buses_Backup_LogEnable') DO set Buses_Backup_LogEnable=%%v
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogEnable /t REG_DWORD /d %Buses_Backup_LogEnable% /f
FOR /F "tokens=3" %%v IN ('reg.exe query "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /v Buses_Backup_LogFlushPeriodSeconds') DO set Buses_Backup_LogFlushPeriodSeconds=%%v
REG.EXE ADD "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf" /v LogFlushPeriodSeconds /t REG_DWORD /d %Buses_Backup_LogFlushPeriodSeconds% /f

REG.EXE DELETE "HKLM\Software\Microsoft\windows NT\CurrentVersion\Wudf\BackupValues" /f
echo.
echo #########
echo   Done.
echo #########
goto End

:End
endlocal
echo.
pause

