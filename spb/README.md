# Simple Peripheral Buses (I2C and SPI)

## SpbCxWppTrace.cmd
### Capture Traces
### Usage:

    SpbCxWppTrace.cmd start <FileName|-kd|-inmem> [-TShell [IP Address]]
    SpbCxWppTrace.cmd stop [-TShell [IP Address]]

    SpbCxWppTrace.cmd start autologger <FileName|-kd|-inmem> [-TShell [IP Address]]
    SpbCxWppTrace.cmd stop [-TShell [IP Address]]

    SpbCxWppTrace.cmd start offlinehive <PathToRoot> [-kd|-inmem]
    SpbCxWppTrace.cmd stop <PathToRoot>

### Examples:

    SpbCxWppTrace.cmd start Trace.etl
        - Start a trace and dump it to Trace.etl in the current directory.

    SpbCxWppTrace.cmd stop
        - Stop a trace that was started earlier using this script (SpbCxWppTrace.cmd).

	SpbCxWppTrace.cmd start autologger Trace.etl
        - Start a trace that will persist across boots.

    SpbCxWppTrace.cmd start autologger -kd
        - An autologger trace that sends messages to the kernel debugger.

View the resulting WPP traces with TraceView, available in the Windows SDK.

Click here to download the BUS IO Tools package as a zip file: https://github.com/Microsoft/busiotools/archive/master.zip


[Back to root](http://aka.ms/bustools) 
