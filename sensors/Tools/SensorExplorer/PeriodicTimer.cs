// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SensorExplorer
{
    static class PeriodicTimer
    {
        private static CoreDispatcher cd = Window.Current.CoreWindow.Dispatcher;
        private static ThreadPoolTimer periodicTimerSensorDisplay = null;
        private static ThreadPoolTimer periodicTimerMALTScenario2 = null;
        private static ThreadPoolTimer periodicTimerDEO = null;
        private static ThreadPoolTimer periodicTimerDistance = null;

        /// <summary>
        /// Create a periodic timer that fires every time the period elapses.
        /// When the timer expires, its callback handler is called and the timer is reset.
        /// This behavior continues until the periodic timer is cancelled.
        /// </summary>
        public static void CreateSensorDisplay(int index)
        {
            Sensor.SensorData[index].ClearReading();
            if (periodicTimerSensorDisplay == null)
            {
                periodicTimerSensorDisplay = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallbackSensorDisplay), new TimeSpan(0, 0, 1));
            }
        }

        public static void CancelSensorDisplay()
        {
            bool allOff = true;
            for (int i = 0; i < Sensor.SensorDisplay.Count; i++)
            {
                if (Sensor.SensorDisplay[i].IsOn)
                {
                    allOff = false;
                    break;
                }
            }

            if (allOff && periodicTimerSensorDisplay != null)
            {
                periodicTimerSensorDisplay.Cancel();
                periodicTimerSensorDisplay = null;
            }
        }

        private async static void PeriodicTimerCallbackSensorDisplay(ThreadPoolTimer timer)
        {
            await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                for (int i = 0; i < Sensor.SensorDisplay.Count; i++)
                {
                    SensorDisplay selectedDisplay = Sensor.SensorDisplay[i];
                    SensorData selectedData = Sensor.SensorData[i];
                    if (selectedDisplay.StackPanelSensor.Visibility == Visibility.Visible)
                    {
                        selectedDisplay.PlotCanvas.Plot(selectedData);
                        selectedDisplay.UpdateText(selectedData);
                    }

                    if (selectedData.ReportIntervalChanged)
                    {
                        try
                        {
                            if (selectedData.SensorType == Sensor.ACCELEROMETER)
                            {
                                Sensor.AccelerometerStandardList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.AccelerometerStandardList[selectedDisplay.Index].ReportInterval;
                            }
                            else if (selectedData.SensorType == Sensor.ACCELEROMETERGRAVITY)
                            {
                                Sensor.AccelerometerGravityList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.AccelerometerGravityList[selectedDisplay.Index].ReportInterval;
                            }
                            else if (selectedData.SensorType == Sensor.ACCELEROMETERLINEAR)
                            {
                                Sensor.AccelerometerLinearList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.AccelerometerLinearList[selectedDisplay.Index].ReportInterval;
                            }
                            else if (selectedData.SensorType == Sensor.COMPASS)
                            {
                                Sensor.CompassList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.CompassList[selectedDisplay.Index].ReportInterval;
                            }
                            else if (selectedData.SensorType == Sensor.GYROMETER)
                            {
                                Sensor.GyrometerList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.GyrometerList[selectedDisplay.Index].ReportInterval;
                            }
                            else if (selectedData.SensorType == Sensor.INCLINOMETER)
                            {
                                Sensor.InclinometerList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.InclinometerList[selectedDisplay.Index].ReportInterval;
                            }
                            else if (selectedData.SensorType == Sensor.LIGHTSENSOR)
                            {
                                Sensor.LightSensorList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.LightSensorList[selectedDisplay.Index].ReportInterval;
                            }
                            else if (selectedData.SensorType == Sensor.ORIENTATIONSENSOR)
                            {
                                Sensor.OrientationAbsoluteList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.OrientationAbsoluteList[selectedDisplay.Index].ReportInterval;
                            }
                            else if (selectedData.SensorType == Sensor.ORIENTATIONGEOMAGNETIC)
                            {
                                Sensor.OrientationGeomagneticList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.OrientationGeomagneticList[selectedDisplay.Index].ReportInterval;
                            }
                            else if (selectedData.SensorType == Sensor.ORIENTATIONRELATIVE)
                            {
                                Sensor.OrientationRelativeList[selectedDisplay.Index].ReportInterval = selectedData.ReportInterval;
                                selectedData.ReportInterval = Sensor.OrientationRelativeList[selectedDisplay.Index].ReportInterval;
                            }

                            selectedData.ReportIntervalChanged = false;
                        }
                        catch { }
                    }
                }
            });
        }

        public static void CreateMALTScenario2()
        {
            if (periodicTimerMALTScenario2 == null)
            {
                periodicTimerMALTScenario2 = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallbackMALTScenario2), new TimeSpan(0, 0, 2));
            }
        }

        public static void CancelMALTScenario2()
        {
            if (periodicTimerMALTScenario2 != null)
            {
                periodicTimerMALTScenario2.Cancel();
                periodicTimerMALTScenario2 = null;
            }
        }

        private async static void PeriodicTimerCallbackMALTScenario2(ThreadPoolTimer timer)
        {
            await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Scenario2MALT.Scenario2.GetMALTData();
            });
        }

        public static void CreateDEO()
        {
            if (periodicTimerDEO == null)
            {
                periodicTimerDEO = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallbackDEO), new TimeSpan(0, 0, 1));
            }
        }

        public static void CancelDEO()
        {
            if (periodicTimerDEO != null)
            {
                periodicTimerDEO.Cancel();
                periodicTimerDEO = null;
            }
        }

        private async static void PeriodicTimerCallbackDEO(ThreadPoolTimer timer)
        {
            await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Scenario3DEO.Scenario3.BrightnessLevelChanged();
            });
        }

        public static void CreateDistanceTimer()
        {
            if (periodicTimerDistance == null)
            {
                periodicTimerDistance = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallbackDistance), new TimeSpan(0, 0, 1));
            }
        }

        public static void CancelDistanceTimer()
        {
            if (periodicTimerDistance != null)
            {
                periodicTimerDistance.Cancel();
                periodicTimerDistance = null;
            }
        }

        private async static void PeriodicTimerCallbackDistance(ThreadPoolTimer timer)
        {
            await cd.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await Scenario4Distance.Scenario4.EvaluateStateEverySecondAsync();
            });
        }
    }
}