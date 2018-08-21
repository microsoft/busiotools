using System;
using System.Collections.Generic;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SensorExplorer
{
    static class PeriodicTimer
    {
        public static List<SensorData> SensorData = null;
        public static List<SensorDisplay> SensorDisplay = null;

        private static CoreDispatcher cd = Window.Current.CoreWindow.Dispatcher;
        private static ThreadPoolTimer periodicTimer = null;

        /// <summary>
        /// Create a periodic timer that fires every time the period elapses.
        /// When the timer expires, its callback handler is called and the timer is reset.
        /// This behavior continues until the periodic timer is cancelled.
        /// </summary>
        public static void Create(int index)
        {
            SensorData[index].ClearReading();
            if (periodicTimer == null)
            {
                periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), new TimeSpan(0, 0, 1));
            }
        }

        private async static void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                for (int i = 0; i < SensorDisplay.Count; i++)
                {
                    if (SensorDisplay[i].StackPanelSensor.Visibility == Visibility.Visible)
                    {
                        SensorDisplay[i].PlotCanvas.Plot(SensorData[i]);
                        SensorDisplay[i].UpdateText(SensorData[i]);
                    }

                    // Update report interval
                    if (SensorData[i].ReportIntervalChanged)
                    {
                        try
                        {
                            if (SensorData[i].SensorType == Sensor.ACCELEROMETER)
                            {
                                Sensor.AccelerometerStandardList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.AccelerometerStandardList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.ACCELEROMETERLINEAR)
                            {
                                Sensor.AccelerometerLinearList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.AccelerometerLinearList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.ACCELEROMETERGRAVITY)
                            {
                                Sensor.AccelerometerGravityList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.AccelerometerGravityList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.COMPASS)
                            {
                                Sensor.CompassList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.CompassList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.GYROMETER)
                            {
                                Sensor.GyrometerList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.GyrometerList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.INCLINOMETER)
                            {
                                Sensor.InclinometerList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.InclinometerList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.LIGHTSENSOR)
                            {
                                Sensor.LightSensor.ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.LightSensor.ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.ORIENTATIONSENSOR)
                            {
                                Sensor.OrientationAbsoluteList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.OrientationAbsoluteList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.ORIENTATIONRELATIVE)
                            {
                                Sensor.OrientationRelativeList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.OrientationRelativeList[SensorData[i].Count].ReportInterval;
                            }

                            SensorData[i].ReportIntervalChanged = false;
                        }
                        catch { }

                        // Update UI
                        SensorDisplay[i].UpdateProperty(SensorData[i].DeviceId, SensorData[i].DeviceName, SensorData[i].ReportInterval, SensorData[i].MinReportInterval, SensorData[i].ReportLatency,
                                                        SensorData[i].Category, SensorData[i].PersistentUniqueId, SensorData[i].Manufacturer, SensorData[i].Model, SensorData[i].ConnectionType);
                    }
                }
            });
        }
    }
}