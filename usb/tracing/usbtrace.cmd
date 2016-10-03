@echo off
cls
:start
echo ###########################
echo         USB TRACING
echo ###########################
echo.
echo 1) Start USB Tracing
echo 2) Stop Boot Session Trace
echo 3) Cleanup Previous Session
echo 4) Exit
echo.
set /p selection=Enter selection number: 
echo.
if '%selection%'=='1' goto :starttracing
if '%selection%'=='2' goto :stopboottracing
if '%selection%'=='3' goto :cleanup
if '%selection%'=='4' goto :end
echo "%selection%" is not a valid option.  Please try again.
echo.
goto start

:cleanup
set cleanup=1
echo.
echo Cleaning up previous session.  Use this if the trace script
echo was interrupted unexpectedly and a previous trace session is
echo still active. You may see errors for trace sessions that were
echo not currently active.
echo.
goto :stopsession

:starttracing
set cleanup=0
echo Start USB Tracing Options:
echo 1) Start Now
echo 2) Start Next Boot Session
echo 3) Back
echo.
set /p selection=Enter selection number: 
echo.
if '%selection%'=='1' goto :starttracingnow
if '%selection%'=='2' goto :startboottracing
if '%selection%'=='3' goto :start
echo "%selection%" is not a valid option.  Please try again.
echo.
goto starttracing

:starttracingnow
logman create trace -n usbtrace -o %SystemRoot%\Tracing\usbtrace.etl -ct perf -nb 128 640 -bs 128
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBXHCI (Default,PartialDataBusTrace,StateMachine)
logman update trace -n usbtrace -p Microsoft-Windows-USB-UCX (Default,PartialDataBusTrace,StateMachine)
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBHUB3 (Default,PartialDataBusTrace,StateMachine)
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBPORT
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBHUB
logman update trace -n usbtrace -p Microsoft-Windows-Kernel-IoTrace 0 2
logman start -n usbtrace
logman start -ets usbccgp -ct perf -p {bc6c9364-fc67-42c5-acf7-abed3b12ecc6} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbccgp.etl
logman start -ets winusb -ct perf -p {ef201d1b-4e45-4199-9e9e-74591f447955} 0xffffffff 0xff  -o %SystemRoot%\Tracing\winusb.etl
logman start -ets pci -ct perf -p {47711976-08c7-44ef-8fa2-082da6a30a30} 0xffffffff 0xff  -o %SystemRoot%\Tracing\pci.etl
logman start -ets usbhub3 -ct perf -p {6e6cc2c5-8110-490e-9905-9f2ed700e455} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbhub3.etl
logman start -ets ucx01000 -ct perf -p {6fb6e467-9ed4-4b73-8c22-70b97e22c7d9}  0xffffffff 0xff  -o %SystemRoot%\Tracing\ucx01000.etl
logman start -ets usbxhci -ct perf -p {9F7711DD-29AD-C1EE-1B1B-B52A0118A54C} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbxhci.etl
logman start -ets usbhub -ct perf -p {b10d03b8-e1f6-47f5-afc2-0fa0779b8188} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbhub.etl
logman start -ets usbport -ct perf -p {d75aedbe-cfcd-42b9-94ab-f47b224245dd} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbport.etl
echo.
echo Tracing started. Reproduce the issue and hit any key to stop tracing...
pause
echo.

:stopsession
logman stop -n usbtrace
logman delete -n usbtrace
move /Y %SystemRoot%\Tracing\usbtrace_000001.etl %SystemRoot%\Tracing\usbtrace.etl
logman stop -ets usbccgp
logman stop -ets winusb
logman stop -ets pci
logman stop -ets usbhub3
logman stop -ets ucx01000
logman stop -ets usbxhci
logman stop -ets usbhub
logman stop -ets usbport
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v BuildLabEX > %SystemRoot%\Tracing\BuildNumber.txt
if '%cleanup%'=='1' goto :stopboottracing
echo.
echo Please collect log files from %SystemRoot%\Tracing for further analysis.
goto :livekernelreports

:startboottracing
echo Starting Boot Tracing
logman create trace autosession\UsbBootTrace -o UsbBootTrace.etl -ets -nb 128 640 -bs 128 -f bincirc -max 50
logman update autosession\UsbBootTrace -ets -p Microsoft-Windows-USB-USBXHCI Default
logman update autosession\UsbBootTrace -ets -p Microsoft-Windows-USB-UCX Default,PartialDataBusTrace,StateMachine
logman update autosession\UsbBootTrace -ets -p Microsoft-Windows-USB-USBHUB3 Default,PartialDataBusTrace,StateMachine
echo.
echo Please reboot your PC to enable tracing.
echo Run this script and select Stop Boot Session Trace to disable.
echo.
goto :end

:stopboottracing
echo Stop Boot Session Trace
logman stop UsbBootTrace -ets
logman delete autosession\UsbBootTrace -ets
if '%cleanup%'=='1' goto :end
echo Please collect log file UsbBootTrace.etl from the current directory for further analysis.
goto :livekernelreports

:livekernelreports
if not exist %SystemRoot%\LiveKernelReports\USB* goto :end
echo.
echo Found LiveKernelReports related to USB:
echo.
dir %SystemRoot%\LiveKernelReports\USB*
echo.
echo These reports may be related to the issue under investigation.
echo Please copy them along with the tracing.
echo.
goto :end

:end
echo.
pause