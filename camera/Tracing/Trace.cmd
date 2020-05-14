@echo off
setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION

REM Redirect /?, /h, etc. to help.
for %%a in (./ .- .) do if ".%~1." == "%%a?." goto Usage
for %%a in (./ .- .) do if ".%~1." == "%%ah." goto Usage

powershell.exe /?  > nul 2>&1
 if ERRORLEVEL 9009 ( 
    SET ps=pwsh.exe
) ELSE ( 
    SET ps=powershell.exe
)

if "%1" == "elevated" (
    REM Remove Zone.Identifier in case the script was marked as downloaded from the Internet
    %ps% -Command "Get-ChildItem %~dp0 -Include *.ps1,*.psm1 -Recurse | Unblock-File" > nul

    %ps% -ExecutionPolicy Bypass -File "%~dp0Trace.ps1" 

) else (
    %ps% -Command "Start-Process %0 -Verb Runas -ArgumentList 'elevated %1 %2 %3 %4 %5 %6 %7 %8'"
    REM In FactoryOS, launching newshell using start-process is not supported, just try to run the script directly.
    if ERRORLEVEL NEQ 0 (
        %ps% -Command "Get-ChildItem %~dp0 -Include *.ps1,*.psm1 -Recurse | Unblock-File" > nul
        %ps% -ExecutionPolicy Bypass -File "%~dp0Trace.ps1" 
    )
)

goto :EOF

:Usage
echo.
echo Capture ETW traces.
echo.
echo A variety of ETW provider types are supported:
echo TraceLogging, EventSource, WPP, one-line ETW.
echo.
echo USAGE:
echo     Trace             Capture trace for multimedia scenario
echo.
