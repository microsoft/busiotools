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
        public static List<SensorData> sensorData = null;
        public static List<SensorDisplay> sensorDisplay = null;

        private static CoreDispatcher _cd = Window.Current.CoreWindow.Dispatcher;
        private static ThreadPoolTimer _periodicTimer = null;
        private static ThreadPoolTimer _periodicTimer2 = null;
        private static ThreadPoolTimer _periodicTimer3 = null;
        private static ThreadPoolTimer _periodicTimer4 = null;

        /// <summary>
        /// Create a periodic timer that fires every time the period elapses.
        /// When the timer expires, its callback handler is called and the timer is reset.
        /// This behavior continues until the periodic timer is cancelled.
        /// </summary>
        public static void Create(int index)
        {
            sensorData[index].ClearReading();
            if (_periodicTimer == null)
            {
                _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), new TimeSpan(0, 0, 1));
            }
        }

        public static void Create()
        {
            if (_periodicTimer2 == null)
            {
                _periodicTimer2 = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback2), new TimeSpan(0, 0, 2));
            }
        }

        public static void CreateScenario1()
        {
            if (_periodicTimer3 == null)
            {
                _periodicTimer3 = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback3), new TimeSpan(0, 0, 2));
            }
        }

        public static void CreateScenario3()
        {
            if (_periodicTimer4 == null)
            {
                _periodicTimer4 = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback4), new TimeSpan(0, 0, 1));
            }
        }

        public static void Cancel()
        {
            bool allOff = true;
            for (int i = 0; i < sensorDisplay.Count; i++)
            {
                if (sensorDisplay[i]._isOn)
                {
                    allOff = false;
                    break;
                }
            }

            if (allOff && _periodicTimer != null)
            {
                _periodicTimer.Cancel();
                _periodicTimer = null;
            }
        }

        public static void Cancel2()
        {
            if (_periodicTimer2 != null)
            {
                _periodicTimer2.Cancel();
                _periodicTimer2 = null;
            }
        }

        public static void Cancel3()
        {
            if (_periodicTimer3 != null)
            {
                _periodicTimer3.Cancel();
                _periodicTimer3 = null;
            }
        }

        public static void Cancel4()
        {
            if (_periodicTimer4 != null)
            {
                _periodicTimer4.Cancel();
                _periodicTimer4 = null;
            }
        }

        private async static void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                for (int i = 0; i < sensorDisplay.Count; i++)
                {
                    if (sensorDisplay[i].StackPanelSensor.Visibility == Visibility.Visible)
                    {
                        sensorDisplay[i]._plotCanvas.Plot(sensorData[i]);
                        sensorDisplay[i].UpdateText(sensorData[i]);
                    }

                    // Update report interval
                    if (sensorData[i]._reportIntervalChanged)
                    {
                        try
                        {
                            if (sensorData[i]._sensorType == Sensor.ACCELEROMETER)
                            {
                                Sensor.AccelerometerStandardList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.AccelerometerStandardList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.ACCELEROMETERLINEAR)
                            {
                                Sensor.AccelerometerLinearList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.AccelerometerLinearList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.ACCELEROMETERGRAVITY)
                            {
                                Sensor.AccelerometerGravityList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.AccelerometerGravityList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.COMPASS)
                            {
                                Sensor.CompassList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.CompassList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.GYROMETER)
                            {
                                Sensor.GyrometerList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.GyrometerList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.INCLINOMETER)
                            {
                                Sensor.InclinometerList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.InclinometerList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.LIGHTSENSOR)
                            {
                                Sensor.LightSensorList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.LightSensorList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.ORIENTATIONSENSOR)
                            {
                                Sensor.OrientationAbsoluteList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.OrientationAbsoluteList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.ORIENTATIONRELATIVE)
                            {
                                Sensor.OrientationRelativeList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.OrientationRelativeList[sensorData[i]._count].ReportInterval;
                            }

                            sensorData[i]._reportIntervalChanged = false;
                        }
                        catch { }
                    }
                }
            });
        }

        private async static void PeriodicTimerCallback2(ThreadPoolTimer timer)
        {
            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Scenario2MALT.Scenario2.GetMALTData();
            });
        }

        private async static void PeriodicTimerCallback3(ThreadPoolTimer timer)
        {
            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Scenario1View.Scenario1.GetMALTData();
            });
        }

        private async static void PeriodicTimerCallback4(ThreadPoolTimer timer)
        {
            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Scenario3DEO.Scenario3.BrightnessLevelChanged();
            });
        }
    }
}