// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Custom;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Devices.Sensors.Custom;
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
        public const int CUSTOMSENSOR = 22;

        public const int ACTIVITYNONE = 2;
        public const int ACTIVITYNOTSUPPORTED = 3;

        public static int currentId = -1;
        public static List<SensorData> sensorData;
        public static List<SensorDisplay> sensorDisplay;
        public static DeviceInformationCollection SensorClassDevice;
        public static int NumFailedEnumerations;

        public static List<Accelerometer> AccelerometerStandardList;
        public static List<DeviceInformation> AccelerometerStandardDeviceInfo;
        public static List<string[]> AccelerometerStandardPLD;
        public static List<Accelerometer> AccelerometerLinearList;
        public static List<DeviceInformation> AccelerometerLinearDeviceInfo;
        public static List<string[]> AccelerometerLinearPLD;
        public static List<Accelerometer> AccelerometerGravityList;
        public static List<DeviceInformation> AccelerometerGravityDeviceInfo;
        public static List<string[]> AccelerometerGravityPLD;
        public static List<ActivitySensor> ActivitySensorList;
        public static List<DeviceInformation> ActivitySensorDeviceInfo;
        public static List<string[]> ActivitySensorPLD;
        public static Altimeter Altimeter;
        public static DeviceInformation AltimeterDeviceInfo;
        public static string[] AltimeterPLD;
        public static List<Barometer> BarometerList;
        public static List<DeviceInformation> BarometerDeviceInfo;
        public static List<string[]> BarometerPLD;
        public static List<Compass> CompassList;
        public static List<DeviceInformation> CompassDeviceInfo;
        public static List<string[]> CompassPLD;
        public static List<CustomSensor> CustomSensorList;
        public static List<DeviceInformation> CustomSensorDeviceInfo;
        public static List<string[]> CustomSensorPLD;
        public static List<Gyrometer> GyrometerList;
        public static List<DeviceInformation> GyrometerDeviceInfo;
        public static List<string[]> GyrometerPLD;
        public static List<Inclinometer> InclinometerList;
        public static List<DeviceInformation> InclinometerDeviceInfo;
        public static List<string[]> InclinometerPLD;
        public static List<LightSensor> LightSensorList;
        public static List<DeviceInformation> LightSensorDeviceInfo;
        public static List<string[]> LightSensorPLD;
        public static List<Magnetometer> MagnetometerList;
        public static List<DeviceInformation> MagnetometerDeviceInfo;
        public static List<string[]> MagnetometerPLD;
        public static List<OrientationSensor> OrientationAbsoluteList;
        public static List<DeviceInformation> OrientationAbsoluteDeviceInfo;
        public static List<string[]> OrientationAbsolutePLD;
        public static List<OrientationSensor> OrientationGeomagneticList;
        public static List<DeviceInformation> OrientationGeomagneticDeviceInfo;
        public static List<string[]> OrientationGeomagneticPLD;
        public static List<OrientationSensor> OrientationRelativeList;
        public static List<DeviceInformation> OrientationRelativeDeviceInfo;
        public static List<string[]> OrientationRelativePLD;
        public static List<Pedometer> PedometerList;
        public static List<DeviceInformation> PedometerDeviceInfo;
        public static List<string[]> PedometerPLD;
        public static List<ProximitySensor> ProximitySensorList;
        public static List<DeviceInformation> ProximitySensorDeviceInfo;
        public static List<string[]> ProximitySensorPLD;
        public static List<SimpleOrientationSensor> SimpleOrientationSensorList;
        public static List<DeviceInformation> SimpleOrientationSensorDeviceInfo;
        public static List<string[]> SimpleOrientationSensorPLD;

        public static bool AccelerometerStandardFailed;
        public static bool AccelerometerLinearFailed;
        public static bool AccelerometerGravityFailed;
        public static bool ActivitySensorFailed;
        public static bool AltimeterFailed;
        public static bool BarometerFailed;
        public static bool CompassFailed;
        public static bool CustomSensorFailed;
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

        public static string[] GetProperties(DeviceInformation deviceInfo)
        {
            string[] properties = new string[8];
            try
            {
                properties[0] = Constants.SensorCategories[deviceInfo.Properties[Constants.Properties["Sensor_Category"]].ToString()];
            }
            catch { }
            try
            {
                properties[1] = deviceInfo.Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString();
            }
            catch { }
            try
            {
                properties[2] = deviceInfo.Properties[Constants.Properties["Sensor_Manufacturer"]].ToString();
            }
            catch { }
            try
            {
                properties[3] = deviceInfo.Properties[Constants.Properties["Sensor_Model"]].ToString();
            }
            catch { }
            try
            {
                properties[4] = Constants.SensorConnectionTypes[int.Parse(deviceInfo.Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
            }
            catch { }
            try
            {
                properties[5] = deviceInfo.Properties[Constants.Properties["Sensor_IsPrimary"]].ToString();
            }
            catch { }
            try
            {
                properties[6] = deviceInfo.Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
            }
            catch { }
            try
            {
                properties[7] = deviceInfo.Properties[Constants.Properties["Sensor_State"]].ToString();
            }
            catch { }

            return properties;
        }

        public static async Task<string[]> GetPLDInformation(string deviceInstanceId)
        {
            string[] pld = new string[15];
            try
            {
                DeviceInformation pldPanelId = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelId"] }, DeviceInformationKind.Device);
                pld[0] = pldPanelId.Properties[Constants.PLD["Device_PanelId"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelGroup = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelGroup"] }, DeviceInformationKind.Device);
                pld[1] = pldPanelGroup.Properties[Constants.PLD["Device_PanelGroup"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelSide = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelSide"] }, DeviceInformationKind.Device);
                pld[2] = pldPanelSide.Properties[Constants.PLD["Device_PanelSide"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelWidth = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelWidth"] }, DeviceInformationKind.Device);
                pld[3] = pldPanelWidth.Properties[Constants.PLD["Device_PanelWidth"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelHeight = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelHeight"] }, DeviceInformationKind.Device);
                pld[4] = pldPanelHeight.Properties[Constants.PLD["Device_PanelHeight"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelLength = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelLength"] }, DeviceInformationKind.Device);
                pld[5] = pldPanelLength.Properties[Constants.PLD["Device_PanelLength"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelPositionX = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelPositionX"] }, DeviceInformationKind.Device);
                pld[6] = pldPanelPositionX.Properties[Constants.PLD["Device_PanelPositionX"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelPositionY = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelPositionY"] }, DeviceInformationKind.Device);
                pld[7] = pldPanelPositionY.Properties[Constants.PLD["Device_PanelPositionY"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelPositionZ = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelPositionZ"] }, DeviceInformationKind.Device);
                pld[8] = pldPanelPositionZ.Properties[Constants.PLD["Device_PanelPositionZ"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelRotationX = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelRotationX"] }, DeviceInformationKind.Device);
                pld[9] = pldPanelRotationX.Properties[Constants.PLD["Device_PanelRotationX"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelRotationY = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelPosiRotationY"] }, DeviceInformationKind.Device);
                pld[10] = pldPanelRotationY.Properties[Constants.PLD["Device_PanelRotationY"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelRotationZ = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelRotationZ"] }, DeviceInformationKind.Device);
                pld[11] = pldPanelRotationZ.Properties[Constants.PLD["Device_PanelRotationZ"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelColor = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelColor"] }, DeviceInformationKind.Device);
                pld[12] = pldPanelColor.Properties[Constants.PLD["Device_PanelColor"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelShape = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelShape"] }, DeviceInformationKind.Device);
                pld[13] = pldPanelShape.Properties[Constants.PLD["Device_PanelShape"]].ToString();
            }
            catch { }
            try
            {
                DeviceInformation pldPanelVisible = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelVisible"] }, DeviceInformationKind.Device);
                pld[14] = pldPanelVisible.Properties[Constants.PLD["Device_PanelVisible"]].ToString();
            }
            catch { }

            return pld;
        }

        public static async Task<bool> GetDefault(bool getPLD)
        {
            AccelerometerStandardList = new List<Accelerometer>();
            AccelerometerStandardDeviceInfo = new List<DeviceInformation>();
            AccelerometerStandardPLD = new List<string[]>();
            AccelerometerLinearList = new List<Accelerometer>();
            AccelerometerLinearDeviceInfo = new List<DeviceInformation>();
            AccelerometerLinearPLD = new List<string[]>();
            AccelerometerGravityList = new List<Accelerometer>();
            AccelerometerGravityDeviceInfo = new List<DeviceInformation>();
            AccelerometerGravityPLD = new List<string[]>();
            ActivitySensorList = new List<ActivitySensor>();
            ActivitySensorDeviceInfo = new List<DeviceInformation>();
            ActivitySensorPLD = new List<string[]>();
            BarometerList = new List<Barometer>();
            BarometerDeviceInfo = new List<DeviceInformation>();
            BarometerPLD = new List<string[]>();
            CompassList = new List<Compass>();
            CompassDeviceInfo = new List<DeviceInformation>();
            CompassPLD = new List<string[]>();
            CustomSensorList = new List<CustomSensor>();
            CustomSensorDeviceInfo = new List<DeviceInformation>();
            CustomSensorPLD = new List<string[]>();
            GyrometerList = new List<Gyrometer>();
            GyrometerDeviceInfo = new List<DeviceInformation>();
            GyrometerPLD = new List<string[]>();
            InclinometerList = new List<Inclinometer>();
            InclinometerDeviceInfo = new List<DeviceInformation>();
            InclinometerPLD = new List<string[]>();
            LightSensorList = new List<LightSensor>();
            LightSensorDeviceInfo = new List<DeviceInformation>();
            LightSensorPLD = new List<string[]>();
            MagnetometerList = new List<Magnetometer>();
            MagnetometerDeviceInfo = new List<DeviceInformation>();
            MagnetometerPLD = new List<string[]>();
            OrientationAbsoluteList = new List<OrientationSensor>();
            OrientationAbsoluteDeviceInfo = new List<DeviceInformation>();
            OrientationAbsolutePLD = new List<string[]>();
            OrientationGeomagneticList = new List<OrientationSensor>();
            OrientationGeomagneticDeviceInfo = new List<DeviceInformation>();
            OrientationGeomagneticPLD = new List<string[]>();
            OrientationRelativeList = new List<OrientationSensor>();
            OrientationRelativeDeviceInfo = new List<DeviceInformation>();
            OrientationRelativePLD = new List<string[]>();
            PedometerList = new List<Pedometer>();
            PedometerDeviceInfo = new List<DeviceInformation>();
            PedometerPLD = new List<string[]>();
            ProximitySensorList = new List<ProximitySensor>();
            ProximitySensorDeviceInfo = new List<DeviceInformation>();
            ProximitySensorPLD = new List<string[]>();
            SimpleOrientationSensorList = new List<SimpleOrientationSensor>();
            SimpleOrientationSensorDeviceInfo = new List<DeviceInformation>();
            SimpleOrientationSensorPLD = new List<string[]>();

            NumFailedEnumerations = 0;

            AccelerometerStandardFailed = false;
            AccelerometerLinearFailed = false;
            AccelerometerGravityFailed = false;
            ActivitySensorFailed = false;
            AltimeterFailed = false;
            BarometerFailed = false;
            CompassFailed = false;
            CustomSensorFailed = false;
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
                //GUID_SensorType_Accelerometer3D = {C2FB0F5F-E2D2-4C78-BCD0-352A9582819D}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Accelerometer.GetDeviceSelector(AccelerometerReadingType.Standard), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Accelerometer accelerometer = await Accelerometer.FromIdAsync(deviceInfo.Id);
                    AccelerometerStandardList.Add(accelerometer);
                    AccelerometerStandardDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        AccelerometerStandardPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                AccelerometerStandardFailed = true;
            }
            try
            {
                //GUID_SensorType_LinearAccelerometer = {038B0283-97B4-41C8-BC24-5FF1AA48FEC7}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Accelerometer.GetDeviceSelector(AccelerometerReadingType.Linear), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Accelerometer accelerometer = await Accelerometer.FromIdAsync(deviceInfo.Id);
                    AccelerometerLinearList.Add(accelerometer);
                    AccelerometerLinearDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        AccelerometerLinearPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                AccelerometerLinearFailed = false;
            }
            try
            {
                //GUID_SensorType_GravityVector = {03B52C73-BB76-463F-9524-38DE76EB700B}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Accelerometer.GetDeviceSelector(AccelerometerReadingType.Gravity), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Accelerometer accelerometer = await Accelerometer.FromIdAsync(deviceInfo.Id);
                    AccelerometerGravityList.Add(accelerometer);
                    AccelerometerGravityDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        AccelerometerGravityPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                AccelerometerGravityFailed = false;
            }
            try
            {
                //GUID_SensorType_ActivityDetection = {9D9E0118-1807-4F2E-96E4-2CE57142E196}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(ActivitySensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    ActivitySensor activitySensor = await ActivitySensor.FromIdAsync(deviceInfo.Id);
                    ActivitySensorList.Add(activitySensor);
                    ActivitySensorDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        ActivitySensorPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                ActivitySensorFailed = false;
            }
            try
            {
                Altimeter = Altimeter.GetDefault();

                if (Altimeter != null)
                {
                    AltimeterDeviceInfo = await DeviceInformation.CreateFromIdAsync(Altimeter.DeviceId);

                    if (getPLD)
                    {
                        string deviceInstanceId = AltimeterDeviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        AltimeterPLD = await GetPLDInformation(deviceInstanceId);
                    }
                }
            }
            catch
            {
                AltimeterFailed = false;
            }
            try
            {
                //GUID_SensorType_Barometer = {0E903829-FF8A-4A93-97DF-3DCBDE402288}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Barometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Barometer barometer = await Barometer.FromIdAsync(deviceInfo.Id);
                    BarometerList.Add(barometer);
                    BarometerDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        BarometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                BarometerFailed = false;
            }
            try
            {
                //GUID_SensorType_Orientation = {CDB5D8F7-3CFD-41C8-8542-CCE622CF5D6E}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Compass.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Compass compass = await Compass.FromIdAsync(deviceInfo.Id);
                    CompassList.Add(compass);
                    CompassDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        CompassPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                CompassFailed = false;
            }
            try
            {
                //GUID_SensorType_Custom = {E83AF229-8640-4D18-A213-E22675EBB2C3}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(CustomSensor.GetDeviceSelector(new Guid("E83AF229-8640-4D18-A213-E22675EBB2C3")), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Compass compass = await Compass.FromIdAsync(deviceInfo.Id);
                    CompassList.Add(compass);
                    CompassDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        CompassPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                CompassFailed = false;
            }
            try
            {
                //GUID_SensorType_Gyrometer3D = {09485F5A-759E-42C2-BD4B-A349B75C8643}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Gyrometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Gyrometer gyrometer = await Gyrometer.FromIdAsync(deviceInfo.Id);
                    GyrometerList.Add(gyrometer);
                    GyrometerDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        GyrometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                GyrometerFailed = false;
            }
            try
            {
                //GUID_SensorType_Gyrometer3D = {09485F5A-759E-42C2-BD4B-A349B75C8643}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Inclinometer.GetDeviceSelector(SensorReadingType.Absolute), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Inclinometer inclinometer = await Inclinometer.FromIdAsync(deviceInfo.Id);
                    InclinometerList.Add(inclinometer);
                    InclinometerDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        InclinometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                InclinometerFailed = false;
            }
            try
            {
                //GUID_SensorType_AmbientLight = {97F115C8-599A-4153-8894-D2D12899918A}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(LightSensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    LightSensor lightSensor = await LightSensor.FromIdAsync(deviceInfo.Id);
                    LightSensorList.Add(lightSensor);
                    LightSensorDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        LightSensorPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                LightSensorFailed = false;
            }
            try
            {
                //GUID_SensorType_Magnetometer3D = {55E5EFFB-15C7-40df-8698-A84B7C863C53}
                string s = Magnetometer.GetDeviceSelector();
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Magnetometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Magnetometer magnetometer = await Magnetometer.FromIdAsync(deviceInfo.Id);
                    MagnetometerList.Add(magnetometer);
                    MagnetometerDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        MagnetometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                MagnetometerFailed = false;
            }
            try
            {
                //GUID_SensorType_Orientation = {CDB5D8F7-3CFD-41C8-8542-CCE622CF5D6E}
                string s = OrientationSensor.GetDeviceSelector(SensorReadingType.Absolute);
                deviceInfoCollection = await DeviceInformation.FindAllAsync(OrientationSensor.GetDeviceSelector(SensorReadingType.Absolute), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    OrientationSensor orientationSensor = await OrientationSensor.FromIdAsync(deviceInfo.Id);
                    OrientationAbsoluteList.Add(orientationSensor);
                    OrientationAbsoluteDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        OrientationAbsolutePLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                OrientationAbsoluteFailed = false;
            }
            try
            {
                //GUID_SensorType_Orientation = {CDB5D8F7-3CFD-41C8-8542-CCE622CF5D6E}
                string s = OrientationSensor.GetDeviceSelector(SensorReadingType.Absolute);
                deviceInfoCollection = await DeviceInformation.FindAllAsync(OrientationSensor.GetDeviceSelector(SensorReadingType.Absolute, SensorOptimizationGoal.PowerEfficiency), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    OrientationSensor orientationSensor = await OrientationSensor.FromIdAsync(deviceInfo.Id);
                    OrientationGeomagneticList.Add(orientationSensor);
                    OrientationGeomagneticDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        OrientationGeomagneticPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                OrientationGeomagneticFailed = false;
            }
            try
            {
                //GUID_SensorType_RelativeOrientation = {40993B51-4706-44DC-98D5-C920C037FFAB}
                string s = OrientationSensor.GetDeviceSelector(SensorReadingType.Relative);
                deviceInfoCollection = await DeviceInformation.FindAllAsync(OrientationSensor.GetDeviceSelector(SensorReadingType.Relative), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    OrientationSensor orientationSensor = await OrientationSensor.FromIdAsync(deviceInfo.Id);
                    OrientationRelativeList.Add(orientationSensor);
                    OrientationRelativeDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        OrientationRelativePLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                OrientationRelativeFailed = false;
            }
            try
            {
                //GUID_SensorType_Pedometer = {B19F89AF-E3EB-444B-8DEA-202575A71599}
                string s = Pedometer.GetDeviceSelector();
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Pedometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Pedometer pedometer = await Pedometer.FromIdAsync(deviceInfo.Id);
                    PedometerList.Add(pedometer);
                    PedometerDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        PedometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                PedometerFailed = false;
            }
            try
            {
                //GUID_SensorType_Proximity = {5220DAE9-3179-4430-9F90-06266D2A34DE}
                string s = ProximitySensor.GetDeviceSelector();
                deviceInfoCollection = await DeviceInformation.FindAllAsync(ProximitySensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    ProximitySensor proximitySensor = ProximitySensor.FromId(deviceInfo.Id);
                    ProximitySensorList.Add(proximitySensor);
                    ProximitySensorDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        ProximitySensorPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
                }
            }
            catch
            {
                ProximitySensorFailed = false;
            }
            try
            {
                //GUID_SensorType_SimpleDeviceOrientation = {86A19291-0482-402C-BF4C-ADDAC52B1C39}
                string s = SimpleOrientationSensor.GetDeviceSelector();
                deviceInfoCollection = await DeviceInformation.FindAllAsync(SimpleOrientationSensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    SimpleOrientationSensor simpleOrientationSensor = await SimpleOrientationSensor.FromIdAsync(deviceInfo.Id);
                    SimpleOrientationSensorList.Add(simpleOrientationSensor);
                    SimpleOrientationSensorDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                        SimpleOrientationSensorPLD.Add(await GetPLDInformation(deviceInstanceId));
                    }
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
                    case CUSTOMSENSOR: EnableCustomSensor(index, totalIndex); break;
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
                    case CUSTOMSENSOR: DisableCustomSensor(index); break;
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, reportLatency, GetProperties(AccelerometerStandardDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(AccelerometerStandardPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, reportLatency, GetProperties(AccelerometerLinearDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(AccelerometerLinearPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, reportLatency, GetProperties(AccelerometerGravityDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(AccelerometerGravityPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(CompassDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(CompassPLD[index]);
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

        private static void EnableCustomSensor(int index, int totalIndex)
        {
            if (CustomSensorList[index] != null)
            {
                string deviceId = string.Empty;
                string deviceName = string.Empty;
                uint reportInterval = 0;
                uint minimumReportInterval = 0;

                try
                {
                    deviceId = CustomSensorList[index].DeviceId;
                }
                catch { }
                try
                {
                    reportInterval = CustomSensorList[index].ReportInterval;
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(CompassDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(CompassPLD[index]);
                CustomSensorList[index].ReadingChanged += CustomSensor_ReadingChanged;
            }
        }

        private static void DisableCustomSensor(int index)
        {
            if (CustomSensorList[index] != null)
            {
                CustomSensorList[index].ReadingChanged -= CustomSensor_ReadingChanged;
            }
        }

        private async static void CustomSensor_ReadingChanged(object sender, CustomSensorReadingChangedEventArgs e)
        {
            try
            {
                if (sensorData[currentId]._sensorType == CUSTOMSENSOR)
                {
                    CustomSensorReading reading = e.Reading;
                    if (sensorData[currentId].AddReading(reading.Timestamp.UtcDateTime, new double[0]))
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(GyrometerDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(GyrometerPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(InclinometerDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(InclinometerPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(InclinometerDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(LightSensorPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(OrientationAbsoluteDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(OrientationAbsolutePLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(OrientationRelativeDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(OrientationRelativePLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(OrientationGeomagneticDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(OrientationGeomagneticPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, 0, minimumReportInterval, 0, GetProperties(ActivitySensorDeviceInfo[index]));

                // subscribe to all supported activities
                foreach (ActivityType activityType in ActivitySensorList[index].SupportedActivities)
                {
                    ActivitySensorList[index].SubscribedActivities.Add(activityType);
                }

                sensorData[totalIndex].AddPLDProperty(ActivitySensorPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(AltimeterDeviceInfo));
                sensorData[totalIndex].AddPLDProperty(AltimeterPLD);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(BarometerDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(BarometerPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(MagnetometerDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(MagnetometerPLD[index]);
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

                sensorData[totalIndex].AddProperty(deviceId, deviceName, reportInterval, minimumReportInterval, 0, GetProperties(PedometerDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(PedometerPLD[index]);
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

                try
                {
                    deviceId = ProximitySensorList[index].DeviceId;
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, 0, 0, 0, GetProperties(ProximitySensorDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(ProximitySensorPLD[index]);
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

                try
                {
                    deviceId = SimpleOrientationSensorList[index].DeviceId;
                }
                catch { }

                sensorData[totalIndex].AddProperty(deviceId, deviceName, 0, 0, 0, GetProperties(SimpleOrientationSensorDeviceInfo[index]));
                sensorData[totalIndex].AddPLDProperty(SimpleOrientationSensorPLD[index]);
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