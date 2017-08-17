@setlocal enabledelayedexpansion
@echo off

REM ----------------------------------------------------------------------------
REM ---------------------CUSTOMIZE THIS SECTION---------------------------------
REM ----------------------------------------------------------------------------

REM Set the following parameters for the component you want to
REM enable logging for.

REM Comma-separated list of provider names.
set ProviderNames=SpbCxWpp

REM Name of the trace session. Pick something unique.
set TraceSessionName=SpbCxWpp_Session

REM Settings for each of the providers listed in ProviderNames.
set SpbCxWpp_guid={E6086B4D-AEFF-472B-BDA7-EEC662AFBF11}
set SpbCxWpp_level=5
set SpbCxWpp_keywords=0xFFFFFFFFFFFFFFFF


REM Force use of logman.exe on systems where tracelog.exe is available.
REM set USELOGMAN=1

REM Verbose output for debugging.
REM set VERBOSE=1

REM Make it a circular session with the following maximum file size.
REM set MaxFileSizeInMB=64

REM Configurable settings.
set BufferSizeInKB=256

REM ----------------------------------------------------------------------------
REM -------------------------END OF SECTION-------------------------------------
REM ----------------------------------------------------------------------------

tracelog.exe 1>NUL 2>NUL
if !ERRORLEVEL!==9009 (
    logman.exe 1>NUL 2>NUL
    if !ERRORLEVEL!==9009 (
        echo Could not find logman.exe or tracelog.exe^^!
        goto :eof
    )
    set USELOGMAN=1
)


if /I "%~1" EQU "start" (

    if /I "%~2" EQU "autologger" (

        if [%3] == [] (
            goto :ShowUsage
        )

        if /I "%~4" EQU "-TShell" (

            if [%5] == [] (
                call :RunOverTShell 127.0.0.1 "%~f0" "start" "start autologger %~nx3"
            ) else (
                call :RunOverTShell %5 "%~f0" "start" "start autologger %~nx3"
            )

        ) else (
            call :StartAutologgerCommand "%~3"
        )

    ) else if /I "%~2" EQU "offlinehive" (

        if [%3] == [] (
            goto :ShowUsage
        )

        call :StartOfflineHiveCommand "%~3" %4

    ) else (

        if "%~2" EQU "" (
            goto :ShowUsage
        )

        if /I "%~3" EQU "-TShell" (

            if [%4] == [] (
                call :RunOverTShell 127.0.0.1 "%~f0" "start" "start %~nx2"
            ) else (
                call :RunOverTShell %4 "%~f0" "start" "start %~nx2"
            )

        ) else (
            call :StartCommand "%~2"
        )
    )

) else if /I "%~1" EQU "stop" (

    if /I "%~2" EQU "-TShell" (

        if [%3] == [] (
            call :RunOverTShell 127.0.0.1 "%~f0" "stop" "stop"
        ) else (
            call :RunOverTShell %3 "%~f0" "stop" "stop"
        )

    ) else if "%~2" EQU "" (

        call :StopCommand

    ) else (

        call :StopOfflineHiveCommand "%~2"
    )

) else (

    goto :ShowUsage

)

goto :eof

--------------------------------------------------------------------------------
:RunOverTShell

setlocal

set IpAddress=%~1
set ScriptFilePath="%~2"
set ScriptFileName=%~nx2
set Operation=%~3
set Command=%~4


if /I "%Operation%" EQU "start" (
    set TShellCmd=Open-Device %IpAddress%
    set TShellCmd=!TShellCmd!;cdd C:\Test
    set TShellCmd=!TShellCmd!;mkdird %TraceSessionName%
    set TShellCmd=!TShellCmd!;cdd %TraceSessionName%
    set TShellCmd=!TShellCmd!;putd %ScriptFilePath%
    set TShellCmd=!TShellCmd!;execd %ScriptFileName% %Command%

    if defined VERBOSE (
        echo TShell command: !TShellCmd!
    )

) else if /I "%Operation%" EQU "stop" (

    set TShellCmd=Open-Device %IpAddress%
    set TShellCmd=!TShellCmd!;cdd C:\Test\%TraceSessionName%
    set TShellCmd=!TShellCmd!;execd %ScriptFileName% %Command%
    set TShellCmd=!TShellCmd!;getd *.etl*
    set TShellCmd=!TShellCmd!;cdd ..
    set TShellCmd=!TShellCmd!;rmdird /S /Q C:\Test\%TraceSessionName%

    if defined VERBOSE (
        echo TShell command: !TShellCmd!
    )
)

%SystemRoot%\syswow64\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Unrestricted -Command "%TShellCmd%"

endlocal
goto :eof

--------------------------------------------------------------------------------
:ConfigureAutologgerHive

set WmiKey=%~1

call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v Start /t REG_DWORD /d 1 /f
call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v Guid /t REG_SZ /d "{00000000-0000-0000-0000-000000000000}" /f
call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v FlushTimer /t REG_DWORD /d 1 /f
call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v BufferSize /t REG_DWORD /d %BufferSizeInKB% /f

if [%~2] == [-kd] (
    call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v LogFileMode /t REG_DWORD /d 0x80100 /f
) else (

    if [%~2] == [-inmem] (

        call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v LogFileMode /t REG_DWORD /d 0x400 /f

    ) else (

        if not [%~2] == [] (
            call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v FileName /t REG_SZ /d "%~f2" /f
        )

        if defined MaxFileSizeInMB (
            call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v LogFileMode /t REG_DWORD /d 0x2 /f
            call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v MaxFileSize /t REG_DWORD /d %MaxFileSizeInMB% /f
        )

        call :ExecCmd reg add %WmiKey%\%TraceSessionName% /v FileMax /t REG_DWORD /d 0x10 /f
    )
)

for /D %%P in (%ProviderNames%) do (

    set ProviderGuid=!%%P_guid!
    set ProviderLevel=!%%P_level!
    set ProviderKeywords=!%%P_keywords!
    set ProviderKey="!WmiKey!\!TraceSessionName!\!ProviderGuid!"
    call :ExecCmd reg add !ProviderKey! /v Enabled /t REG_DWORD /d 1 /f
    call :ExecCmd reg add !ProviderKey! /v EnableLevel /t REG_DWORD /d !ProviderLevel! /f
    call :ExecCmd reg add !ProviderKey! /v MatchAnyKeyword /t REG_QWORD /d !ProviderKeywords! /f
)

echo Tracing configured. Reboot for it to take effect.

goto :eof

--------------------------------------------------------------------------------
:StartOfflineHiveCommand

call :CheckIsElevated
if %ERRORLEVEL% EQU 0 (
    goto :eof
)

set OfflinePath="%~1\Windows\System32\Config\SYSTEM"
set KeyName=HIVE%RANDOM%

call :ExecCmd reg load HKLM\%KeyName% %OfflinePath%

set WmiKey=HKLM\%KeyName%\ControlSet001\Control\WMI\Autologger

call :ConfigureAutologgerHive %WmiKey%

call :ExecCmd reg unload HKLM\%KeyName%

goto :eof

--------------------------------------------------------------------------------
:StopOfflineHiveCommand

call :CheckIsElevated
if %ERRORLEVEL% EQU 0 (
    goto :eof
)

set OfflinePath="%~1\Windows\System32\Config\SYSTEM"
set KeyName=HIVE%RANDOM%

call :ExecCmd reg load HKLM\%KeyName% %OfflinePath%

set WmiKey=HKLM\%KeyName%\ControlSet001\Control\WMI\Autologger

call :ExecCmd reg delete HKLM\%KeyName%\ControlSet001\Control\WMI\Autologger\%TraceSessionName% /f

call :ExecCmd reg unload HKLM\%KeyName%

echo Tracing configuration deleted.

goto :eof

--------------------------------------------------------------------------------
:StartAutologgerCommand

call :CheckIsElevated
if %ERRORLEVEL% EQU 0 (
    goto :eof
)

set WmiKey=HKLM\System\CurrentControlSet\Control\WMI\Autologger

call :ConfigureAutologgerHive %WmiKey% "%~1"

goto :eof

--------------------------------------------------------------------------------
:StartCommand

call :CheckIsElevated
if %ERRORLEVEL% EQU 0 (
    goto :eof
)

if defined USELOGMAN (

    if [%~1] == [-kd] (
        echo -kd switch for a non-autologger trace session requires TraceLog.exe^^!
        exit /B 1
    )

    call :StartCommandLogMan "%~1"
) else (
    call :StartCommandTraceLog "%~1"
)

echo Tracing started.

goto :eof

--------------------------------------------------------------------------------
:StartCommandLogMan

if [%~1] == [-inmem] (
    call :ExecCmd logman.exe create trace -n %TraceSessionName% -mode bufferonly -bs %BufferSizeInKB%
) else (
    if defined MaxFileSizeInMB (
        call :ExecCmd logman.exe create trace -n %TraceSessionName% -o "%~f1" -ft 1 -f bincirc -max %MaxFileSizeInMB% -bs %BufferSizeInKB%
    ) else (
        call :ExecCmd logman.exe create trace -n %TraceSessionName% -o "%~f1" -ft 1 -bs %BufferSizeInKB%
    )
)

for /D %%P in (%ProviderNames%) do (
    set ProviderGuid=!%%P_guid!
    set ProviderLevel=!%%P_level!
    set ProviderKeywords=!%%P_keywords!
    call :ExecCmd logman.exe update trace -n %TraceSessionName% -p !ProviderGuid! !ProviderKeywords! !ProviderLevel!
)

call :ExecCmd logman.exe start -n %TraceSessionName%

goto :eof

--------------------------------------------------------------------------------
:StartCommandTraceLog

if [%~1] == [-kd] (
    call :ExecCmd tracelog.exe -start %TraceSessionName% -rt -kd
) else (
    if [%~1] == [-inmem] (
        call :ExecCmd tracelog.exe -start %TraceSessionName% -b %BufferSizeInKB% -buffering
    ) else (
        if defined MaxFileSizeInMB (
            call :ExecCmd tracelog.exe -start %TraceSessionName% -f "%~f1" -ft 1 -b %BufferSizeInKB% -cir %MaxFileSizeInMB%
        ) else (
            call :ExecCmd tracelog.exe -start %TraceSessionName% -f "%~f1" -ft 1 -b %BufferSizeInKB%
        )
    )
)

for /D %%P in (%ProviderNames%) do (
    set ProviderGuid=!%%P_guid!
    set ProviderLevel=!%%P_level!
    set ProviderKeywords=!%%P_keywords!
    call :ExecCmd tracelog.exe -enableex %TraceSessionName% -guid #!ProviderGuid! -matchanykw !ProviderKeywords! -level !ProviderLevel!
)

goto :eof

--------------------------------------------------------------------------------
:StopCommand

call :CheckIsElevated
if %ERRORLEVEL% EQU 0 (
    goto :eof
)

if defined USELOGMAN (
    call :StopCommandLogMan
) else (
    call :StopCommandTraceLog
)

call :ExecCmdIgnoreError reg delete HKLM\SYSTEM\CurrentControlSet\Control\WMI\Autologger\%TraceSessionName% /f

echo Tracing stopped.

goto :eof

--------------------------------------------------------------------------------
:StopCommandLogMan

call :ExecCmdIgnoreError logman.exe stop -n %TraceSessionName% -ets
call :ExecCmdIgnoreError logman.exe delete -n %TraceSessionName%

goto :eof

--------------------------------------------------------------------------------
:StopCommandTraceLog

call :ExecCmdIgnoreError tracelog.exe -stop %TraceSessionName%
call :ExecCmdIgnoreError tracelog.exe -remove %TraceSessionName%

goto :eof

--------------------------------------------------------------------------------
:ExecCmd

call :ExecCmdIgnoreError %*

if %ERRORLEVEL% NEQ 0 (
    echo Error on command: %*
    exit /B %ERRORLEVEL%
)

goto :eof

--------------------------------------------------------------------------------
:ExecCmdIgnoreError

if defined VERBOSE (
    echo Executing: %*
    %*
    echo ReturnCode: %ERRORLEVEL%
) else (
    %* 1> nul 2>&1
)

goto :eof

--------------------------------------------------------------------------------
:CheckIsElevated

REM Make a best effort to see if the script is running elevated.
openfiles >nul 2>&1
if !ERRORLEVEL! NEQ 9009 (
    if !ERRORLEVEL! NEQ 0 (
        echo Please run this script from an elevated command prompt^^!
        exit /B 0
    )
)

exit /B 1

--------------------------------------------------------------------------------
:ShowUsage

echo.
echo Usage:
echo.
echo     %~nx0 start ^<FileName^|-kd^|-inmem^> [-TShell [IP Address]]
echo     %~nx0 stop [-TShell [IP Address]]
echo.
echo     %~nx0 start autologger ^<FileName^|-kd^|-inmem^> [-TShell [IP Address]]
echo     %~nx0 stop [-TShell [IP Address]]
echo.
echo     %~nx0 start offlinehive ^<PathToRoot^> [-kd^|-inmem]
echo     %~nx0 stop ^<PathToRoot^>
echo.
echo Examples:
echo.
echo     %~nx0 start Trace.etl
echo         - Start a trace and dump it to Trace.etl in the current directory.
echo.
echo     %~nx0 stop
echo         - Stop a trace that was started earlier using this script (%~nx0).
echo.
echo     %~nx0 start Trace.etl -TShell
echo         - Connect to a Phone over T-Shell on 127.0.0.1 and start a trace.
echo.
echo     %~nx0 start Trace.etl -TShell 10.132.6.123
echo         - Connect to a Phone over T-Shell on 10.132.6.123 and start a trace.
echo.
echo     %~nx0 start -kd
echo         - Start a trace that sends messages to the kernel debugger. This needs TraceLog.exe.
echo.
echo     %~nx0 start -inmem
echo         - Start a trace that logs to an in-memory buffer. You can view the logs in the kernel
echo           debugger using the ^^!wmitrace debugger extension.
echo.
echo     %~nx0 start autologger Trace.etl
echo         - Start a trace that will persist across boots.
echo.
echo     %~nx0 start autologger -kd
echo         - An autologger trace that sends messages to the kernel debugger.
echo.
echo     %~nx0 start autologger -inmem
echo         - An autologger trace that logs to an in-memory buffer.
echo.
echo     %~nx0 start offlinehive F:
echo         - Configure an autologger in the offline system mounted at F:\
echo           Creates ETL files %%SystemRoot%%\System32\LogFiles\WMI\%TraceSessionName%.etl.*
echo.
echo     %~nx0 start offlinehive F: -kd
echo         - Configure an autologger in the offline system mounted at F:\
echo           Messages go straight to the kernel debugger.
echo.
echo     %~nx0 start offlinehive F: -inmem
echo         - Configure an autologger in the offline system mounted at F:\
echo           Messages go to an in-memory buffer.
echo.
echo     %~nx0 stop F:
echo         - Delete the autologger that was configured in the offline system mounted at F:\
echo.
goto :eof

endlocal
