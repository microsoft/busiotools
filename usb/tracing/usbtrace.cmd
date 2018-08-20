@echo off
cls
:start
echo ###########################
echo     USB AND HID TRACING
echo ###########################
echo.
echo 1) Start Tracing
echo 2) Stop Boot Session Trace
echo 3) Cleanup Previous Session
echo 4) Exit
echo.
set /p selection=Enter selection number: 
echo.
if '%selection%'=='1' goto :starttracing
if '%selection%'=='2' goto :stopboottracing_sethid
if '%selection%'=='3' goto :cleanup
if '%selection%'=='4' goto :end
echo "%selection%" is not a valid option.  Please try again.
echo.
goto start

:stopboottracing_sethid
set usehid=1
goto :stopboottracing

:cleanup
set cleanup=1
set usehid=1
echo.
echo Cleaning up previous session.  Use this if the trace script
echo was interrupted unexpectedly and a previous trace session is
echo still active. You may see errors for trace sessions that were
echo not currently active.
echo.
goto :stopsession

:starttracing
set cleanup=0
set usehid=0
set /p hid=Include HID? (y/n)
echo.
if '%hid%'=='y' set usehid=1
if '%hid%'=='Y' set usehid=1
echo Start Tracing Options:
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
if '%usehid%'=='0' goto :startUSBnow
logman create trace -n HID_WPP -o %SystemRoot%\Tracing\HID_WPP.etl -nb 128 640 -bs 128
logman update trace -n HID_WPP -p {47c779cd-4efd-49d7-9b10-9f16e5c25d06} 0x7FFFFFFF 0xFF
logman update trace -n HID_WPP -p {896f2806-9d0e-4d5f-aa25-7acdbf4eaf2c} 0x7FFFFFFF 0xFF
logman update trace -n HID_WPP -p {E742C27D-29B1-4E4B-94EE-074D3AD72836} 0x7FFFFFFF 0xFF
logman update trace -n HID_WPP -p {07699FF6-D2C0-4323-B927-2C53442ED29B} 0x7FFFFFFF 0xFF
logman update trace -n HID_WPP -p {0107cf95-313a-473e-9078-e73cd932f2fe} 0x7FFFFFFF 0xF
logman update trace -n HID_WPP -p {0a6b3bb2-3504-49c1-81d0-6a4b88b96427} 0x7FFFFFFF 0xF
logman start -n HID_WPP
:startUSBnow
logman create trace -n usbtrace -o %SystemRoot%\Tracing\usbtrace.etl -ct perf -nb 128 640 -bs 128
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBXHCI (Default,PartialDataBusTrace,StateMachine)
logman update trace -n usbtrace -p Microsoft-Windows-USB-UCX (Default,PartialDataBusTrace,StateMachine)
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBHUB3 (Default,PartialDataBusTrace,StateMachine)
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBPORT
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBHUB
logman update trace -n usbtrace -p {E98EBDBF-3058-4784-8521-47860B1D2B8E}
logman update trace -n usbtrace -p {49B12C7C-4BD5-4F93-BB75-30FCE739600B}
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
logman start -ets ufx01000 -ct perf -p {468D9E9D-07F5-4537-B650-98389559206E} 0xffffffff 0xff  -o %SystemRoot%\Tracing\ufx01000.etl
logman start -ets ufxsynopsys -ct perf -p {8650230D-68B0-476E-93ED-634490DCE145} 0xffffffff 0xff  -o %SystemRoot%\Tracing\ufxsynopsys.etl
logman start -ets ucmcx -ct perf -p {C5964C90-1824-4835-857A-5E95F8AA33B2} 0xffffffff 0xff  -o %SystemRoot%\Tracing\ucmcx.etl
logman start -ets ucmtcpcicx -ct perf -p {8DEAEA72-4C63-49A4-9B8B-25DA24DAE056} 0xffffffff 0xff  -o %SystemRoot%\Tracing\ucmtcpcicx.etl
logman start -ets ucmucsi -ct perf -p {EAD1EE75-4BFE-4E28-8AFA-E94B0A1BAF37} 0xffffffff 0xff  -o %SystemRoot%\Tracing\ucmucsi.etl
logman start -ets ucsicx -ct perf -p {C500C63A-6EFE-433B-84A7-C0740D5DC97F} 0xffffffff 0xff  -o %SystemRoot%\Tracing\ucsicx.etl
logman start -ets ucsicxacpi -ct perf -p {EDEF8E04-4E22-4A95-9D04-539EBD112A5E} 0xffffffff 0xff  -o %SystemRoot%\Tracing\ucsicxacpi.etl
logman start -ets usbtask -ct perf -p {04b3644b-27ca-4cac-9243-29bed5c91cf9} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbtask.etl
logman start -ets usbpmapi -ct perf -p {9c06e0ca-f00e-4ac3-a049-65663b654393} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbpmapi.etl
logman start -ets usbcapi -ct perf -p {C1330B70-D01E-4AA6-B30D-B2BDAF228EC3} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbcapi.etl
echo.
echo Tracing started. Reproduce the issue and hit any key to stop tracing...
pause
echo.

:stopsession
if '%usehid%'=='0' goto :stopUSBSession
logman stop -n HID_WPP
logman delete -n HID_WPP
move /Y %SystemRoot%\Tracing\HID_WPP_000001.etl %SystemRoot%\Tracing\HID_WPP.etl
:stopUSBSession
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
logman stop -ets ufx01000
logman stop -ets ufxsynopsys
logman stop -ets ucmcx
logman stop -ets ucmtcpcicx
logman stop -ets ucmucsi
logman stop -ets ucsicx
logman stop -ets ucsicxacpi
logman stop -ets usbtask
logman stop -ets usbpmapi
logman stop -ets usbcapi
reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v BuildLabEX > %SystemRoot%\Tracing\BuildNumber.txt
if '%cleanup%'=='1' goto :stopboottracing
echo.
echo Please collect log files from %SystemRoot%\Tracing for further analysis.
goto :livekernelreports

:startboottracing
echo Starting Boot Tracing
if '%usehid%'=='0' goto :startUSBboot
logman create trace autosession\HIDBootTrace -o HIDBootTrace.etl -nb 128 640 -bs 128 
logman update trace autosession\HIDBootTrace -p {47c779cd-4efd-49d7-9b10-9f16e5c25d06} 0x7FFFFFFF 0xFF
logman update trace autosession\HIDBootTrace -p {E742C27D-29B1-4E4B-94EE-074D3AD72836} 0x7FFFFFFF 0xFF
logman update trace autosession\HIDBootTrace -p {896f2806-9d0e-4d5f-aa25-7acdbf4eaf2c} 0x7FFFFFFF 0xFF
logman update trace autosession\HIDBootTrace -p {07699FF6-D2C0-4323-B927-2C53442ED29B} 0x7FFFFFFF 0xFF
logman update trace autosession\HIDBootTrace -p {0107cf95-313a-473e-9078-e73cd932f2fe} 0x7FFFFFFF 0xFF
:startUSBboot
logman create trace autosession\UsbBootTrace -o UsbBootTrace.etl -ets -nb 128 640 -bs 128 -f bincirc -max 50
logman update autosession\UsbBootTrace -ets -p Microsoft-Windows-USB-USBXHCI Default
logman update autosession\UsbBootTrace -ets -p Microsoft-Windows-USB-UCX Default,PartialDataBusTrace,StateMachine
logman update autosession\UsbBootTrace -ets -p Microsoft-Windows-USB-USBHUB3 Default,PartialDataBusTrace,StateMachine
logman update autosession\UsbBootTrace -ets -p Microsoft-Windows-USB-USBPORT
logman update autosession\UsbBootTrace -ets -p Microsoft-Windows-USB-USBHUB
logman update autosession\UsbBootTrace -ets -p {E98EBDBF-3058-4784-8521-47860B1D2B8E}
logman update autosession\UsbBootTrace -ets -p {49B12C7C-4BD5-4F93-BB75-30FCE739600B}
logman update autosession\UsbBootTrace -ets -p Microsoft-Windows-Kernel-IoTrace 0 2

echo.
echo Please reboot your PC to enable tracing.
echo Run this script and select Stop Boot Session Trace to disable.
echo.
goto :end

:stopboottracing
echo Stop Boot Session Trace
if '%usehid%'=='0' goto :stopUSBboot
logman stop HIDBootTrace
logman delete autosession\HIDBootTrace -ets
:stopUSBboot
logman stop UsbBootTrace -ets
logman delete autosession\UsbBootTrace -ets
if '%cleanup%'=='1' goto :end
echo Please collect log file boot trace file(s) from the current directory for further analysis.
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
