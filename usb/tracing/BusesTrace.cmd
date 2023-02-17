@echo off
set wprpFileName=BusesAllProfile.wprp
set traceFilesOutputPath=%SystemRoot%\Tracing
set wprStatusFileName=BusesWprStatus.txt
set etlFileName=BusesMergedTraces.etl
set pnpStatePreReproFileName=BusesPnpStatePreRepro.pnp
set pnpStatePostReproFileName=BusesPnpStatePostRepro.pnp
set pnpLogsFileName=BusesDriverWatchdog.evtx
set kseLogsFileName=BusesKernelShimEngine.evtx
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
echo 2) USB4 components
echo 3) Input/HID components only (select for HID problems - keyboard, mouse, touch input, buttons etc.)
echo 4) Other options...
echo 5) Back
echo.
set /p selection=Enter selection number: 
if "%selection%"=="1" set profileName=BusesAllProfile
if "%selection%"=="2" set profileName=Usb4WithTunnelsProfile
if "%selection%"=="3" set profileName=InputOnlyProfile
if "%selection%"=="4" goto OtherProfilesMenu
if "%selection%"=="5" goto MainMenu
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
rem Collect pre-repro PnP state
echo Collecting pre-repro PnP state... (If it doesn't complete within a few minutes, use CTRL-C to interrupt it.) 
if exist %traceFilesOutputPath%\%pnpStatePreReproFileName% del %traceFilesOutputPath%\%pnpStatePreReproFileName%
pnputil.exe /export-pnpstate %traceFilesOutputPath%\%pnpStatePreReproFileName%
rem Start tracing now
echo Starting Tracing Now... (%wprpFileName%!%profileName%)
wpr.exe -start %wprpFileName%!%profileName% -filemode -recordTempTo %traceFilesOutputPath%\
if not %ERRORLEVEL%==0 goto End
echo.
echo ----------------------------------------------------------------------
echo Tracing started. Reproduce the issue and hit any key to stop tracing.
echo ----------------------------------------------------------------------
pause
echo Saving status to %wprStatusFileName%...
wpr.exe -status profiles collectors -details > %traceFilesOutputPath%\%wprStatusFileName%
echo Stopping tracing...
wpr.exe -stop %traceFilesOutputPath%\%etlFileName%
goto CollectMoreInfo

:ConfigureBootTrace
rem Configure boot trace
echo Configuring Boot Session Trace... (%wprpFileName%!%profileName%)
wpr.exe -addboot %wprpFileName%!%profileName% -filemode -recordTempTo %traceFilesOutputPath%\
if not %ERRORLEVEL%==0 goto End
rem Collect pre-repro PnP state
echo Collecting pre-repro PnP state... (If it doesn't complete within a few minutes, use CTRL-C to interrupt it.) 
if exist %traceFilesOutputPath%\%pnpStatePreReproFileName% del %traceFilesOutputPath%\%pnpStatePreReproFileName%
pnputil.exe /export-pnpstate %traceFilesOutputPath%\%pnpStatePreReproFileName%
echo.
echo ###############################################################################
echo Please reboot your PC to start tracing. After reproducing the issue, run this
echo script again and select "Stop Boot Session Trace" to stop tracing.
echo ###############################################################################
echo.
goto End

:StopBootTrace
echo Saving status to %wprStatusFileName%...
wpr.exe -status profiles collectors -details > %traceFilesOutputPath%\%wprStatusFileName%
echo Stopping boot session tracing...
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
echo - Post-repro PnP state... (If it doesn't complete within a few minutes, use CTRL-C to interrupt it.) 
if exist %traceFilesOutputPath%\%pnpStatePostReproFileName% del %traceFilesOutputPath%\%pnpStatePostReproFileName%
pnputil.exe /export-pnpstate %traceFilesOutputPath%\%pnpStatePostReproFileName%
rem Event Logs
echo - Event logs...
copy %SystemRoot%\System32\Winevt\Logs\System.evtx %traceFilesOutputPath%\
copy %SystemRoot%\System32\Winevt\Logs\Application.evtx %traceFilesOutputPath%\
if exist %traceFilesOutputPath%\%pnpLogsFileName% del %traceFilesOutputPath%\%pnpLogsFileName%
if exist %traceFilesOutputPath%\%kseLogsFileName% del %traceFilesOutputPath%\%kseLogsFileName%
wevtutil.exe export-log "Microsoft-Windows-Kernel-PnP/Driver Watchdog" "%traceFilesOutputPath%\%pnpLogsFileName%"
wevtutil.exe export-log "Microsoft-Windows-Kernel-ShimEngine/Operational" "%traceFilesOutputPath%\%kseLogsFileName%"
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
echo   %pnpStatePreReproFileName%
echo   %pnpStatePostReproFileName%
echo   System.evtx
echo   Application.evtx
echo   %pnpLogsFileName%
echo   %kseLogsFileName%
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
set wprpFileName=
set traceFilesOutputPath=
set etlFileName=
set pnpStatePreReproFileName=
set pnpStatePostReproFileName=
set pnpLogsFileName=
set kseLogsFileName=
set selection=
set profileName=
echo.
pause
