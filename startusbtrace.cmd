logman create trace -n usbtrace -o %SystemRoot%\Tracing\usbtrace.etl -ct perf -nb 128 640 -bs 128
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBXHCI (Default,PartialDataBusTrace,StateMachine)
logman update trace -n usbtrace -p Microsoft-Windows-USB-UCX (Default,PartialDataBusTrace,StateMachine)
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBHUB3 (Default,PartialDataBusTrace,StateMachine)
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBPORT
logman update trace -n usbtrace -p Microsoft-Windows-USB-USBHUB
logman update trace -n usbtrace -p Microsoft-Windows-Kernel-IoTrace 0 2
logman start -n usbtrace
logman start -ets usbhub3 -ct perf -p {6e6cc2c5-8110-490e-9905-9f2ed700e455} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbhub3.etl
logman start -ets ucx01000 -ct perf -p {6fb6e467-9ed4-4b73-8c22-70b97e22c7d9}  0xffffffff 0xff  -o %SystemRoot%\Tracing\ucx01000.etl
logman start -ets usbxhci -ct perf -p {9F7711DD-29AD-C1EE-1B1B-B52A0118A54C} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbxhci.etl
logman start -ets usbhub -ct perf -p {b10d03b8-e1f6-47f5-afc2-0fa0779b8188} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbhub.etl
logman start -ets usbport -ct perf -p {d75aedbe-cfcd-42b9-94ab-f47b224245dd} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbport.etl
logman start -ets usbccgp -ct perf -p {bc6c9364-fc67-42c5-acf7-abed3b12ecc6} 0xffffffff 0xff  -o %SystemRoot%\Tracing\usbccgp.etl
logman start -ets winusb -ct perf -p {ef201d1b-4e45-4199-9e9e-74591f447955} 0xffffffff 0xff  -o %SystemRoot%\Tracing\winusb.etl
logman start -ets pci -ct perf -p {47711976-08c7-44ef-8fa2-082da6a30a30} 0xffffffff 0xff  -o %SystemRoot%\Tracing\pci.etl