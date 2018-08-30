
$ThinkTimeMs = 50

$MonitorMethods = Get-WmiObject -Namespace 'root\wmi' -Class "WmiMonitorBrightnessMethods"

$MonitorBrightness = Get-CimInstance -Namespace "root\WMI" -ClassName WmiMonitorBrightness

$BrightnessLevel = $MonitorBrightness.Level

$BrightnessLevelsCount = $MonitorBrightness.Levels

for($i = 100; $i -ge 0; $i--) {

    #
    # Attempt to set a new brightness level
    #

    Write-Host "Attempt setting brightness level @ Level[$i] = " $BrightnessLevel[$i]

    $MonitorMethods.WmiSetBrightness(0, $BrightnessLevel[$i])

    Sleep -Milliseconds $ThinkTimeMs

} 
