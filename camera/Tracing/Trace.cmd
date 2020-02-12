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
    REM Remove Zone.Identifier in case the script was marked as downloaded from the Internet.

    %ps% -Command "Get-ChildItem %~dp0 -Include *.ps1,*.psm1 -Recurse | Unblock-File" > nul

    if "%2" == "" (
        %ps% -ExecutionPolicy Bypass -File "%~dp0Trace.ps1" %3 %4 %5 %6 %7 %8 %9
    ) else (
        %ps% -ExecutionPolicy Bypass -File "%~dp0Trace.ps1" -Scenario %2 %3 %4 %5 %6 %7 %8 %9
    )
) else (
    %ps% -Command "Start-Process %0 -Verb Runas -ArgumentList 'elevated %1 %2 %3 %4 %5 %6 %7 %8'"
)

goto :EOF

:Usage
echo.
echo Capture ETW traces.
echo.
echo A variety of ETW provider types are supported:
echo Manifest-based ETW, TraceLogging, EventSource, WPP, one-line ETW.
echo.
echo USAGE:
echo     Trace             Capture using the default scenario.
echo     Trace scenario    Capture using a particular scenario.
echo.
echo Scenarios control which set of ETW event providers get enabled.
