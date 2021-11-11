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
        public const int ACCELEROMETERGRAVITY = 1;
        public const int ACCELEROMETERLINEAR = 2;
        public const int ACTIVITYSENSOR = 3;
        public const int ALTIMETER = 4;
        public const int BAROMETER = 5;
        public const int COMPASS = 6;
        public const int CUSTOMSENSOR = 7;
        public const int GYROMETER = 8;
        public const int INCLINOMETER = 9;
        public const int LIGHTSENSOR = 10;
        public const int MAGNETOMETER = 11;
        public const int ORIENTATIONSENSOR = 12;
        public const int ORIENTATIONGEOMAGNETIC = 13;
        public const int ORIENTATIONRELATIVE = 14;
        public const int PEDOMETER = 15;
        public const int PROXIMITYSENSOR = 16;
        public const int SIMPLEORIENTATIONSENSOR = 17;

        public const int ACTIVITYNONE = 2;
        public const int ACTIVITYNOTSUPPORTED = 3;

        public static DeviceInformationCollection SensorClassDevice;
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

        public static int CurrentId = -1;
        public static int NumFailedEnumerations;
        public static List<SensorData> SensorData;
        public static List<SensorDisplay> SensorDisplay;

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

        private static CoreDispatcher cd = Window.Current.CoreWindow.Dispatcher;

        public static async Task<string[]> GetProperties(DeviceInformation deviceInfo)
        {
            string[] properties = new string[11];
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
            catch
            {
                properties[5] = "N/A";
            }
            try
            {
                properties[6] = deviceInfo.Properties[Constants.Properties["Sensor_VendorDefinedSubType"]].ToString();
            }
            catch
            {
                properties[6] = "N/A";
            }
            try
            {
                properties[7] = deviceInfo.Properties[Constants.Properties["SensorState"]].ToString();
            }
            catch
            {
                properties[7] = "N/A";
            }

            try
            {
                string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                properties[8] = await GetAcpiObjectHierarchy(deviceInstanceId);
            }
            catch
            {
                properties[8] = "Device not created by ACPI";
            }

            try
            {
                properties[9] = deviceInfo.Properties[Constants.Properties["Sensor_Name"]].ToString();
            }
            catch
            {
                properties[9] = "No sensor name set";
            }

            try
            {
                properties[10] = Constants.HumanPresenceDetectionTypes[int.Parse(deviceInfo.Properties[Constants.Properties["DEVPKEY_Sensor_HumanPresenceDetectionType"]].ToString())];
            }
            catch
            {
                properties[10] = "N/A";
            }

            return properties;
        }

        public static async Task<string[]> GetPLDInformation(string deviceInstanceId, DeviceInformationKind kind = DeviceInformationKind.Device)
        {
            string[] pld = new string[15];
            bool isPldInformationPresent = false;
            try
            {
                DeviceInformation pldPanelId = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelId"] }, kind);
                if(pldPanelId.Properties[Constants.PLD["Device_PanelId"]] != null)
                {
                    pld[0] = pldPanelId.Properties[Constants.PLD["Device_PanelId"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelGroup = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelGroup"] }, kind);
                if(pldPanelGroup.Properties[Constants.PLD["Device_PanelGroup"]] != null)
                {
                    pld[1] = pldPanelGroup.Properties[Constants.PLD["Device_PanelGroup"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelSide = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelSide"] }, kind);
                if (pldPanelSide.Properties[Constants.PLD["Device_PanelSide"]] != null)
                {
                    pld[2] = pldPanelSide.Properties[Constants.PLD["Device_PanelSide"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelWidth = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelWidth"] }, kind);
                if (pldPanelWidth.Properties[Constants.PLD["Device_PanelWidth"]] != null)
                {
                    pld[3] = pldPanelWidth.Properties[Constants.PLD["Device_PanelWidth"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelHeight = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelHeight"] }, kind);
                if (pldPanelHeight.Properties[Constants.PLD["Device_PanelHeight"]] != null)
                {
                    pld[4] = pldPanelHeight.Properties[Constants.PLD["Device_PanelHeight"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelLength = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelLength"] }, kind);
                if (pldPanelLength.Properties[Constants.PLD["Device_PanelLength"]] != null)
                {
                    pld[5] = pldPanelLength.Properties[Constants.PLD["Device_PanelLength"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelPositionX = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelPositionX"] }, kind);
                if (pldPanelPositionX.Properties[Constants.PLD["Device_PanelPositionX"]] != null)
                {
                    pld[6] = pldPanelPositionX.Properties[Constants.PLD["Device_PanelPositionX"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelPositionY = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelPositionY"] }, kind);
                if (pldPanelPositionY.Properties[Constants.PLD["Device_PanelPositionY"]] != null)
                {
                    pld[7] = pldPanelPositionY.Properties[Constants.PLD["Device_PanelPositionY"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelPositionZ = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelPositionZ"] }, kind);
                if(pldPanelPositionZ.Properties[Constants.PLD["Device_PanelPositionZ"]] != null)
                {
                    pld[8] = pldPanelPositionZ.Properties[Constants.PLD["Device_PanelPositionZ"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelRotationX = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelRotationX"] }, kind);
                if(pldPanelRotationX.Properties[Constants.PLD["Device_PanelRotationX"]] != null)
                {
                    pld[9] = pldPanelRotationX.Properties[Constants.PLD["Device_PanelRotationX"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelRotationY = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelRotationY"] }, kind);
                if (pldPanelRotationY.Properties[Constants.PLD["Device_PanelRotationY"]] != null)
                {
                    pld[10] = pldPanelRotationY.Properties[Constants.PLD["Device_PanelRotationY"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelRotationZ = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelRotationZ"] }, kind);
                if(pldPanelRotationZ.Properties[Constants.PLD["Device_PanelRotationZ"]] != null)
                {
                    pld[11] = pldPanelRotationZ.Properties[Constants.PLD["Device_PanelRotationZ"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelColor = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelColor"] }, kind);
                if(pldPanelColor.Properties[Constants.PLD["Device_PanelColor"]] != null)
                {
                    pld[12] = pldPanelColor.Properties[Constants.PLD["Device_PanelColor"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelShape = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelShape"] }, kind);
                if(pldPanelShape.Properties[Constants.PLD["Device_PanelShape"]] != null)
                {
                    pld[13] = pldPanelShape.Properties[Constants.PLD["Device_PanelShape"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }
            try
            {
                DeviceInformation pldPanelVisible = await DeviceInformation.CreateFromIdAsync(deviceInstanceId, new string[] { Constants.PLD["Device_PanelVisible"] }, kind);
                if(pldPanelVisible.Properties[Constants.PLD["Device_PanelVisible"]] != null)
                {
                    pld[14] = pldPanelVisible.Properties[Constants.PLD["Device_PanelVisible"]].ToString();
                    isPldInformationPresent = true;
                }
            }
            catch { }

            return isPldInformationPresent ? pld : null;
        }

        private static async Task<string> GetAcpiObjectHierarchy(string deviceInstanceId)
        {
            string objectHierarchy = "Device was not created by ACPI";
            try
            {
                string currentDeviceObject = deviceInstanceId;
                Stack<string> objectNameAndAddress = new Stack<string>();
                string acpiObject = null;
                // Get the first device that has presence in ACPI
                do
                {
                    // Get the device's address, object name and the parent
                    DeviceInformation deviceProperties = await DeviceInformation.CreateFromIdAsync(
                        currentDeviceObject,
                        new string[] {
                            Constants.DeviceProperties["DeviceAddress"],
                            Constants.DeviceProperties["BiosDeviceName"],
                            Constants.DeviceProperties["ParentDevice"]
                        },
                        DeviceInformationKind.Device
                    );

                    // If the object has a name then terminate search
                    if (deviceProperties.Properties.ContainsKey(Constants.DeviceProperties["BiosDeviceName"]) && (null != deviceProperties.Properties[Constants.DeviceProperties["BiosDeviceName"]]))
                    {

                        acpiObject = deviceProperties.Properties[Constants.DeviceProperties["BiosDeviceName"]].ToString();
                        break;
                    }
                    // If not, store the current object's address in the stack and get this object's parent and set that as the current object
                    else if (deviceProperties.Properties.ContainsKey(Constants.DeviceProperties["ParentDevice"]) && (null != deviceProperties.Properties[Constants.DeviceProperties["ParentDevice"]]))
                    {
                        string deviceNameAndAddress = "";
                        if (null != deviceProperties.Name)
                        {
                            deviceNameAndAddress = deviceProperties.Name;
                        }
                        else
                        {
                            deviceNameAndAddress = currentDeviceObject;
                        }

                        if (deviceProperties.Properties.ContainsKey(Constants.DeviceProperties["DeviceAddress"]) && (null != deviceProperties.Properties[Constants.DeviceProperties["DeviceAddress"]]))
                        {
                            deviceNameAndAddress += " [Address: " + deviceProperties.Properties[Constants.DeviceProperties["DeviceAddress"]].ToString() + " ]";
                        }
                        else
                        {
                            deviceNameAndAddress += " [Address: No address found ]";
                        }

                        objectNameAndAddress.Push(deviceNameAndAddress);
                        currentDeviceObject = deviceProperties.Properties[Constants.DeviceProperties["ParentDevice"]].ToString();
                    }
                    else
                    {
                        break;
                    }
                }
                while (null != currentDeviceObject);

                // Now reconstruct the hierarchy.
                if (null != acpiObject)
                {
                    objectHierarchy = acpiObject;

                    while (objectNameAndAddress.Count > 0)
                    {
                        objectHierarchy += " -> " + objectNameAndAddress.Pop();
                    }
                }
            }
            catch (Exception Ex)
            {
                string message = Ex.Message;
            };

            return objectHierarchy;
        }

        private static void InitializeLists()
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
        }

        private static void InitializeVariables()
        {
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
        }

        public static async Task<bool> GetDefault(bool getPLD)
        {
            InitializeLists();
            InitializeVariables();

            DeviceInformationCollection deviceInfoCollection;

            // Enumerate the Sensor class
            try
            {
                Guid sensorGuid = new Guid("{5175d334-c371-4806-b3ba-71fd53c9258d}");
                SensorClassDevice = await DeviceInformation.FindAllAsync(CustomDevice.GetDeviceSelector(sensorGuid));
                NumFailedEnumerations = SensorClassDevice.Count;
            }
            catch
            {
                OtherSensorFailed = true;
            }
            try
            {
                //GUIDSensorType_Accelerometer3D = {C2FB0F5F-E2D2-4C78-BCD0-352A9582819D}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Accelerometer.GetDeviceSelector(AccelerometerReadingType.Standard), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Accelerometer accelerometer = await Accelerometer.FromIdAsync(deviceInfo.Id);
                    AccelerometerStandardList.Add(accelerometer);
                    AccelerometerStandardDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if(pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            AccelerometerStandardPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            AccelerometerStandardPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                AccelerometerStandardFailed = true;
            }
            try
            {
                //GUIDSensorType_GravityVector = {03B52C73-BB76-463F-9524-38DE76EB700B}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Accelerometer.GetDeviceSelector(AccelerometerReadingType.Gravity), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Accelerometer accelerometer = await Accelerometer.FromIdAsync(deviceInfo.Id);
                    AccelerometerGravityList.Add(accelerometer);
                    AccelerometerGravityDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            AccelerometerGravityPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            AccelerometerGravityPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                AccelerometerGravityFailed = false;
            }
            try
            {
                //GUIDSensorType_LinearAccelerometer = {038B0283-97B4-41C8-BC24-5FF1AA48FEC7}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Accelerometer.GetDeviceSelector(AccelerometerReadingType.Linear), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Accelerometer accelerometer = await Accelerometer.FromIdAsync(deviceInfo.Id);
                    AccelerometerLinearList.Add(accelerometer);
                    AccelerometerLinearDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            AccelerometerLinearPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            AccelerometerLinearPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                AccelerometerLinearFailed = false;
            }
            try
            {
                //GUIDSensorType_ActivityDetection = {9D9E0118-1807-4F2E-96E4-2CE57142E196}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(ActivitySensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    ActivitySensor activitySensor = await ActivitySensor.FromIdAsync(deviceInfo.Id);
                    ActivitySensorList.Add(activitySensor);
                    ActivitySensorDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            ActivitySensorPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            ActivitySensorPLD.Add(pldInfo);
                        }
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
                        var pldInfo = await GetPLDInformation(AltimeterDeviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = AltimeterDeviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            AltimeterPLD = await GetPLDInformation(deviceInstanceId);
                        }
                        else
                        {
                            AltimeterPLD = pldInfo;
                        }
                    }
                }
            }
            catch
            {
                AltimeterFailed = false;
            }
            try
            {
                //GUIDSensorType_Barometer = {0E903829-FF8A-4A93-97DF-3DCBDE402288}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Barometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Barometer barometer = await Barometer.FromIdAsync(deviceInfo.Id);
                    BarometerList.Add(barometer);
                    BarometerDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            BarometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            BarometerPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                BarometerFailed = false;
            }
            try
            {
                //GUIDSensorType_Orientation = {CDB5D8F7-3CFD-41C8-8542-CCE622CF5D6E}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Compass.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Compass compass = await Compass.FromIdAsync(deviceInfo.Id);
                    CompassList.Add(compass);
                    CompassDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            CompassPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            CompassPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                CompassFailed = false;
            }
            try
            {
                //GUIDSensorType_Custom = {E83AF229-8640-4D18-A213-E22675EBB2C3}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(CustomSensor.GetDeviceSelector(new Guid("E83AF229-8640-4D18-A213-E22675EBB2C3")), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    CustomSensor customSensor = await CustomSensor.FromIdAsync(deviceInfo.Id);
                    CustomSensorList.Add(customSensor);
                    CustomSensorDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            CustomSensorPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            CustomSensorPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                CustomSensorFailed = false;
            }
            try
            {
                //GUIDSensorType_Gyrometer3D = {09485F5A-759E-42C2-BD4B-A349B75C8643}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Gyrometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Gyrometer gyrometer = await Gyrometer.FromIdAsync(deviceInfo.Id);
                    GyrometerList.Add(gyrometer);
                    GyrometerDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            GyrometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            GyrometerPLD.Add(pldInfo);
                        }
                    }
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

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            InclinometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            InclinometerPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                InclinometerFailed = false;
            }
            try
            {
                //GUIDSensorType_AmbientLight = {97F115C8-599A-4153-8894-D2D12899918A}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(LightSensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    LightSensor lightSensor = await LightSensor.FromIdAsync(deviceInfo.Id);
                    LightSensorList.Add(lightSensor);
                    LightSensorDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            LightSensorPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            LightSensorPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                LightSensorFailed = false;
            }
            try
            {
                //GUIDSensorType_Magnetometer3D = {55E5EFFB-15C7-40df-8698-A84B7C863C53}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Magnetometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Magnetometer magnetometer = await Magnetometer.FromIdAsync(deviceInfo.Id);
                    MagnetometerList.Add(magnetometer);
                    MagnetometerDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            MagnetometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            MagnetometerPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                MagnetometerFailed = false;
            }
            try
            {
                //GUIDSensorType_Orientation = {CDB5D8F7-3CFD-41C8-8542-CCE622CF5D6E}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(OrientationSensor.GetDeviceSelector(SensorReadingType.Absolute), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    OrientationSensor orientationSensor = await OrientationSensor.FromIdAsync(deviceInfo.Id);
                    OrientationAbsoluteList.Add(orientationSensor);
                    OrientationAbsoluteDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            OrientationAbsolutePLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            OrientationAbsolutePLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                OrientationAbsoluteFailed = false;
            }
            try
            {
                //GUIDSensorType_Orientation = {CDB5D8F7-3CFD-41C8-8542-CCE622CF5D6E}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(OrientationSensor.GetDeviceSelector(SensorReadingType.Absolute, SensorOptimizationGoal.PowerEfficiency), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    OrientationSensor orientationSensor = await OrientationSensor.FromIdAsync(deviceInfo.Id);
                    OrientationGeomagneticList.Add(orientationSensor);
                    OrientationGeomagneticDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            OrientationGeomagneticPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            OrientationGeomagneticPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                OrientationGeomagneticFailed = false;
            }
            try
            {
                //GUIDSensorType_RelativeOrientation = {40993B51-4706-44DC-98D5-C920C037FFAB}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(OrientationSensor.GetDeviceSelector(SensorReadingType.Relative), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    OrientationSensor orientationSensor = await OrientationSensor.FromIdAsync(deviceInfo.Id);
                    OrientationRelativeList.Add(orientationSensor);
                    OrientationRelativeDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            OrientationRelativePLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            OrientationRelativePLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                OrientationRelativeFailed = false;
            }
            try
            {
                //GUIDSensorType_Pedometer = {B19F89AF-E3EB-444B-8DEA-202575A71599}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(Pedometer.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    Pedometer pedometer = await Pedometer.FromIdAsync(deviceInfo.Id);
                    PedometerList.Add(pedometer);
                    PedometerDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            PedometerPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            PedometerPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                PedometerFailed = false;
            }
            try
            {
                //GUIDSensorType_Proximity = {5220DAE9-3179-4430-9F90-06266D2A34DE}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(ProximitySensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    ProximitySensor proximitySensor = ProximitySensor.FromId(deviceInfo.Id);
                    ProximitySensorList.Add(proximitySensor);
                    ProximitySensorDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);

                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            ProximitySensorPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            ProximitySensorPLD.Add(pldInfo);
                        }
                    }
                }
            }
            catch
            {
                ProximitySensorFailed = false;
            }
            try
            {
                //GUIDSensorTypeSimpleDeviceOrientation = {86A19291-0482-402C-BF4C-ADDAC52B1C39}
                deviceInfoCollection = await DeviceInformation.FindAllAsync(SimpleOrientationSensor.GetDeviceSelector(), Constants.RequestedProperties);
                foreach (DeviceInformation deviceInfo in deviceInfoCollection)
                {
                    SimpleOrientationSensor simpleOrientationSensor = await SimpleOrientationSensor.FromIdAsync(deviceInfo.Id);
                    SimpleOrientationSensorList.Add(simpleOrientationSensor);
                    SimpleOrientationSensorDeviceInfo.Add(deviceInfo);

                    if (getPLD)
                    {
                        var pldInfo = await GetPLDInformation(deviceInfo.Id, DeviceInformationKind.DeviceInterface);



                        if (pldInfo == null)
                        {
                            string deviceInstanceId = deviceInfo.Properties[Constants.Properties["DEVPKEY_Device_InstanceId"]].ToString();
                            SimpleOrientationSensorPLD.Add(await GetPLDInformation(deviceInstanceId));
                        }
                        else
                        {
                            SimpleOrientationSensorPLD.Add(pldInfo);
                        }
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
                    case ACCELEROMETER:
                        EnableAccelerometer(index, totalIndex);
                        break;
                    case ACCELEROMETERGRAVITY:
                        EnableAccelerometerGravity(index, totalIndex);
                        break;
                    case ACCELEROMETERLINEAR:
                        EnableAccelerometerLinear(index, totalIndex);
                        break;
                    case ACTIVITYSENSOR:
                        EnableActivitySensor(index, totalIndex);
                        break;
                    case ALTIMETER:
                        EnableAltimeter(totalIndex);
                        break;
                    case BAROMETER:
                        EnableBarometer(index, totalIndex);
                        break;
                    case COMPASS:
                        EnableCompass(index, totalIndex);
                        break;
                    case CUSTOMSENSOR:
                        EnableCustomSensor(index, totalIndex);
                        break;
                    case GYROMETER:
                        EnableGyrometer(index, totalIndex);
                        break;
                    case INCLINOMETER:
                        EnableInclinometer(index, totalIndex);
                        break;
                    case LIGHTSENSOR:
                        EnableLightSensor(index, totalIndex);
                        break;
                    case MAGNETOMETER:
                        EnableMagnetometer(index, totalIndex);
                        break;
                    case ORIENTATIONSENSOR:
                        EnableOrientationSensor(index, totalIndex);
                        break;
                    case ORIENTATIONGEOMAGNETIC:
                        EnableOrientationGeomagnetic(index, totalIndex);
                        break;
                    case ORIENTATIONRELATIVE:
                        EnableOrientationRelative(index, totalIndex);
                        break;
                    case PEDOMETER:
                        EnablePedometer(index, totalIndex);
                        break;
                    case PROXIMITYSENSOR:
                        EnableProximitySensor(index, totalIndex);
                        break;
                    case SIMPLEORIENTATIONSENSOR:
                        EnableSimpleOrientationSensor(index, totalIndex);
                        break;
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
                    case ACCELEROMETER:
                        DisableAccelerometer(index);
                        break;
                    case ACCELEROMETERGRAVITY:
                        DisableAccelerometerGravity(index);
                        break;
                    case ACCELEROMETERLINEAR:
                        DisableAccelerometerLinear(index);
                        break;
                    case ACTIVITYSENSOR:
                        DisableActivitySensor(index);
                        break;
                    case ALTIMETER:
                        DisableAltimeter();
                        break;
                    case BAROMETER:
                        DisableBarometer(index);
                        break;
                    case COMPASS:
                        DisableCompass(index);
                        break;
                    case CUSTOMSENSOR:
                        DisableCustomSensor(index);
                        break;
                    case GYROMETER:
                        DisableGyrometer(index);
                        break;
                    case INCLINOMETER:
                        DisableInclinometer(index);
                        break;
                    case LIGHTSENSOR:
                        DisableLightSensor(index);
                        break;
                    case MAGNETOMETER:
                        DisableMagnetometer(index);
                        break;
                    case ORIENTATIONSENSOR:
                        DisableOrientationSensor(index);
                        break;
                    case ORIENTATIONGEOMAGNETIC:
                        DisableOrientationGeomagnetic(index);
                        break;
                    case ORIENTATIONRELATIVE:
                        DisableOrientationRelative(index);
                        break;
                    case PEDOMETER:
                        DisablePedometer(index);
                        break;
                    case PROXIMITYSENSOR:
                        DisableProximitySensor(index);
                        break;
                    case SIMPLEORIENTATIONSENSOR:
                        DisableSimpleOrientationSensor(index);
                        break;
                }
            }
            catch { }
        }

        private static async void EnableAccelerometer(int index, int totalIndex)
        {
            if (AccelerometerStandardList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(AccelerometerStandardDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, reportLatency, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(AccelerometerStandardPLD[index]);
                AccelerometerStandardList[index].ReadingChanged += AccelerometerReadingChanged;
            }
        }

        private static void DisableAccelerometer(int index)
        {
            if (AccelerometerStandardList[index] != null)
            {
                AccelerometerStandardList[index].ReadingChanged -= AccelerometerReadingChanged;
            }
        }

        private async static void AccelerometerReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == ACCELEROMETER)
                {
                    AccelerometerReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AccelerationX, reading.AccelerationY, reading.AccelerationZ }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableAccelerometerGravity(int index, int totalIndex)
        {
            if (AccelerometerGravityList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(AccelerometerGravityDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, reportLatency, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(AccelerometerGravityPLD[index]);
                AccelerometerGravityList[index].ReadingChanged += AccelerometerGravityReadingChanged;
            }
        }

        private static void DisableAccelerometerGravity(int index)
        {
            if (AccelerometerGravityList[index] != null)
            {
                AccelerometerGravityList[index].ReadingChanged -= AccelerometerGravityReadingChanged;
            }
        }

        private async static void AccelerometerGravityReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == ACCELEROMETERGRAVITY)
                {
                    AccelerometerReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AccelerationX, reading.AccelerationY, reading.AccelerationZ }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableAccelerometerLinear(int index, int totalIndex)
        {
            if (AccelerometerLinearList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(AccelerometerLinearDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, reportLatency, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(AccelerometerLinearPLD[index]);
                AccelerometerLinearList[index].ReadingChanged += AccelerometerLinearReadingChanged;
            }
        }

        private static void DisableAccelerometerLinear(int index)
        {
            if (AccelerometerLinearList[index] != null)
            {
                AccelerometerLinearList[index].ReadingChanged -= AccelerometerLinearReadingChanged;
            }
        }

        private async static void AccelerometerLinearReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == ACCELEROMETERLINEAR)
                {
                    AccelerometerReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AccelerationX, reading.AccelerationY, reading.AccelerationZ }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableActivitySensor(int index, int totalIndex)
        {
            if (ActivitySensorList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(ActivitySensorDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, 0, minimumReportInterval, 0, deviceProperties);

                // subscribe to all supported activities
                foreach (ActivityType activityType in ActivitySensorList[index].SupportedActivities)
                {
                    ActivitySensorList[index].SubscribedActivities.Add(activityType);
                }

                SensorData[totalIndex].AddPLDProperty(ActivitySensorPLD[index]);
                ActivitySensorList[index].ReadingChanged += ActivitySensorReadingChanged;
            }
        }

        private static void DisableActivitySensor(int index)
        {
            if (ActivitySensorList[index] != null)
            {
                ActivitySensorList[index].ReadingChanged -= ActivitySensorReadingChanged;
            }
        }

        private async static void ActivitySensorReadingChanged(object sender, ActivitySensorReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == ACTIVITYSENSOR)
                {
                    ActivitySensorReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { 0 }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableAltimeter(int totalIndex)
        {
            if (Altimeter != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(AltimeterDeviceInfo);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(AltimeterPLD);
                Altimeter.ReadingChanged += AltimeterReadingChanged;
            }
        }

        private static void DisableAltimeter()
        {
            if (Altimeter != null)
            {
                Altimeter.ReadingChanged -= AltimeterReadingChanged;
            }
        }

        private async static void AltimeterReadingChanged(object sender, AltimeterReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == ALTIMETER)
                {
                    AltimeterReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AltitudeChangeInMeters }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableBarometer(int index, int totalIndex)
        {
            if (BarometerList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(BarometerDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(BarometerPLD[index]);
                BarometerList[index].ReadingChanged += BarometerReadingChanged;
            }
        }

        private static void DisableBarometer(int index)
        {
            if (BarometerList[index] != null)
            {
                BarometerList[index].ReadingChanged -= BarometerReadingChanged;
            }
        }

        private async static void BarometerReadingChanged(object sender, BarometerReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == BAROMETER)
                {
                    BarometerReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.StationPressureInHectopascals }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableCompass(int index, int totalIndex)
        {
            if (CompassList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(CompassDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(CompassPLD[index]);
                CompassList[index].ReadingChanged += CompassReadingChanged;
            }
        }

        private static void DisableCompass(int index)
        {
            if (CompassList[index] != null)
            {
                CompassList[index].ReadingChanged -= CompassReadingChanged;
            }
        }

        private async static void CompassReadingChanged(object sender, CompassReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == COMPASS)
                {
                    CompassReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { Convert.ToDouble(reading.HeadingMagneticNorth),
                                                                                                       Convert.ToDouble(reading.HeadingTrueNorth),
                                                                                                       (int)reading.HeadingAccuracy }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableCustomSensor(int index, int totalIndex)
        {
            if (CustomSensorList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(CustomSensorDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(CustomSensorPLD[index]);
                CustomSensorList[index].ReadingChanged += CustomSensorReadingChanged;
            }
        }

        private static void DisableCustomSensor(int index)
        {
            if (CustomSensorList[index] != null)
            {
                CustomSensorList[index].ReadingChanged -= CustomSensorReadingChanged;
            }
        }

        private async static void CustomSensorReadingChanged(object sender, CustomSensorReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == CUSTOMSENSOR)
                {
                    CustomSensorReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[0]))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableGyrometer(int index, int totalIndex)
        {
            if (GyrometerList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(GyrometerDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(GyrometerPLD[index]);
                GyrometerList[index].ReadingChanged += GyrometerReadingChanged;
            }
        }

        private static void DisableGyrometer(int index)
        {
            if (GyrometerList[index] != null)
            {
                GyrometerList[index].ReadingChanged -= GyrometerReadingChanged;
            }
        }

        private async static void GyrometerReadingChanged(object sender, GyrometerReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == GYROMETER)
                {
                    GyrometerReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.AngularVelocityX, reading.AngularVelocityY, reading.AngularVelocityZ }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableInclinometer(int index, int totalIndex)
        {
            if (InclinometerList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(InclinometerDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(InclinometerPLD[index]);
                InclinometerList[index].ReadingChanged += InclinometerReadingChanged;
            }
        }

        private static void DisableInclinometer(int index)
        {
            if (InclinometerList[index] != null)
            {
                InclinometerList[index].ReadingChanged -= InclinometerReadingChanged;
            }
        }

        private async static void InclinometerReadingChanged(object sender, InclinometerReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == INCLINOMETER)
                {
                    InclinometerReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.PitchDegrees,
                                                                                                       reading.RollDegrees,
                                                                                                       reading.YawDegrees,
                                                                                                       (int)reading.YawAccuracy }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableLightSensor(int index, int totalIndex)
        {
            if (LightSensorList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(LightSensorDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(LightSensorPLD[index]);
                LightSensorList[index].ReadingChanged += LightSensorReadingChanged;
            }
        }

        private static void DisableLightSensor(int index)
        {
            if (LightSensorList[index] != null)
            {
                LightSensorList[index].ReadingChanged -= LightSensorReadingChanged;
            }
        }

        private async static void LightSensorReadingChanged(object sender, LightSensorReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == LIGHTSENSOR)
                {
                    LightSensorReading reading = e.Reading;
                    Scenario1View.Scenario1.LogDataLightSensor(reading);

                    // for color sensor
                    object x, y;
                    reading.Properties.TryGetValue("{C458F8A7-4AE8-4777-9607-2E9BDD65110A} 62", out x);
                    reading.Properties.TryGetValue("{C458F8A7-4AE8-4777-9607-2E9BDD65110A} 63", out y);

                    double chromaticityX = -1, chromaticityY = -1;
                    try
                    {
                        chromaticityX = double.Parse(x.ToString());
                        chromaticityY = double.Parse(y.ToString());
                    }
                    catch { }

                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.IlluminanceInLux, chromaticityX, chromaticityY }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableMagnetometer(int index, int totalIndex)
        {
            if (MagnetometerList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(MagnetometerDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(MagnetometerPLD[index]);
                MagnetometerList[index].ReadingChanged += MagnetometerReadingChanged;
            }
        }

        private static void DisableMagnetometer(int index)
        {
            if (MagnetometerList[index] != null)
            {
                MagnetometerList[index].ReadingChanged -= MagnetometerReadingChanged;
            }
        }

        private async static void MagnetometerReadingChanged(object sender, MagnetometerReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == MAGNETOMETER)
                {
                    MagnetometerReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.MagneticFieldX, reading.MagneticFieldY, reading.MagneticFieldZ }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableOrientationSensor(int index, int totalIndex)
        {
            if (OrientationAbsoluteList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(OrientationAbsoluteDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(OrientationAbsolutePLD[index]);
                OrientationAbsoluteList[index].ReadingChanged += OrientationSensorReadingChanged;
            }
        }

        private static void DisableOrientationSensor(int index)
        {
            if (OrientationAbsoluteList[index] != null)
            {
                OrientationAbsoluteList[index].ReadingChanged -= OrientationSensorReadingChanged;
            }
        }

        private async static void OrientationSensorReadingChanged(object sender, OrientationSensorReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == ORIENTATIONSENSOR)
                {
                    OrientationSensorReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.Quaternion.X,
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
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableOrientationGeomagnetic(int index, int totalIndex)
        {
            if (OrientationGeomagneticList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(OrientationGeomagneticDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(OrientationGeomagneticPLD[index]);
                OrientationGeomagneticList[index].ReadingChanged += OrientationGeomagneticReadingChanged;
            }
        }

        private static void DisableOrientationGeomagnetic(int index)
        {
            if (OrientationGeomagneticList[index] != null)
            {
                OrientationGeomagneticList[index].ReadingChanged -= OrientationGeomagneticReadingChanged;
            }
        }

        private async static void OrientationGeomagneticReadingChanged(object sender, OrientationSensorReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == ORIENTATIONRELATIVE)
                {
                    OrientationSensorReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.Quaternion.X,
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
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableOrientationRelative(int index, int totalIndex)
        {
            if (OrientationRelativeList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(OrientationRelativeDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(OrientationRelativePLD[index]);
                OrientationRelativeList[index].ReadingChanged += OrientationRelativeReadingChanged;
            }
        }

        private static void DisableOrientationRelative(int index)
        {
            if (OrientationRelativeList[index] != null)
            {
                OrientationRelativeList[index].ReadingChanged -= OrientationRelativeReadingChanged;
            }
        }

        private async static void OrientationRelativeReadingChanged(object sender, OrientationSensorReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == ORIENTATIONRELATIVE)
                {
                    OrientationSensorReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.Quaternion.X,
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
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnablePedometer(int index, int totalIndex)
        {
            if (PedometerList[index] != null)
            {
                string deviceId = string.Empty;
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

                var deviceProperties = await GetProperties(PedometerDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, reportInterval, minimumReportInterval, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(PedometerPLD[index]);
                PedometerList[index].ReadingChanged += PedometerReadingChanged;
            }
        }

        private static void DisablePedometer(int index)
        {
            if (PedometerList[index] != null)
            {
                PedometerList[index].ReadingChanged -= PedometerReadingChanged;
            }
        }

        private async static void PedometerReadingChanged(object sender, PedometerReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == PEDOMETER)
                {
                    PedometerReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { reading.CumulativeSteps, reading.CumulativeStepsDuration.Seconds, Convert.ToDouble(reading.StepKind) }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableProximitySensor(int index, int totalIndex)
        {
            if (ProximitySensorList[index] != null)
            {
                string deviceId = string.Empty;

                try
                {
                    deviceId = ProximitySensorList[index].DeviceId;
                }
                catch { }

                var deviceProperties = await GetProperties(ProximitySensorDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, 0, 0, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(ProximitySensorPLD[index]);
                ProximitySensorList[index].ReadingChanged += ProximitySensorReadingChanged;
            }
        }

        private static void DisableProximitySensor(int index)
        {
            if (ProximitySensorList[index] != null)
            {
                ProximitySensorList[index].ReadingChanged -= ProximitySensorReadingChanged;
            }
        }

        private async static void ProximitySensorReadingChanged(object sender, ProximitySensorReadingChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == PROXIMITYSENSOR)
                {
                    ProximitySensorReading reading = e.Reading;
                    if (SensorData[CurrentId].AddReading(reading.Timestamp.UtcDateTime, new double[] { Convert.ToDouble(reading.IsDetected), Convert.ToDouble(reading.DistanceInMillimeters) }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (CurrentId < SensorData.Count)
                            {
                                SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                            }
                        });
                    }
                }
            }
            catch { }
        }

        private static async void EnableSimpleOrientationSensor(int index, int totalIndex)
        {
            if (SimpleOrientationSensorList[index] != null)
            {
                string deviceId = string.Empty;

                try
                {
                    deviceId = SimpleOrientationSensorList[index].DeviceId;
                }
                catch { }

                var deviceProperties = await GetProperties(SimpleOrientationSensorDeviceInfo[index]);
                SensorData[totalIndex].AddProperty(deviceId, 0, 0, 0, deviceProperties);
                SensorData[totalIndex].AddPLDProperty(SimpleOrientationSensorPLD[index]);
                SimpleOrientationSensorList[index].OrientationChanged += SimpleOrientationSensorOrientationChanged;
            }
        }

        private static void DisableSimpleOrientationSensor(int index)
        {
            if (SimpleOrientationSensorList[index] != null)
            {
                SimpleOrientationSensorList[index].OrientationChanged -= SimpleOrientationSensorOrientationChanged;
            }
        }

        private async static void SimpleOrientationSensorOrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs e)
        {
            try
            {
                if (SensorData[CurrentId].SensorType == SIMPLEORIENTATIONSENSOR)
                {
                    if (SensorData[CurrentId].AddReading(DateTime.UtcNow, new double[] { Convert.ToDouble(e.Orientation) }))
                    {
                        await cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            SensorDisplay[CurrentId].UpdateText(SensorData[CurrentId]);
                        });
                    }
                }
            }
            catch { }
        }
    }
}