cd /d "C:\Users\jialzhu\Desktop\SensorExplorer\SensorExplorer" &msbuild "SensorExplorer.csproj" /t:sdvViewer /p:configuration="Debug" /p:platform=x86
exit %errorlevel% 