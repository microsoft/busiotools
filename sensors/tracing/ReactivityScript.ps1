
$ThinkTimeSeconds = 2

$MonitorMethods = Get-WmiObject -Namespace 'root\wmi' -Class "WmiMonitorBrightnessMethods"

$MonitorBrightness = Get-CimInstance -Namespace "root\WMI" -ClassName WmiMonitorBrightness

$BrightnessLevel = $MonitorBrightness.Level

$BrightnessLevelsCount = $MonitorBrightness.Levels

for($i = 0; $i -lt 4; $i++) {

    Write-Host "Attempt setting brightness level @ Level[0] = " $BrightnessLevel[0]

    $MonitorMethods.WmiSetBrightness(0, $BrightnessLevel[0])

    Sleep -Seconds $ThinkTimeSeconds

    Write-Host "Attempt setting brightness level @ Level[$BrightnessLevelsCount - 1] = " $BrightnessLevel[$BrightnessLevelsCount - 1]

    $MonitorMethods.WmiSetBrightness(0, $BrightnessLevel[$BrightnessLevelsCount - 1])

    Sleep -Seconds $ThinkTimeSeconds
}
