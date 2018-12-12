:: Copyright (c) Microsoft Corporation. All rights reserved.
:: Licensed under the MIT License.

@echo off
rem cls

for /f "tokens=4 delims= " %%i IN ('powercfg -q ^| find "Power Scheme GUID:"') do Set StrOne=%%i
for /f "tokens=3 delims= " %%i IN ('powercfg -q ^| find "(Power buttons and lid)"') do Set StrTwo=%%i 
for /f "tokens=4 delims= " %%i IN ('powercfg -q ^| find "(Lid close action)"') do Set StrThree=%%i 

rem Set Background to white 
reg add "HKEY_CURRENT_USER\Control Panel\Colors" /v Background /t REG_SZ /d "255 255 255" /f
reg add "HKEY_CURRENT_USER\Control Panel\Desktop" /v Wallpaper /t REG_SZ /d " " /f

rem Disable Sleep on DC power and AC power
powercfg -SETACVALUEINDEX %StrOne% %StrTwo% %StrThree% 000
powercfg -SETDCVALUEINDEX %StrOne% %StrTwo% %StrThree% 000

rem Turn on autobrightness on DC power and on AC power
powercfg -setdcvalueindex %StrOne% 7516b95f-f776-4464-8c53-06167f40cc99 FBD9AA66-9553-4097-BA44-ED6E9D65EAB8 1
powercfg -setacvalueindex %StrOne% 7516b95f-f776-4464-8c53-06167f40cc99 FBD9AA66-9553-4097-BA44-ED6E9D65EAB8 1


:start
    echo ###########################
    echo     MALT Test Setup
    echo ###########################
    echo.
    echo 1) Prepare for Auto Brightness functional testing
    echo 2) Prepare for Manual Brightness functional testing
    echo 3) Prepare for Auto Brightness stress testing 
    echo 4) Exit and enable Sleep
    echo.
    set /p selection=Enter selection number: 
    echo.
    if '%selection%'=='1' goto absfunctional
    if '%selection%'=='2' goto manualfunctional
    if '%selection%'=='3' goto absstress
    if '%selection%'=='4' goto resetSystem
    echo "%selection%" is not a valid option.  Please try again.
    echo.
    goto :start

:absfunctional
    rem Sleep, autobrightness and background already set.
    goto end

:manualfunctional
    rem Sleep and background already set.  Redoing auobrightness
    rem Turn off autobrightness on DC power and on AC power
    powercfg -setdcvalueindex %StrOne% 7516b95f-f776-4464-8c53-06167f40cc99 FBD9AA66-9553-4097-BA44-ED6E9D65EAB8 0
    powercfg -setacvalueindex %StrOne% 7516b95f-f776-4464-8c53-06167f40cc99 FBD9AA66-9553-4097-BA44-ED6E9D65EAB8 0
    goto end

:absstress
    rem autobrightness and background already set.  Redoing sleep
    rem Enable Sleep on DC power and AC power
    powercfg -SETACVALUEINDEX %StrOne% %StrTwo% %StrThree% 001
    powercfg -SETDCVALUEINDEX %StrOne% %StrTwo% %StrThree% 001
    goto end

:resetSystem
    rem Set Background to black 
    reg add "HKEY_CURRENT_USER\Control Panel\Colors" /v Background /t REG_SZ /d "0 0 0" /f
    reg add "HKEY_CURRENT_USER\Control Panel\Desktop" /v Wallpaper /t REG_SZ /d " " /f

    rem Enable Sleep on DC power and AC power
    powercfg -SETACVALUEINDEX %StrOne% %StrTwo% %StrThree% 001
    powercfg -SETDCVALUEINDEX %StrOne% %StrTwo% %StrThree% 001
    goto end

:end
echo Reboot so settings will take
shutdown /r /t 10
