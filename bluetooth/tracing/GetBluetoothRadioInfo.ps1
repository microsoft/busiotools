# Copyright and License https://github.com/Microsoft/busiotools/blob/master/LICENSE

$devices = Get-PnpDevice -Class Bluetooth |? InstanceId -notlike "BTH*"

$radios = New-Object System.Collections.ArrayList
foreach ($device in $devices)
{   
    $radio = New-Object PSObject
    Add-Member -InputObject $radio -MemberType NoteProperty -Name "InstanceId" -Value $device.InstanceId
    $radios.Add($radio) | Out-Null

    # Driver Info
    $property = Get-PnpDeviceProperty -InstanceId $device.InstanceId -KeyName 'DEVPKEY_Device_DriverDesc'
    Add-Member -InputObject $radio -MemberType NoteProperty -Name "DriverDescription" -Value $property.Data
    $property = Get-PnpDeviceProperty -InstanceId $device.InstanceId -KeyName 'DEVPKEY_Device_DriverVersion'
    Add-Member -InputObject $radio -MemberType NoteProperty -Name "DriverVersion" -Value $property.Data

    # Error Recovery
    $property = Get-PnpDeviceProperty -InstanceId $device.InstanceId -KeyName '{A92F26CA-EDA7-4B1D-9DB2-27B68AA5A2EB} 14'
    $supportedTypes = $property.Data
    if ($supportedTypes -eq 0)
    {
        Add-Member -InputObject $radio -MemberType NoteProperty -Name "ErrorRecovery" -Value "None"
    } elseif ($supportedTypes -band 1 -shl 0)
    {
        Add-Member -InputObject $radio -MemberType NoteProperty -Name "ErrorRecovery" -Value "FLDR"
    } elseif ($supportedTypes -band 1 -shl 1)
    {
        Add-Member -InputObject $radio -MemberType NoteProperty -Name "ErrorRecovery" -Value "PLDR"
    }
    
}
$radios | Format-Table
