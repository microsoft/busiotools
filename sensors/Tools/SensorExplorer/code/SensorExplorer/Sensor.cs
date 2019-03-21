// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Custom;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SensorExplorer
{
    static class Sensor
    {
        public const int ACCELEROMETER = 0;
        public const int ACCELEROMETERLINEAR = 1;
        public const int ACCELEROMETERGRAVITY = 2;
        public const int COMPASS = 3;
        public const int GYROMETER = 4;
        public const int INCLINOMETER = 5;
        public const int LIGHTSENSOR = 6;
        public const int COLORSENSOR = 22;
        public const int ORIENTATIONSENSOR = 7;
        public const int ORIENTATIONRELATIVE = 8;
        public const int ORIENTATIONGEOMAGNETIC = 9;
        public const int ACTIVITYSENSOR = 10;
        public const int ALTIMETER = 11;
        public const int BAROMETER = 12;
        public const int MAGNETOMETER = 13;
        public const int PEDOMETER = 14;
        public const int PROXIMITYSENSOR = 15;
        public const int SIMPLEORIENTATIONSENSOR = 16;
        public const int CO2SENSOR = 17;
        public const int HEARTRATESENSOR = 18;
        public const int HUMIDITYSENSOR = 19;
        public const int UVSENSOR = 20;
        public const int TEMPERATURESENSOR = 21;

        public const int ACTIVITYNONE = 2;
        public const int ACTIVITYNOTSUPPORTED = 3;

        public static int currentId = -1;

        public static List<SensorData> sensorData;
        public static List<SensorDisplay> sensorDisplay;

        public static List<Accelerometer> AccelerometerStandardList;
        public static List<DeviceInformation> AccelerometerStandardDeviceInfo;
        public static List<Accelerometer> AccelerometerLinearList;
        public static List<DeviceInformation> AccelerometerLinearDeviceInfo;
        public static List<Accelerometer> AccelerometerGravityList;
        public static List<DeviceInformation> AccelerometerGravityDeviceInfo;
        public static List<ActivitySensor> ActivitySensorList;
        public static List<DeviceInformation> ActivitySensorDeviceInfo;
        public static Altimeter Altimeter;
        public static DeviceInformation AltimeterDeviceInfo;
        public static List<Barometer> BarometerList;
        public static List<DeviceInformation> BarometerDeviceInfo;
        public static List<Compass> CompassList;
        public static List<DeviceInformation> CompassDeviceInfo;
        public static List<Gyrometer> GyrometerList;
        public static List<DeviceInformation> GyrometerDeviceInfo;
        public static List<Inclinometer> InclinometerList;
        public static List<DeviceInformation> InclinometerDeviceInfo;
        public static List<LightSensor> LightSensorList;
        public static List<DeviceInformation> LightSensorDeviceInfo;
        public static List<Magnetometer> MagnetometerList;
        public static List<DeviceInformation> MagnetometerDeviceInfo;
        public static List<OrientationSensor> OrientationAbsoluteList;
        public static List<DeviceInformation> OrientationAbsoluteDeviceInfo;
        public static List<OrientationSensor> OrientationGeomagneticList;
        public static List<DeviceInformation> OrientationGeomagneticDeviceInfo;
        public static List<OrientationSensor> OrientationRelativeList;
        public static List<DeviceInformation> OrientationRelativeDeviceInfo;
        public static List<Pedometer> PedometerList;
        public static List<DeviceInformation> PedometerDeviceInfo;
        public static List<ProximitySensor> ProximitySensorList;
        public static List<DeviceInformation> ProximitySensorDeviceInfo;
        public static List<SimpleOrientationSensor> SimpleOrientationSensorList;
        public static List<DeviceInformation> SimpleOrientationSensorDeviceInfo;

        public static DeviceInformationCollection SensorClassDevice;

        public static int NumFailedEnumerations;

        public static bool AccelerometerStandardFailed;
        public static bool AccelerometerLinearFailed;
        public static bool AccelerometerGravityFailed;
        public static bool ActivitySensorFailed;
        public static bool AltimeterFailed;
        public static bool BarometerFailed;
        public static bool CompassFailed;
        public static bool GyrometerFailed;
        public static bool InclinometerFailed;
        public static bool LightSensorFailed;
        public static bool MagnetometerFailed;
        public static bool OrientationAbsoluteFailed;
        public static bool OrientationGeomagneticFailed;
        public static bool OrientationRelativeFailed;
        public static bool PedometerFailed;
        public static bool ProximitySensorFailed;
        public static bool SimpleOrientationSensorFailed;
        public static bool OtherSensorFailed;

        private static CoreDispatcher _cd = Window.Current.CoreWindow.Dispatcher;

        public static async Task<bool> GetDefault()
        {
            AccelerometerStandardList = new List<Accelerometer>();
            AccelerometerStandardDeviceInfo = new List<DeviceInformation>();
            AccelerometerLinearList = new List<Accelerometer>();
            AccelerometerLinearDeviceInfo = new List<DeviceInformation>();
            AccelerometerGravityList = new List<Accelerometer>();
            AccelerometerGravityDeviceInfo = new List<DeviceInformation>();
            ActivitySensorList = new List<ActivitySensor>();
            ActivitySensorDeviceInfo = new List<DeviceInformation>();
            BarometerList = new List<Barometer>();
            BarometerDeviceInfo = new List<DeviceInformation>();
            CompassList = new List<Compass>();
            CompassDeviceInfo = new List<DeviceInformation>();
            GyrometerList = new List<Gyrometer>();
            GyrometerDeviceInfo = new List<DeviceInformation>();
            InclinometerList = new List<Inclinometer>();
            InclinometerDeviceInfo = new List<DeviceInformation>();
            LightSensorList = new List<LightSensor>();
            LightSensorDeviceInfo = new List<DeviceInformation>();
            MagnetometerList = new List<Magnetometer>();
            MagnetometerDeviceInfo = new List<DeviceInformation>();
            OrientationAbsoluteList = new List<OrientationSensor>();
            OrientationAbsoluteDeviceInfo = new List<DeviceInformation>();
            OrientationGeomagneticList = new List<OrientationSensor>();
            OrientationGeomagneticDeviceInfo = new List<DeviceInformation>();
            OrientationRelativeList = new List<OrientationSensor>();
            OrientationRelativeDeviceInfo = new List<DeviceInformation>();
            PedometerList = new List<Pedometer>();
            PedometerDeviceInfo = new List<DeviceInformation>();
            ProximitySensorList = new List<ProximitySensor>();
            ProximitySensorDeviceInfo = new List<DeviceInformation>();
            SimpleOrientationSensorList = new List<SimpleOrientationSensor>();
            SimpleOrientationSensorDeviceInfo = new List<DeviceInformation>();

            NumFailedEnumerations = 0;

            AccelerometerStandardFailed = false;
            AccelerometerLinearFailed = false;
            AccelerometerGravityFailed = false;
            ActivitySensorFailed = false;
            AltimeterFailed = false;
            BarometerFailed = false;
            CompassFailed = false;
            GyrometerFailed = false;
            InclinometerFailed = false;
            LightSensorFailed = false;
            MagnetometerFailed = false;
            OrientationAbsoluteFailed = false;
            OrientationGeomagneticFailed = false;
            OrientationRelativeFailed = false;
            PedometerFailed = false;
            ProximitySensorFailed = false;
            SimpleOrientationSensorFailed = false;
            OtherSensorFailed = false;

            DeviceInformationCollection deviceInfoCollection;

            // Enumerate the Sensor class
            try
            {
                Guid sensorGuid = new Guid("{5175d334-c371-4806-b3ba-71fd53c9258d}");
                string sensorDeviceSelector = CustomDevice.GetDeviceSelector(sensorGuid);
                SensorClassDevice = await DeviceInformation.FindAllAsync(sensorDeviceSelector);
                NumFailedEnumerations = SensorClassDevice.Count;
            }
            catch
            {
                OtherSensorFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Accelerometer.GetDeviceSelector(AccelerometerReadingType.Standard), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Accelerometer accelerometer = await Accelerometer.FromIdAsync(deviceInfo.Id);
                    AccelerometerStandardList.Add(accelerometer);
                    AccelerometerStandardDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                AccelerometerStandardFailed = true;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Accelerometer.GetDeviceSelector(AccelerometerReadingType.Linear), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Accelerometer accelerometer = await Accelerometer.FromIdAsync(deviceInfo.Id);
                    AccelerometerLinearList.Add(accelerometer);
                    AccelerometerLinearDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                AccelerometerLinearFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Accelerometer.GetDeviceSelector(AccelerometerReadingType.Gravity), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Accelerometer accelerometer = await Accelerometer.FromIdAsync(deviceInfo.Id);
                    AccelerometerGravityList.Add(accelerometer);
                    AccelerometerGravityDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                AccelerometerGravityFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(ActivitySensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    ActivitySensor activitySensor = await ActivitySensor.FromIdAsync(deviceInfo.Id);
                    ActivitySensorList.Add(activitySensor);
                    ActivitySensorDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                ActivitySensorFailed = false;
            }
            try
            {
                // No GetDeviceSelector() for altimeter
                Guid altimeterGuid = new Guid("{0E903829-FF8A-4A93-97DF-3DCBDE402288}");
                string altimeterDeviceSelector = CustomDevice.GetDeviceSelector(altimeterGuid);
                deviceInfoCollection = await DeviceInformation.FindAllAsync(altimeterDeviceSelector, Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Altimeter = Altimeter.GetDefault();
                    AltimeterDeviceInfo = deviceInfo;
                }
            }
            catch
            {
                AltimeterFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Barometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Barometer barometer = await Barometer.FromIdAsync(deviceInfo.Id);
                    BarometerList.Add(barometer);
                    BarometerDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                BarometerFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Compass.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Compass compass = await Compass.FromIdAsync(deviceInfo.Id);
                    CompassList.Add(compass);
                    CompassDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                CompassFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Gyrometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Gyrometer gyrometer = await Gyrometer.FromIdAsync(deviceInfo.Id);
                    GyrometerList.Add(gyrometer);
                    GyrometerDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                GyrometerFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Inclinometer.GetDeviceSelector(SensorReadingType.Absolute), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Inclinometer inclinometer = await Inclinometer.FromIdAsync(deviceInfo.Id);
                    InclinometerList.Add(inclinometer);
                    InclinometerDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                InclinometerFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(LightSensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    LightSensor lightSensor = await LightSensor.FromIdAsync(deviceInfo.Id);
                    LightSensorList.Add(lightSensor);
                    LightSensorDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                LightSensorFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Magnetometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Magnetometer magnetometer = await Magnetometer.FromIdAsync(deviceInfo.Id);
                    MagnetometerList.Add(magnetometer);
                    MagnetometerDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                MagnetometerFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(OrientationSensor.GetDeviceSelector(SensorReadingType.Absolute), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    OrientationSensor orientationSensor = await OrientationSensor.FromIdAsync(deviceInfo.Id);
                    OrientationAbsoluteList.Add(orientationSensor);
                    OrientationAbsoluteDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                OrientationAbsoluteFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(OrientationSensor.GetDeviceSelector(SensorReadingType.Absolute, SensorOptimizationGoal.PowerEfficiency), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    OrientationSensor orientationSensor = await OrientationSensor.FromIdAsync(deviceInfo.Id);
                    OrientationGeomagneticList.Add(orientationSensor);
                    OrientationGeomagneticDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                OrientationGeomagneticFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(OrientationSensor.GetDeviceSelector(SensorReadingType.Relative), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    OrientationSensor orientationSensor = await OrientationSensor.FromIdAsync(deviceInfo.Id);
                    OrientationRelativeList.Add(orientationSensor);
                    OrientationRelativeDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                OrientationRelativeFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Pedometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Pedometer pedometer = await Pedometer.FromIdAsync(deviceInfo.Id);
                    PedometerList.Add(pedometer);
                    PedometerDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                PedometerFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(ProximitySensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    ProximitySensor proximitySensor = ProximitySensor.FromId(deviceInfo.Id);
                    ProximitySensorList.Add(proximitySensor);
                    ProximitySensorDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                ProximitySensorFailed = false;
            }
            try
            {
                deviceInfoCollection = await DeviceInformation.FindAllAsync(SimpleOrientationSensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    SimpleOrientationSensor simpleOrientationSensor = await SimpleOrientationSensor.FromIdAsync(deviceInfo.Id);
                    SimpleOrientationSensorList.Add(simpleOrientationSensor);
                    SimpleOrientationSensorDeviceInfo.Add(deviceInfo);
                }
            }
            catch
            {
                SimpleOrientationSensorFailed = false;
            }
            return true;
        }

        public static void EnableSensor(int sensorType, int index, int totalIndex)
        {
            try
            {
                switch (sensorType)
                {
                    case ACCELEROMETER: EnableAccelerometer(index, totalIndex); break;
                    case ACCELEROMETERLINEAR: EnableAccelerometerLinear(index, totalIndex); break;
                    case ACCELEROMETERGRAVITY: EnableAccelerometerGravity(index, totalIndex); break;
                    case ACTIVITYSENSOR: EnableActivitySensor(index, totalIndex); break;
                    case ALTIMETER: EnableAltimeter(totalIndex); break;
                    case BAROMETER: EnableBarometer(index, totalIndex); break;
                    case COMPASS: EnableCompass(index, totalIndex); break;
                    case GYROMETER: EnableGyrometer(index, totalIndex); break;
                    case INCLINOMETER: EnableInclinometer(index, totalIndex); break;
                    case LIGHTSENSOR: EnableLightSensor(index, totalIndex); break;
                    case MAGNETOMETER: EnableMagnetometer(index, totalIndex); break;
                    case ORIENTATIONSENSOR: EnableOrientationSensor(index, totalIndex); break;
                    case ORIENTATIONRELATIVE: EnableRelativeOrientationSensor(index, totalIndex); break;
                    case ORIENTATIONGEOMAGNETIC: EnableOrientationGeomagnetic(index, totalIndex); break;
                    case PEDOMETER: EnablePedometer(index, totalIndex); break;
                    case PROXIMITYSENSOR: EnableProximitySensor(index, totalIndex); break;
                    case SIMPLEORIENTATIONSENSOR: EnableSimpleOrientationSensor(index, totalIndex); break;
                }
            }
            catch { }
        }

        public static void DisableSensor(int sensorType, int index)
        {
            try
            {
                switch (sensorType)
                {
                    case ACCELEROMETER: DisableAccelerometer(index); break;
                    case ACCELEROMETERLINEAR: DisableAccelerometerLinear(index); break;
                    case ACCELEROMETERGRAVITY: DisableAccelerometerGravity(index); break;
                    case ACTIVITYSENSOR: DisableActivitySensor(index); break;
                    case ALTIMETER: DisableAltimeter(); break;
                    case BAROMETER: DisableBarometer(index); break;
                    case COMPASS: DisableCompass(index); break;
                    case GYROMETER: DisableGyrometer(index); break;
                    case INCLINOMETER: DisableInclinometer(index); break;
                    case LIGHTSENSOR: DisableLightSensor(index); break;
                    case MAGNETOMETER: DisableMagnetometer(index); break;
                    case ORIENTATIONSENSOR: DisableOrientationSensor(index); break;
                    case ORIENTATIONRELATIVE: DisableRelativeOrientationSensor(index); break;
                    case ORIENTATIONGEOMAGNETIC: DisableOrientationGeomagnetic(index); break;
                    case PEDOMETER: DisablePedometer(index); break;
                    case PROXIMITYSENSOR: DisableProximitySensor(index); break;
                    case SIMPLEORIENTATIONSENSOR: DisableSimpleOrientationSensor(index); break;
                }
            }
            catch { }
        }

        private static void EnableAccelerometer(int index, int totalIndex)
        {
            if (AccelerometerStandardList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                uint reportLatency = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;
                try
                {
                    deviceId = AccelerometerStandardList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = AccelerometerStandardList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = AccelerometerStandardList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    reportLatency = AccelerometerStandardList[index].ReportLatency;
                }
                catch { };
                try
                {
                    category = Constants.SensorCategories[AccelerometerStandardDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(AccelerometerStandardDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    manufacturer = AccelerometerStandardDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = AccelerometerStandardDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    persistentUniqueId = AccelerometerStandardDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    isPrimary = AccelerometerStandardDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = AccelerometerStandardDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = AccelerometerStandardDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, reportLatency,
                                                   category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                AccelerometerStandardList[index].ReadingChanged += Accelerometer_ReadingChanged;
            }
        }

        private static void DisableAccelerometer(int index)
        {
            if (AccelerometerStandardList[index] != null)
            {
                AccelerometerStandardList[index].ReadingChanged -= Accelerometer_ReadingChanged;
            }
        }

        private async static void Accelerometer_ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == ACCELEROMETER)
                {
                    AccelerometerReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AccelerationX, reading.AccelerationY, reading.AccelerationZ }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableAccelerometerLinear(int index, int totalIndex)
        {
            if (AccelerometerLinearList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                uint reportLatency = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = AccelerometerLinearList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = AccelerometerLinearList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = AccelerometerLinearList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    reportLatency = AccelerometerLinearList[index].ReportLatency;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[AccelerometerLinearDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = AccelerometerLinearDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = AccelerometerLinearDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = AccelerometerLinearDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(AccelerometerLinearDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = AccelerometerLinearDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = AccelerometerLinearDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = AccelerometerLinearDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }
                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, reportLatency, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                AccelerometerLinearList[index].ReadingChanged += AccelerometerLinear_ReadingChanged;
            }
        }

        private static void DisableAccelerometerLinear(int index)
        {
            if (AccelerometerLinearList[index] != null)
            {
                AccelerometerLinearList[index].ReadingChanged -= AccelerometerLinear_ReadingChanged;
            }
        }

        private async static void AccelerometerLinear_ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == ACCELEROMETERLINEAR)
                {
                    AccelerometerReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AccelerationX, reading.AccelerationY, reading.AccelerationZ }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableAccelerometerGravity(int index, int totalIndex)
        {
            if (AccelerometerGravityList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                uint reportLatency = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = AccelerometerGravityList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = AccelerometerGravityList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = AccelerometerGravityList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    reportLatency = AccelerometerGravityList[index].ReportLatency;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[AccelerometerGravityDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = AccelerometerGravityDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = AccelerometerGravityDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = AccelerometerGravityDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(AccelerometerGravityDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = AccelerometerGravityDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = AccelerometerGravityDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = AccelerometerGravityDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, reportLatency, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                AccelerometerGravityList[index].ReadingChanged += AccelerometerGravity_ReadingChanged;
            }
        }

        private static void DisableAccelerometerGravity(int index)
        {
            if (AccelerometerGravityList[index] != null)
            {
                AccelerometerGravityList[index].ReadingChanged -= AccelerometerGravity_ReadingChanged;
            }
        }

        private async static void AccelerometerGravity_ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == ACCELEROMETERGRAVITY)
                {
                    AccelerometerReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AccelerationX, reading.AccelerationY, reading.AccelerationZ }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableCompass(int index, int totalIndex)
        {
            if (CompassList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = CompassList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = CompassList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = CompassList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[CompassDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = CompassDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = CompassDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = CompassDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(CompassDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = CompassDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = CompassDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = CompassDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                CompassList[index].ReadingChanged += Compass_ReadingChanged;
            }
        }

        private static void DisableCompass(int index)
        {
            if (CompassList[index] != null)
            {
                CompassList[index].ReadingChanged -= Compass_ReadingChanged;
            }
        }

        private async static void Compass_ReadingChanged(object sender, CompassReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == COMPASS)
                {
                    CompassReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { Convert.ToDouble(reading.HeadingMagneticNorth),
                                                                                                       Convert.ToDouble(reading.HeadingTrueNorth),
                                                                                                       (int)reading.HeadingAccuracy }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableGyrometer(int index, int totalIndex)
        {
            if (GyrometerList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = GyrometerList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = GyrometerList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = GyrometerList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[GyrometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = GyrometerDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = GyrometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = GyrometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(GyrometerDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = GyrometerDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = GyrometerDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = GyrometerDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                GyrometerList[index].ReadingChanged += Gyrometer_ReadingChanged;
            }
        }

        private static void DisableGyrometer(int index)
        {
            if (GyrometerList[index] != null)
            {
                GyrometerList[index].ReadingChanged -= Gyrometer_ReadingChanged;
            }
        }

        private async static void Gyrometer_ReadingChanged(object sender, GyrometerReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == GYROMETER)
                {
                    GyrometerReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AngularVelocityX, reading.AngularVelocityY, reading.AngularVelocityZ }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableInclinometer(int index, int totalIndex)
        {
            if (InclinometerList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = InclinometerList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = InclinometerList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = InclinometerList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[InclinometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = InclinometerDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = InclinometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = InclinometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(InclinometerDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = InclinometerDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = InclinometerDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = InclinometerDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                InclinometerList[index].ReadingChanged += Inclinometer_ReadingChanged;
            }
        }

        private static void DisableInclinometer(int index)
        {
            if (InclinometerList[index] != null)
            {
                InclinometerList[index].ReadingChanged -= Inclinometer_ReadingChanged;
            }
        }

        private async static void Inclinometer_ReadingChanged(object sender, InclinometerReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == INCLINOMETER)
                {
                    InclinometerReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.PitchDegrees,
                                                                                                       reading.RollDegrees,
                                                                                                       reading.YawDegrees,
                                                                                                       (int)reading.YawAccuracy }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableLightSensor(int index, int totalIndex)
        {
            if (LightSensorList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = LightSensorList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = LightSensorList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = LightSensorList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[LightSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = LightSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = LightSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = LightSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(LightSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = LightSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = LightSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = LightSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                LightSensorList[index].ReadingChanged += LightSensor_ReadingChanged;
            }
        }

        private static void DisableLightSensor(int index)
        {
            if (LightSensorList[index] != null)
            {
                LightSensorList[index].ReadingChanged -= LightSensor_ReadingChanged;
            }
        }

        private async static void LightSensor_ReadingChanged(object sender, LightSensorReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == LIGHTSENSOR)
                {
                    LightSensorReading reading = e.Reading;
                    Scenario1View.Scenario1.LogDataLightSensor(reading);

                    // for color sensor
                    object x, y;
                    reading.Properties.TryGetValue("{C458F8A7-4AE8-4777-9607-2E9BDD65110A} 62", out x);
                    reading.Properties.TryGetValue("{C458F8A7-4AE8-4777-9607-2E9BDD65110A} 63", out y);

                    double chromaticity_x = -1, chromaticity_y = -1;
                    try
                    {
                        chromaticity_x = double.Parse(x.ToString());
                        chromaticity_y = double.Parse(y.ToString());
                    }
                    catch { }

                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.IlluminanceInLux, chromaticity_x, chromaticity_y }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableOrientationSensor(int index, int totalIndex)
        {
            if (OrientationAbsoluteList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = OrientationAbsoluteList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = OrientationAbsoluteList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = OrientationAbsoluteList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[OrientationAbsoluteDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = OrientationAbsoluteDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = OrientationAbsoluteDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = OrientationAbsoluteDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(OrientationAbsoluteDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = OrientationAbsoluteDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = OrientationAbsoluteDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = OrientationAbsoluteDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                OrientationAbsoluteList[index].ReadingChanged += OrientationSensor_ReadingChanged;
            }
        }

        private static void DisableOrientationSensor(int index)
        {
            if (OrientationAbsoluteList[index] != null)
            {
                OrientationAbsoluteList[index].ReadingChanged -= OrientationSensor_ReadingChanged;
            }
        }

        private async static void OrientationSensor_ReadingChanged(object sender, OrientationSensorReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == ORIENTATIONSENSOR)
                {
                    OrientationSensorReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.Quaternion.X,
                                                                                                       reading.Quaternion.Y,
                                                                                                       reading.Quaternion.Z,
                                                                                                       reading.Quaternion.W,
                                                                                                       reading.RotationMatrix.M11,
                                                                                                       reading.RotationMatrix.M12,
                                                                                                       reading.RotationMatrix.M13,
                                                                                                       reading.RotationMatrix.M21,
                                                                                                       reading.RotationMatrix.M22,
                                                                                                       reading.RotationMatrix.M23,
                                                                                                       reading.RotationMatrix.M31,
                                                                                                       reading.RotationMatrix.M32,
                                                                                                       reading.RotationMatrix.M33 }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableRelativeOrientationSensor(int index, int totalIndex)
        {
            if (OrientationRelativeList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = OrientationRelativeList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = OrientationRelativeList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = OrientationRelativeList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[OrientationRelativeDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = OrientationRelativeDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = OrientationRelativeDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = OrientationRelativeDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(OrientationRelativeDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = OrientationRelativeDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = OrientationRelativeDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = OrientationRelativeDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                OrientationRelativeList[totalIndex].ReadingChanged += RelativeOrientationSensor_ReadingChanged;
            }
        }

        private static void DisableRelativeOrientationSensor(int index)
        {
            if (OrientationRelativeList[index] != null)
            {
                OrientationRelativeList[index].ReadingChanged -= RelativeOrientationSensor_ReadingChanged;
            }
        }

        private async static void RelativeOrientationSensor_ReadingChanged(object sender, OrientationSensorReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == ORIENTATIONRELATIVE)
                {
                    OrientationSensorReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.Quaternion.X,
                                                                                                       reading.Quaternion.Y,
                                                                                                       reading.Quaternion.Z,
                                                                                                       reading.Quaternion.W,
                                                                                                       reading.RotationMatrix.M11,
                                                                                                       reading.RotationMatrix.M12,
                                                                                                       reading.RotationMatrix.M13,
                                                                                                       reading.RotationMatrix.M21,
                                                                                                       reading.RotationMatrix.M22,
                                                                                                       reading.RotationMatrix.M23,
                                                                                                       reading.RotationMatrix.M31,
                                                                                                       reading.RotationMatrix.M32,
                                                                                                       reading.RotationMatrix.M33 }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableOrientationGeomagnetic(int index, int totalIndex)
        {
            if (OrientationGeomagneticList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = OrientationGeomagneticList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = OrientationGeomagneticList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = OrientationGeomagneticList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[OrientationGeomagneticDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = OrientationGeomagneticDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = OrientationGeomagneticDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = OrientationGeomagneticDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(OrientationGeomagneticDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = OrientationGeomagneticDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = OrientationGeomagneticDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = OrientationGeomagneticDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                OrientationGeomagneticList[totalIndex].ReadingChanged += OrientationGeomagnetic_ReadingChanged;
            }
        }

        private static void DisableOrientationGeomagnetic(int index)
        {
            if (OrientationGeomagneticList[index] != null)
            {
                OrientationGeomagneticList[index].ReadingChanged -= OrientationGeomagnetic_ReadingChanged;
            }
        }

        private async static void OrientationGeomagnetic_ReadingChanged(object sender, OrientationSensorReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == ORIENTATIONRELATIVE)
                {
                    OrientationSensorReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.Quaternion.X,
                                                                                                       reading.Quaternion.Y,
                                                                                                       reading.Quaternion.Z,
                                                                                                       reading.Quaternion.W,
                                                                                                       reading.RotationMatrix.M11,
                                                                                                       reading.RotationMatrix.M12,
                                                                                                       reading.RotationMatrix.M13,
                                                                                                       reading.RotationMatrix.M21,
                                                                                                       reading.RotationMatrix.M22,
                                                                                                       reading.RotationMatrix.M23,
                                                                                                       reading.RotationMatrix.M31,
                                                                                                       reading.RotationMatrix.M32,
                                                                                                       reading.RotationMatrix.M33 }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableActivitySensor(int index, int totalIndex)
        {
            if (ActivitySensorList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = ActivitySensorList[index].DeviceId;
                }
                catch { }
                try
                {
                    minimumReportInterval = ActivitySensorList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[ActivitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = ActivitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = ActivitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = ActivitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(ActivitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = ActivitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = ActivitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = ActivitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, 0, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);

                // subscribe to all supported activities
                foreach (ActivityType activityType in ActivitySensorList[index].SupportedActivities)
                {
                    ActivitySensorList[index].SubscribedActivities.Add(activityType);
                }

                ActivitySensorList[index].ReadingChanged += ActivitySensor_ReadingChanged;
            }
        }

        private static void DisableActivitySensor(int index)
        {
            if (ActivitySensorList[index] != null)
            {
                ActivitySensorList[index].ReadingChanged -= ActivitySensor_ReadingChanged;
            }
        }

        private async static void ActivitySensor_ReadingChanged(object sender, ActivitySensorReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == ACCELEROMETER)
                {
                    ActivitySensorReading reading = e.Reading;
                    double[] activitySensorReadingConfidence = new double[] { ACTIVITYNOTSUPPORTED, ACTIVITYNOTSUPPORTED, ACTIVITYNOTSUPPORTED, ACTIVITYNOTSUPPORTED, ACTIVITYNOTSUPPORTED, ACTIVITYNOTSUPPORTED, ACTIVITYNOTSUPPORTED, ACTIVITYNOTSUPPORTED };
                    foreach (ActivityType activityType in ActivitySensorList[currentId].SupportedActivities)
                    {
                        activitySensorReadingConfidence[Convert.ToInt32(activityType)] = ACTIVITYNONE;
                    }
                    activitySensorReadingConfidence[Convert.ToInt32(reading.Activity)] = Convert.ToDouble(reading.Confidence);

                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, activitySensorReadingConfidence))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableAltimeter(int totalIndex)
        {
            if (Altimeter != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = Altimeter.DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = Altimeter.ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = Altimeter.MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[AltimeterDeviceInfo.Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = AltimeterDeviceInfo.Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = AltimeterDeviceInfo.Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = AltimeterDeviceInfo.Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(AltimeterDeviceInfo.Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = AltimeterDeviceInfo.Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = AltimeterDeviceInfo.Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = AltimeterDeviceInfo.Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                Altimeter.ReadingChanged += Altimeter_ReadingChanged;
            }
        }

        private static void DisableAltimeter()
        {
            if (Altimeter != null)
            {
                Altimeter.ReadingChanged -= Altimeter_ReadingChanged;
            }
        }

        private async static void Altimeter_ReadingChanged(object sender, AltimeterReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == ALTIMETER)
                {
                    AltimeterReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AltitudeChangeInMeters }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableBarometer(int index, int totalIndex)
        {
            if (BarometerList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = BarometerList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = BarometerList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = BarometerList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[BarometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = BarometerDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = BarometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = BarometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(BarometerDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = BarometerDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = BarometerDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = BarometerDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                BarometerList[index].ReadingChanged += Barometer_ReadingChanged;
            }
        }

        private static void DisableBarometer(int index)
        {
            if (BarometerList[index] != null)
            {
                BarometerList[index].ReadingChanged -= Barometer_ReadingChanged;
            }
        }

        private async static void Barometer_ReadingChanged(object sender, BarometerReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == BAROMETER)
                {
                    BarometerReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.StationPressureInHectopascals }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableMagnetometer(int index, int totalIndex)
        {
            if (MagnetometerList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = MagnetometerList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = MagnetometerList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = MagnetometerList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[MagnetometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = MagnetometerDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = MagnetometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = MagnetometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(MagnetometerDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = MagnetometerDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = MagnetometerDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = MagnetometerDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                MagnetometerList[index].ReadingChanged += Magnetometer_ReadingChanged;
            }
        }

        private static void DisableMagnetometer(int index)
        {
            if (MagnetometerList[index] != null)
            {
                MagnetometerList[index].ReadingChanged -= Magnetometer_ReadingChanged;
            }
        }

        private async static void Magnetometer_ReadingChanged(object sender, MagnetometerReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == MAGNETOMETER)
                {
                    MagnetometerReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.MagneticFieldX, reading.MagneticFieldY, reading.MagneticFieldZ }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnablePedometer(int index, int totalIndex)
        {
            if (PedometerList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = PedometerList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = PedometerList[index].ReportInterval;
                }
                catch { }
                try
                {
                    minimumReportInterval = PedometerList[index].MinimumReportInterval;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[PedometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = PedometerDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = PedometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = PedometerDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(PedometerDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = PedometerDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = PedometerDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = PedometerDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                PedometerList[index].ReadingChanged += Pedometer_ReadingChanged;
            }
        }

        private static void DisablePedometer(int index)
        {
            if (PedometerList[index] != null)
            {
                PedometerList[index].ReadingChanged -= Pedometer_ReadingChanged;
            }
        }

        private async static void Pedometer_ReadingChanged(object sender, PedometerReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == PEDOMETER)
                {
                    PedometerReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.CumulativeSteps, reading.CumulativeStepsDuration.Seconds, Convert.ToDouble(reading.StepKind) }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableProximitySensor(int index, int totalIndex)
        {
            if (ProximitySensorList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = ProximitySensorList[index].DeviceId;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[ProximitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = ProximitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = ProximitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = ProximitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(ProximitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = ProximitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = ProximitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = ProximitySensorDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, 0, 0, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                ProximitySensorList[totalIndex].ReadingChanged += ProximitySensor_ReadingChanged;
            }
        }

        private static void DisableProximitySensor(int index)
        {
            if (ProximitySensorList[index] != null)
            {
                ProximitySensorList[index].ReadingChanged -= ProximitySensor_ReadingChanged;
            }
        }

        private async static void ProximitySensor_ReadingChanged(object sender, ProximitySensorReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == PROXIMITYSENSOR)
                {
                    ProximitySensorReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { Convert.ToDouble(reading.IsDetected), Convert.ToDouble(reading.DistanceInMillimeters) }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (currentId < sensorData.Count)
                            {
                                sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static void EnableSimpleOrientationSensor(int index, int totalIndex)
        {
            if (SimpleOrientationSensorList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                string category = string.Empty;
                string persistentUniqueId = string.Empty;
                string manufacturer = string.Empty;
                string model = string.Empty;
                string connectionType = string.Empty;
                string isPrimary = string.Empty;
                string vendorDefinedSubType = string.Empty;
                string state = string.Empty;

                try
                {
                    deviceId = SimpleOrientationSensorList[index].DeviceId;
                }
                catch { }
                try
                {
                    category = Constants.SensorCategories[SimpleOrientationSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Category"]].ToString()];
                }
                catch { }
                try
                {
                    persistentUniqueId = SimpleOrientationSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
                }
                catch { }
                try
                {
                    manufacturer = SimpleOrientationSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
                }
                catch { }
                try
                {
                    model = SimpleOrientationSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_Model"]].ToString();
                }
                catch { }
                try
                {
                    connectionType = Constants.SensorConnectionTypes[int.Parse(SimpleOrientationSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                }
                catch { }
                try
                {
                    isPrimary = SimpleOrientationSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
                }
                catch { }
                try
                {
                    vendorDefinedSubType = SimpleOrientationSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
                }
                catch { }
                try
                {
                    state = SimpleOrientationSensorDeviceInfo[index].Properties[Constants.Properties["Sensor_State"]].ToString();
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, 0, 0, 0, category, persistentUniqueId, manufacturer, model, connectionType, isPrimary, vendorDefinedSubType, state);
                SimpleOrientationSensorList[index].OrientationChanged += SimpleOrientationSensor_OrientationChanged;
            }
        }

        private static void DisableSimpleOrientationSensor(int index)
        {
            if (SimpleOrientationSensorList[index] != null)
            {
                SimpleOrientationSensorList[index].OrientationChanged -= SimpleOrientationSensor_OrientationChanged;
            }
        }

        private async static void SimpleOrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == SIMPLEORIENTATIONSENSOR)
                {
                    SimpleOrientation reading = e.Orientation;
                    if (sensorData[currentId].AddReading(DateTime.UtcNow, new double[] { Convert.ToDouble(reading) }))
                    {
                        await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            sensorDisplay[currentId].UpdateText(sensorData[currentId]);
                        });
                    }
                }
            }
            catch { }
        }
    }
}
 