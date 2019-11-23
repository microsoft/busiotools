// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        private static ThreadPoolTimer periodicTimer1 = null;
        private static ThreadPoolTimer periodicTimer2 = null;
        private static ThreadPoolTimer periodicTimer3 = null;
        private static ThreadPoolTimer periodicTimer4 = null;

        /// <summary>
        /// Create a periodic timer that fires every time the period elapses.
        /// When the timer expires, its callback handler is called and the timer is reset.
        /// This behavior continues until the periodic timer is cancelled.
        /// </summary>
        public static void Create(int index)
        {
            SensorData[index].ClearReading();
            if (periodicTimer1 == null)
            {
                periodicTimer1 = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), new TimeSpan(0, 0, 1));
            }
        }

        public static void Create()
        {
            if (periodicTimer2 == null)
            {
                periodicTimer2 = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback2), new TimeSpan(0, 0, 2));
            }
        }

        public static void CreateScenario1()
        {
            if (periodicTimer3 == null)
            {
                periodicTimer3 = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback3), new TimeSpan(0, 0, 2));
            }
        }

        public static void CreateScenario3()
        {
            if (periodicTimer4 == null)
            {
                periodicTimer4 = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback4), new TimeSpan(0, 0, 1));
            }
        }

        public static void Cancel()
        {
            bool allOff = true;
            for (int i = 0; i < SensorDisplay.Count; i++)
            {
                if (SensorDisplay[i].IsOn)
                {
                    allOff = false;
                    break;
                }
            }

            if (allOff && periodicTimer1 != null)
            {
                periodicTimer1.Cancel();
                periodicTimer1 = null;
            }
        }

        public static void Cancel2()
        {
            if (periodicTimer2 != null)
            {
                periodicTimer2.Cancel();
                periodicTimer2 = null;
            }
        }

        public static void Cancel3()
        {
            if (periodicTimer3 != null)
            {
                periodicTimer3.Cancel();
                periodicTimer3 = null;
            }
        }

        public static void Cancel4()
        {
            if (periodicTimer4 != null)
            {
                periodicTimer4.Cancel();
                periodicTimer4 = null;
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
                            else if (SensorData[i].SensorType == Sensor.ACCELEROMETERGRAVITY)
                            {
                                Sensor.AccelerometerGravityList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.AccelerometerGravityList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.ACCELEROMETERLINEAR)
                            {
                                Sensor.AccelerometerLinearList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.AccelerometerLinearList[SensorData[i].Count].ReportInterval;
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
                                Sensor.LightSensorList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.LightSensorList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.ORIENTATIONSENSOR)
                            {
                                Sensor.OrientationAbsoluteList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.OrientationAbsoluteList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.ORIENTATIONGEOMAGNETIC)
                            {
                                Sensor.OrientationGeomagneticList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.OrientationGeomagneticList[SensorData[i].Count].ReportInterval;
                            }
                            else if (SensorData[i].SensorType == Sensor.ORIENTATIONRELATIVE)
                            {
                                Sensor.OrientationRelativeList[SensorData[i].Count].ReportInterval = SensorData[i].ReportInterval;
                                SensorData[i].ReportInterval = Sensor.OrientationRelativeList[SensorData[i].Count].ReportInterval;
                            }

                            SensorData[i].ReportIntervalChanged = false;
                        }
                        catch { }
                    }
                }
            });
        }

        private async static void PeriodicTimerCallback2(ThreadPoolTimer timer)
        {
            await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Scenario2MALT.Scenario2.GetMALTData();
            });
        }

        private async static void PeriodicTimerCallback3(ThreadPoolTimer timer)
        {
            await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Scenario1View.Scenario1.GetMALTData();
            });
        }

        private async static void PeriodicTimerCallback4(ThreadPoolTimer timer)
        {
            await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Scenario3DEO.Scenario3.BrightnessLevelChanged();
            });
        }
    }
}