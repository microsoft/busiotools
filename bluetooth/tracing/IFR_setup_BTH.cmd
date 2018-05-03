rem Setting up IFR on bthport and bthusb
@REG ADD "HKLM\System\CurrentControlSet\Services\BTHUSB\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f
@REG ADD "HKLM\System\CurrentControlSet\Services\BTHPORT\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthmini
@REG ADD "HKLM\System\CurrentControlSet\Services\BTHMini\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthuart
@REG ADD "HKLM\System\CurrentControlSet\Services\BTHuart\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on hidclass and hidbth
@REG ADD "HKLM\System\CurrentControlSet\Services\HIDBTH\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthenum
@REG ADD "HKLM\System\CurrentControlSet\Services\BTHENUM\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthprint
@REG ADD "HKLM\System\CurrentControlSet\Services\BTHPRINT\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthmodem
@REG ADD "HKLM\System\CurrentControlSet\Services\BTHMODEM\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on rfcomm
@REG ADD "HKLM\System\CurrentControlSet\Services\RFCOMM\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthserv
@REG ADD "HKLM\System\CurrentControlSet\Services\bthserv\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthleenum
@REG ADD "HKLM\System\CurrentControlSet\Services\bthleenum\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on btha2dp
@REG ADD "HKLM\System\CurrentControlSet\Services\btha2dp\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthhfenum
@REG ADD "HKLM\System\CurrentControlSet\Services\bthhfenum\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthhfaud
@REG ADD "HKLM\System\CurrentControlSet\Services\bthhfaud\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f

rem Setting up IFR on bthhfhid
@REG ADD "HKLM\System\CurrentControlSet\Services\bthhfhid\Parameters" /v "VerboseOn" /t  REG_DWORD /d 0x1 /f
