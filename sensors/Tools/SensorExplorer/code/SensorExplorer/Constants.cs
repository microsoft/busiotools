using System;
using System.Collections.Generic;
using Windows.UI;

namespace SensorExplorer
{
    public static class Constants
    {
        public static Dictionary<string, string> ColorSensorReading = new Dictionary<string, string>() {
            { "Chromaticity_X", "{C458F8A7-4AE8-4777-9607-2E9BDD65110A} 62" },
            { "Chromaticity_Y", "{C458F8A7-4AE8-4777-9607-2E9BDD65110A} 63" },
        };
        public static Dictionary<string, string> Properties = new Dictionary<string, string>() {
            { "Sensor_Type", "{D4247382-969D-4F24-BB14-FB9671870BBF} 2" },
            { "Sensor_Category", "{D4247382-969D-4F24-BB14-FB9671870BBF} 3" },
            { "Sensor_ConnectionType", "{D4247382-969D-4F24-BB14-FB9671870BBF} 4" },
            { "Sensor_IsPrimary", "{D4247382-969D-4F24-BB14-FB9671870BBF} 5" },
            { "Sensor_Name", "{D4247382-969D-4F24-BB14-FB9671870BBF} 6" },
            { "Sensor_Manufacturer", "{D4247382-969D-4F24-BB14-FB9671870BBF} 7" },
            { "Sensor_Model", "{D4247382-969D-4F24-BB14-FB9671870BBF} 8" },
            { "Sensor_PersistentUniqueId", "{D4247382-969D-4F24-BB14-FB9671870BBF} 9" },
            { "Sensor_VendorDefinedSubType", "{D4247382-969D-4F24-BB14-FB9671870BBF} 10" }
        };
        public static Dictionary<string, string> SensorTypes = new Dictionary<string, string>() {
            { "C2FB0F5F-E2D2-4C78-BCD0-352A9582819D".ToLower(), "Accelerometer3D" },
            { "9D9E0118-1807-4F2E-96E4-2CE57142E196".ToLower(), "ActivityDetection" },
            { "97F115C8-599A-4153-8894-D2D12899918A".ToLower(), "AmbientLight" },
            { "0E903829-FF8A-4A93-97DF-3DCBDE402288".ToLower(), "Barometer" },
            { "E83AF229-8640-4D18-A213-E22675EBB2C3".ToLower(), "Custom" },
            { "ADE4987F-7AC4-4DFA-9722-0A027181C747".ToLower(), "FloorElevation" },
            { "E77195F8-2D1F-4823-971B-1C4467556C9D".ToLower(), "GeomagneticOrientation" },
            { "03B52C73-BB76-463F-9524-38DE76EB700B".ToLower(), "GravityVector" },
            { "09485F5A-759E-42C2-BD4B-A349B75C8643".ToLower(), "Gyrometer3D" },
            { "5C72BF67-BD7E-4257-990B-98A3BA3B400A".ToLower(), "Humidity" },
            { "038B0283-97B4-41C8-BC24-5FF1AA48FEC7".ToLower(), "LinearAccelerometer" },
            { "55E5EFFB-15C7-40df-8698-A84B7C863C53".ToLower(), "Magnetometer3D" },
            { "CDB5D8F7-3CFD-41C8-8542-CCE622CF5D6E".ToLower(), "Orientation" },
            { "B19F89AF-E3EB-444B-8DEA-202575A71599".ToLower(), "Pedometer" },
            { "5220DAE9-3179-4430-9F90-06266D2A34DE".ToLower(), "Proximity" },
            { "40993B51-4706-44DC-98D5-C920C037FFAB".ToLower(), "RelativeOrientation" },
            { "86A19291-0482-402C-BF4C-ADDAC52B1C39".ToLower(), "SimpleDeviceOrientation" },
            { "04FD0EC4-D5DA-45FA-95A9-5DB38EE19306".ToLower(), "Temperature" }
        };
        public static Dictionary<string, string> SensorCategories = new Dictionary<string, string>() {
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
        public static string[] SensorConnectionTypes = new string[] { "Integrated", "Attached", "External" };
        public static string[] RequestedProperties = new string[] {
            Properties["Sensor_Type"],
            Properties["Sensor_Category"],
            Properties["Sensor_ConnectionType"],
            Properties["Sensor_Name"],
            Properties["Sensor_Manufacturer"],
            Properties["Sensor_Model"],
            Properties["Sensor_PersistentUniqueId"]
        };
        public static Dictionary<int, double[]> OffsetTestExpectedValue = new Dictionary<int, double[]> {
            { Sensor.ACCELEROMETER, new double[3] { 0, 0, -1 } },
            { Sensor.GYROMETER, new double[3] { 0, 0, 0 } },
            { Sensor.LIGHTSENSOR, new double[1] { 0 } },
            { Sensor.ORIENTATIONSENSOR, new double[4] { 0, 0, 0, 1 } }
        };
        public static Dictionary<int, string> SensorName = new Dictionary<int, string>()
        {
            { Sensor.ACCELEROMETER, "Accelerometer (Standard)" },
            { Sensor.ACCELEROMETERGRAVITY, "Accelerometer (Gravity)" },
            { Sensor.ACCELEROMETERLINEAR, "Accelerometer (Linear)" },
            { Sensor.ACTIVITYSENSOR, "Activity Sensor" },
            { Sensor.ALTIMETER, "Altimeter" },
            { Sensor.BAROMETER, "Barometer" },
            { Sensor.COMPASS, "Compass" },
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
        public static string[] AccelerometerPropertyTitles = new string[] { "AccelerationX (g)", "AccelerationY (g)", "AccelerationZ (g)" };
        public static string[] ActivitySensorPropertyTitles = new string[] { "Unknown", "Idle", "Stationary", "Fidgeting", "Walking", "Running", "InVehicle", "Biking" };
        public static string[] AltimeterPropertyTitles = new string[] { "AccelerationX (g)", "AccelerationY (g)", "AccelerationZ (g)" };
        public static string[] BarometerPropertyTitles = new string[] { "Pressure (hPa)" };
        public static string[] CompassPropertyTitles = new string[] { "MagneticNorth (°)", "TrueNorth (°)", "HeadingAccuracy" };
        public static string[] GyrometerPropertyTitles = new string[] { "AngularVelocityX (°/s)", "AngularVelocityY (°/s)", "AngularVelocityZ (°/s)" };
        public static string[] InclinometerPropertyTitles = new string[] { "Pitch (°)", "Roll (°)", "Yaw (°)", "YawAccuracy" };
        public static string[] LightSensorPropertyTitles = new string[] { "Illuminance (lux)" };
        public static string[] MagnetometerPropertyTitles = new string[] { "MagneticFieldX (µT)", "MagneticFieldY (µT)", "MagneticFieldZ (µT)" };
        public static string[] OrientationSensorPropertyTitles = new string[] { "QuaternionX", "QuaternionY", "QuaternionZ", "QuaternionW",
                                                                                "RotationMatrixM11", "RotationMatrixM12", "RotationMatrixM13",
                                                                                "RotationMatrixM21", "RotationMatrixM22", "RotationMatrixM23",
                                                                                "RotationMatrixM31", "RotationMatrixM32", "RotationMatrixM33" };
        public static string[] PedometerPropertyTitles = new string[] { "CumulativeSteps", "CumulativeStepsDuration (s)", "StepKind" };
        public static string[] ProximitySensorPropertyTitles = new string[] { "IsDetected", "Distance (mm)" };
        public static string[] SimpleOrientationSensorPropertyTitles = new string[] { "SimpleOrientation" };
        public static Color[] AccelerometerColors = new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan };
        public static Color[] ActivitySensorColors = new Color[] { Colors.Gray, Colors.Brown, Colors.DarkRed, Colors.Orange, Colors.DarkOrange, Colors.Lime, Colors.DarkCyan, Colors.DarkViolet };
        public static Color[] AltimeterColors = new Color[] { Colors.DarkRed };
        public static Color[] BarometerColors = new Color[] { Colors.Lime };
        public static Color[] CompassColors = new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan };
        public static Color[] GyrometerColors = new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan };
        public static Color[] InclinometerColors = new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan, Colors.Black };
        public static Color[] LightSensorColors = new Color[] { Colors.DarkOrange };
        public static Color[] MagnetometerColors = new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan };
        public static Color[] OrientationSensorColors = new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan, Colors.DarkViolet,
                                                                      Colors.Black, Colors.Black, Colors.Black,
                                                                      Colors.Black, Colors.Black, Colors.Black,
                                                                      Colors.Black, Colors.Black, Colors.Black };
        public static Color[] PedometerColors = new Color[] { Colors.DarkCyan, Colors.Black, Colors.Black };
        public static Color[] ProximitySensorColors = new Color[] { Colors.DarkOrange, Colors.Black };
        public static Color[] SimpleOrientationSensorColors = new Color[] { Colors.Lime };
}

    public class DeviceProperties
    {
        public const string DeviceInstanceId = "System.Devices.DeviceInstanceId";
    }

    public class ArduinoDevice
    {
        public const UInt16 Vid = 0x2341;
        public const UInt16 Pid = 0x0042;
    }
}
