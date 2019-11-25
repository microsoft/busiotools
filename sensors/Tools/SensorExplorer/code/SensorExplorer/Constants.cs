// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/* Copyright (c) Intel Corporation. All rights reserved.
   Licensed under the MIT License. */

using System.Collections.Generic;
using Windows.UI;

namespace SensorExplorer
{
    public static class Constants
    {
        public static readonly Dictionary<string, string> DeviceProperties = new Dictionary<string, string>() {
            { "BiosDeviceName", "{540b947e-8b40-45bc-a8a2-6a0b894cbda2} 10" },
            { "DeviceAddress", "{a45c254e-df1c-4efd-8020-67d146a850e0} 30" },
            { "ParentDevice", "{4340a6c5-93fa-4706-972c-7b648008a5a7} 8" }
        };

        public static readonly Dictionary<string, string> PLD = new Dictionary<string, string>() {
            { "Device_PanelId", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 2" },
            { "Device_PanelGroup", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 3" },
            { "Device_PanelSide", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 4" },
            { "Device_PanelWidth", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 5" },
            { "Device_PanelHeight", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 6" },
            { "Device_PanelLength", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 7" },
            { "Device_PanelPositionX", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 8" },
            { "Device_PanelPositionY", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 9" },
            { "Device_PanelPositionZ", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 10" },
            { "Device_PanelRotationX", "{8DBC9PositionXC86-97A9-4BFF-9BC6-BFE95D3E6DAD} 11" },
            { "Device_PanelRotationY", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 12" },
            { "Device_PanelRotationZ", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 13" },
            { "Device_PanelColor", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 14" },
            { "Device_PanelShape", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 15" },
            { "Device_PanelVisible", "{8DBC9C86-97A9-4BFF-9BC6-BFE95D3E6DAD} 16" }
        };

        public static readonly Dictionary<string, string> PanelGroup = new Dictionary<string, string>() {
            { "0", "Right Panel" },
            { "1", "Left Panel" }
        };

        public static readonly Dictionary<string, string> PanelRotation = new Dictionary<string, string>() {
            { "0", "0°" },
            { "1", "45°" },
            { "2", "90°" },
            { "3", "135°" },
            { "4", "180°" },
            { "5", "225°" },
            { "6", "270" },
            { "7", "315°" }
        };

        public static readonly Dictionary<string, string> PanelShape = new Dictionary<string, string>() {
            { "0", "Round" },
            { "1", "Oval" },
            { "2", "Square" },
            { "3", "Vertical Rectangle" },
            { "4", "Horizontal Rectangle" },
            { "5", "Vertical Trapezoid" },
            { "6", "Horizontal Trapezoid" },
            { "7", "Unknown" },
            { "8", "Chamfered" }
        };

        public static readonly Dictionary<string, string> PanelSide = new Dictionary<string, string>() {
            { "0", "Top" },
            { "1", "Bottom" },
            { "2", "Left" },
            { "3", "Right" },
            { "4", "Front" },
            { "5", "Back" },
            { "6", "Unknown" }
        };

        public static readonly Dictionary<string, string> Properties = new Dictionary<string, string>() {
            { "Sensor_Type", "{D4247382-969D-4F24-BB14-FB9671870BBF} 2" },
            { "Sensor_Category", "{D4247382-969D-4F24-BB14-FB9671870BBF} 3" },
            { "Sensor_ConnectionType", "{D4247382-969D-4F24-BB14-FB9671870BBF} 4" },
            { "Sensor_IsPrimary", "{D4247382-969D-4F24-BB14-FB9671870BBF} 5" },
            { "Sensor_Name", "{D4247382-969D-4F24-BB14-FB9671870BBF} 6" },
            { "Sensor_Manufacturer", "{D4247382-969D-4F24-BB14-FB9671870BBF} 7" },
            { "Sensor_Model", "{D4247382-969D-4F24-BB14-FB9671870BBF} 8" },
            { "Sensor_PersistentUniqueId", "{D4247382-969D-4F24-BB14-FB9671870BBF} 9" },
            { "Sensor_VendorDefinedSubType", "{D4247382-969D-4F24-BB14-FB9671870BBF} 10" },
            { "Sensor_State", "{D4247382-969D-4F24-BB14-FB9671870BBF} 20" },
            { "DEVPKEY_Device_InstanceId", "{78C34FC8-104A-4ACA-9EA4-524D52996E57} 256" }
        };

        public static readonly Dictionary<string, string> SensorCategories = new Dictionary<string, string>() {
            { "C317C286-C468-4288-9975-D4C4587C442C".ToLower(), "All" },
            { "CA19690F-A2C7-477D-A99E-99EC6E2B5648".ToLower(), "Biometric" },
            { "FB73FCD8-FC4A-483C-AC58-27B691C6BEFF".ToLower(), "Electrical" },
            { "323439AA-7F66-492B-BA0C-73E9AA0A65D5".ToLower(), "Environmental" },
            { "17A665C0-9063-4216-B202-5C7A255E18CE".ToLower(), "Light" },
            { "BFA794E4-F964-4FDB-90F6-51056BFE4B44".ToLower(), "Location" },
            { "8D131D68-8EF7-4656-80B5-CCCBD93791C5".ToLower(), "Mechanical" },
            { "CD09DAF1-3B2E-4C3D-B598-B5E5FF93FD46".ToLower(), "Motion" },
            { "9E6C04B6-96FE-4954-B726-68682A473F69".ToLower(), "Orientation" },
            { "2C90E7A9-F4C9-4FA2-AF37-56D471FE5A3D".ToLower(), "Other" },
            { "F1609081-1E12-412B-A14D-CBB0E95BD2E5".ToLower(), "PersonalActivity" },
            { "B000E77E-F5B5-420F-815D-0270A726F270".ToLower(), "Scanner" },
            { "2BEAE7FA-19B0-48C5-A1F6-B5480DC206B0".ToLower(), "Unsupported" }
        };

        public static readonly string[] SensorConnectionTypes = new string[] {
            "Integrated",
            "Attached",
            "External"
        };

        public static readonly string[] RequestedProperties = new string[] {
            Properties["Sensor_Type"],
            Properties["Sensor_Category"],
            Properties["Sensor_ConnectionType"],
            Properties["Sensor_Name"],
            Properties["Sensor_Manufacturer"],
            Properties["Sensor_Model"],
            Properties["Sensor_PersistentUniqueId"]
        };

        public static readonly Dictionary<int, double[]> OffsetTestExpectedValue = new Dictionary<int, double[]> {
            { Sensor.ACCELEROMETER, new double[3] { 0, 0, -1 } },
            { Sensor.GYROMETER, new double[3] { 0, 0, 0 } },
            { Sensor.LIGHTSENSOR, new double[1] { 0 } },
            { Sensor.ORIENTATIONSENSOR, new double[4] { 0, 0, 0, 1 } },
            { Sensor.ORIENTATIONGEOMAGNETIC, new double[4] { 0, 0, 0, 1 } }
        };

        public static readonly Dictionary<int, string> SensorName = new Dictionary<int, string>() {
            { Sensor.ACCELEROMETER, "Accelerometer (Standard)" },
            { Sensor.ACCELEROMETERGRAVITY, "Accelerometer (Gravity)" },
            { Sensor.ACCELEROMETERLINEAR, "Accelerometer (Linear)" },
            { Sensor.ACTIVITYSENSOR, "Activity Sensor" },
            { Sensor.ALTIMETER, "Altimeter" },
            { Sensor.BAROMETER, "Barometer" },
            { Sensor.COMPASS, "Compass" },
            { Sensor.CUSTOMSENSOR, "Custom Sensor" },
            { Sensor.GYROMETER, "Gyrometer" },
            { Sensor.INCLINOMETER, "Inclinometer" },
            { Sensor.LIGHTSENSOR, "Light Sensor" },
            { Sensor.MAGNETOMETER, "Magnetometer" },
            { Sensor.ORIENTATIONGEOMAGNETIC, "Orientation Sensor (Geomagnetic)" },
            { Sensor.ORIENTATIONRELATIVE, "Orienation Sensor (Relative)" },
            { Sensor.ORIENTATIONSENSOR, "Orientation Sensor" },
            { Sensor.PEDOMETER, "Pedometer" },
            { Sensor.PROXIMITYSENSOR, "Proximity Sensor" },
            { Sensor.SIMPLEORIENTATIONSENSOR, "Simple Orientation Sensor" }
        };

        public static readonly string[] AccelerometerPropertyTitles = new string[] {
            "AccelerationX (g)",
            "AccelerationY (g)",
            "AccelerationZ (g)"
        };

        public static readonly string[] ActivitySensorPropertyTitles = new string[] {
            "Activity",
            "Confidence"
        };

        public static readonly string[] AltimeterPropertyTitles = new string[] {
            "Altitude Change (m)"
        };

        public static readonly string[] BarometerPropertyTitles = new string[] {
            "Pressure (hPa)"
        };

        public static readonly string[] CompassPropertyTitles = new string[] {
            "MagneticNorth (°)",
            "TrueNorth (°)",
            "HeadingAccuracy"
        };

        public static readonly string[] CustomSensorPropertyTitles = new string[0];

        public static readonly string[] GyrometerPropertyTitles = new string[] {
            "AngularVelocityX (°/s)",
            "AngularVelocityY (°/s)",
            "AngularVelocityZ (°/s)"
        };

        public static readonly string[] InclinometerPropertyTitles = new string[] {
            "Pitch (°)",
            "Roll (°)",
            "Yaw (°)",
            "YawAccuracy"
        };

        public static readonly string[] LightSensorPropertyTitles = new string[] {
            "Illuminance (lux)",
            "Chromaticity X",
            "Chromaticity Y"
        };

        public static readonly string[] MagnetometerPropertyTitles = new string[] {
            "MagneticFieldX (µT)",
            "MagneticFieldY (µT)",
            "MagneticFieldZ (µT)"
        };

        public static readonly string[] OrientationSensorPropertyTitles = new string[] {
            "QuaternionX",
            "QuaternionY",
            "QuaternionZ",
            "QuaternionW",
            "RotationMatrixM11",
            "RotationMatrixM12",
            "RotationMatrixM13",
            "RotationMatrixM21",
            "RotationMatrixM22",
            "RotationMatrixM23",
            "RotationMatrixM31",
            "RotationMatrixM32",
            "RotationMatrixM33"
        };

        public static readonly string[] PedometerPropertyTitles = new string[] {
            "CumulativeSteps",
            "CumulativeStepsDuration (s)",
            "StepKind"
        };

        public static readonly string[] ProximitySensorPropertyTitles = new string[] {
            "IsDetected",
            "Distance (mm)"
        };

        public static readonly string[] SimpleOrientationSensorPropertyTitles = new string[] {
            "SimpleOrientation"
        };

        public static readonly Color[] AccelerometerColors = new Color[] {
            Colors.DarkRed,
            Colors.DarkOrange,
            Colors.DarkCyan
        };

        public static readonly Color[] ActivitySensorColors = new Color[] {
            Colors.Gray,
            Colors.Brown
        };

        public static readonly Color[] AltimeterColors = new Color[] {
            Colors.DarkRed
        };

        public static readonly Color[] BarometerColors = new Color[] {
            Colors.Lime
        };

        public static readonly Color[] CompassColors = new Color[] {
            Colors.DarkRed,
            Colors.DarkOrange,
            Colors.DarkCyan
        };

        public static readonly Color[] CustomSensorColors = new Color[0];

        public static readonly Color[] GyrometerColors = new Color[] {
            Colors.DarkRed,
            Colors.DarkOrange,
            Colors.DarkCyan
        };

        public static readonly Color[] InclinometerColors = new Color[] {
            Colors.DarkRed,
            Colors.DarkOrange,
            Colors.DarkCyan,
            Colors.Black
        };

        public static readonly Color[] LightSensorColors = new Color[] {
            Colors.DarkRed,
            Colors.DarkOrange,
            Colors.DarkCyan
        };

        public static readonly Color[] MagnetometerColors = new Color[] {
            Colors.DarkRed,
            Colors.DarkOrange,
            Colors.DarkCyan
        };

        public static readonly Color[] OrientationSensorColors = new Color[] {
            Colors.DarkRed,
            Colors.DarkOrange,
            Colors.DarkCyan,
            Colors.DarkViolet,
            Colors.Black,
            Colors.Black,
            Colors.Black,
            Colors.Black,
            Colors.Black,
            Colors.Black,
            Colors.Black,
            Colors.Black,
            Colors.Black
        };

        public static readonly Color[] PedometerColors = new Color[] {
            Colors.DarkRed,
            Colors.DarkOrange,
            Colors.DarkCyan
        };

        public static readonly Color[] ProximitySensorColors = new Color[] {
            Colors.DarkOrange,
            Colors.Black
        };

        public static readonly Color[] SimpleOrientationSensorColors = new Color[] {
            Colors.Lime
        };
    }
    public class DeviceProperties
    {
        public const string DeviceInstanceId = "System.Devices.DeviceInstanceId";
    }

    public class ArduinoDevice
    {
        public const ushort Vid = 0x2341;
        public const ushort Pid = 0x0042;
    }
}