@echo off
set wprpFileName=BusesAllProfile.wprp
set traceFilesOutputPath=%SystemRoot%\Tracing
set wprStatusFileName=BusesWprStatus.txt
set etlFileName=BusesMergedTraces.etl
set pnpStateFileName=BusesPnpState.pnp
set pnpLogsFileName=BusesDriverWatchdog.evtx
set buildNumberFileName=BuildNumber.txt

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
else (
    cls
    goto MainMenu
)

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
echo 2) Input/HID components only (select for HID problems - keyboard, mouse, touch input, buttons etc.)
echo 3) Other options...
echo 4) Back
echo.
set /p selection=Enter selection number: 
if "%selection%"=="1" set profileName=BusesAllProfile
if "%selection%"=="2" set profileName=InputOnlyProfile
if "%selection%"=="3" goto OtherProfilesMenu
if "%selection%"=="4" goto MainMenu
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
echo 3) Usb4WithTunnelsProfile (USB3, USB4, PnP, Display, Net and PCI)
echo 4) UsbCProfile (UCM, URS and USBFN)
echo 5) LowPowerBusesProfile (SerCx2 and SpbCx)
echo 6) Back
echo.
set /p selection=Enter selection number: 
if "%selection%"=="1" set profileName=UsbOnlyProfile
if "%selection%"=="2" set profileName=UsbAndPnpProfile
if "%selection%"=="3" set profileName=Usb4WithTunnelsProfile
if "%selection%"=="4" set profileName=UsbCProfile
if "%selection%"=="5" set profileName=LowPowerBUsesProfile
if "%selection%"=="6" goto BasicProfilesMenu
if not "%profileName%"=="" goto StartOptionsMenu
echo.
echo "%selection%" is not a valid option.  Please try again.
echo.
goto OtherProfilesMenu

:StartOptionsMenu
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
if "%selection%"=="1" goto StartNow
if "%selection%"=="2" goto ConfigureBootTrace
if "%selection%"=="3" goto BasicProfilesMenu
echo.
echo "%selection%" is not a valid option.  Please try again.
echo.
goto StartOptionsMenu

:StartNow
echo Starting Tracing Now... (%wprpFileName%!%profileName%)
wpr.exe -start %wprpFileName%!%profileName% -recordTempTo %traceFilesOutputPath%\
if not %ERRORLEVEL%==0 goto End
echo Saving status to %wprStatusFileName%...
wpr -status profiles collectors -details > %traceFilesOutputPath%\%wprStatusFileName%
echo.
echo ----------------------------------------------------------------------
echo Tracing started. Reproduce the issue and hit any key to stop tracing.
echo ----------------------------------------------------------------------
pause
wpr.exe -stop %traceFilesOutputPath%\%etlFileName%
goto CollectMoreInfo

:ConfigureBootTrace
echo Configuring Boot Session Trace... (%wprpFileName%!%profileName%)
wpr.exe -addboot %wprpFileName%!%profileName%
if not %ERRORLEVEL%==0 goto End
echo.
echo #################################################################
echo Please reboot your PC to enable tracing.
echo Run this script and select "Stop Boot Session Trace" to disable.
echo #################################################################
echo.
goto End

:StopBootTrace
wpr.exe -stopboot %traceFilesOutputPath%\%etlFileName%
if not %ERRORLEVEL%==0 goto End
goto CollectMoreInfo

:CollectMoreInfo
echo.
echo Collecting more info...
rem OS Build Numbers
echo - OS build numbers...
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v BuildLabEX > %traceFilesOutputPath%\%buildNumberFileName%
rem PnP State
echo - PnP state... (If it doesn't complete within a few minutes, use CTRL-C to interrupt it.) 
if exist %traceFilesOutputPath%\%pnpStateFileName% del %traceFilesOutputPath%\%pnpStateFileName%
pnputil.exe /export-pnpstate %traceFilesOutputPath%\%pnpStateFileName%
rem Event Logs
echo - Event logs...
copy %SystemRoot%\System32\Winevt\Logs\System.evtx %traceFilesOutputPath%\
copy %SystemRoot%\System32\Winevt\Logs\Application.evtx %traceFilesOutputPath%\
if exist %traceFilesOutputPath%\%pnpLogsFileName% del %traceFilesOutputPath%\%pnpLogsFileName%
wevtutil.exe export-log "Microsoft-Windows-Kernel-PnP/Driver Watchdog" "%traceFilesOutputPath%\%pnpLogsFileName%"
rem USB Live Kernel Reports
echo - Live kernel reports...
if exist %SystemRoot%\LiveKernelReports\USB* (
    echo Found these reports for USB. They may be related.
    echo ==================================================
    dir %SystemRoot%\LiveKernelReports\USB*
    echo.
)
rem Summary
echo.
echo ######################################################################################
echo Please collect the following files under %traceFilesOutputPath% for further analysis.
echo.
echo   %etlFileName%
echo   WPR_initiated_WprApp_*.etl
echo   %buildNumberFileName%
echo   %wprStatusFileName%
echo   %pnpStateFileName%
echo   System.evtx
echo   Application.evtx
echo   %pnpLogsFileName%
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
echo.
echo #########
echo   Done.
echo #########
goto End

:End
echo.
pause
set wprpFileName=
set traceFilesOutputPath=
set etlFileName=
set pnpStateFileName=
set pnpLogsFileName=
set selection=
set profileName=
