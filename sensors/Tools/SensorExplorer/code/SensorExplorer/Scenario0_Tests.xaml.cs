// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/* Copyright (c) Intel Corporation. All rights reserved.
   Licensed under the MIT License. */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Foundation.Diagnostics;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

namespace SensorExplorer
{
    public sealed partial class Scenario0Tests : Page
    {
        public static Scenario0Tests Scenario0;
        public MainPage rootPage = MainPage.Current;
        public enum Directions { left, right, up, down, nothing }
        public List<int> SensorType;
        public bool IsSimpleOrientationSensor = false;

        private static readonly int countdownTime = 10; // In seconds
        private static readonly int testIterations = 8;
        private static readonly int numQuadrants = 4;
        private static readonly Dictionary<string, int> testLength =
            new Dictionary<string, int> {
                { "Frequency", 60 },
                { "Offset", 60 },
                { "Jitter", 60*15 },
                { "Drift", 60*15 },
                { "GyroDrift",60},
                { "PacketLoss", 60*5},
                { "StaticAccuracy", 5},
                { "MagInterference", 30}
            }; // In seconds
        private List<int> indices;
        private string testType;
        private bool cancelButtonClicked;
        private List<double[]> dataList;
        private List<double> oriAngles;
        private List<DateTime> timestampList;
        private Countdown countdown;
        private int[] quadrants = new int[numQuadrants]; // Number of tests completed in each quadrant
        private int testsCompleted;
        private int arrowDir;  // The direction the arrow is currently pointing in
        private Accelerometer currentAccelerometer;
        private DeviceInformation currentAccInfo;
        private AccelerometerReading accelerometerInitialReading;
        private Compass currentCompass;
        private CompassReading compassInitialReading;
        private Gyrometer currentGyrometer;
        private GyrometerReading gyrometerInitialReading;
        private Inclinometer currentInclinometer;
        private LightSensor currentLightSensor;
        private LightSensorReading lightSensorInitialReading;
        private Magnetometer currentMagnetometer;
        private OrientationSensor currentOrientationSensor;
        private OrientationSensorReading orientationSensorInitialReading;
        private List<double[]> orientationSensorFirstMinuteDataList;
        private List<double[]> orientationSensorLastMinuteDataList;
        private SimpleOrientationSensor currentSimpleOrientationSensor;
        private bool accelerometerInitialized;
        private bool inclinometerInitialized;
        private bool orientationInitialized;
        private bool orientationAmInitialized;
        private bool simpleOrientationInitialized;
        private string sensorDataLog;
        private DateTime startTime;
        private Run run = new Run();
        private int staticAccuracyCount;

        public Scenario0Tests()
        {
            InitializeComponent();
            Scenario0 = this;
            
            // disable screen display rotation during the test
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
            {
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
                instruction.FontSize = 30;
                timerLog.FontSize = 30;
            }
            else if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
                instruction.FontSize = 20;
                timerLog.FontSize = 20;
            }

            EnumerateSensors();
            hyperlink.Inlines.Add(run);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);
        }

        private async void EnumerateSensors()
        {
            try
            {
                await Sensor.GetDefault();
                int totalIndex = -1;
                SensorType = new List<int>();
                indices = new List<int>();
                for (int index = 0; index < Sensor.AccelerometerStandardList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.ACCELEROMETER, index, totalIndex);
                    SensorType.Add(Sensor.ACCELEROMETER);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.AccelerometerLinearList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.ACCELEROMETERLINEAR, index, totalIndex);
                    SensorType.Add(Sensor.ACCELEROMETERLINEAR);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.AccelerometerGravityList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.ACCELEROMETERGRAVITY, index, totalIndex);
                    SensorType.Add(Sensor.ACCELEROMETERGRAVITY);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.ActivitySensorList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.ACTIVITYSENSOR, index, totalIndex);
                    SensorType.Add(Sensor.ACTIVITYSENSOR);
                    indices.Add(index);
                }
                if (Sensor.Altimeter != null)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.ALTIMETER, 0, totalIndex);
                    SensorType.Add(Sensor.ALTIMETER);
                    indices.Add(0);
                }
                for (int index = 0; index < Sensor.BarometerList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.BAROMETER, index, totalIndex);
                    SensorType.Add(Sensor.BAROMETER);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.CompassList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.COMPASS, index, totalIndex);
                    SensorType.Add(Sensor.COMPASS);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.GyrometerList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.GYROMETER, index, totalIndex);
                    SensorType.Add(Sensor.GYROMETER);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.InclinometerList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.INCLINOMETER, index, totalIndex);
                    SensorType.Add(Sensor.INCLINOMETER);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.LightSensorList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.LIGHTSENSOR, index, totalIndex);
                    SensorType.Add(Sensor.LIGHTSENSOR);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.MagnetometerList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.MAGNETOMETER, index, totalIndex);
                    SensorType.Add(Sensor.MAGNETOMETER);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.OrientationAbsoluteList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.ORIENTATIONSENSOR, index, totalIndex);
                    SensorType.Add(Sensor.ORIENTATIONSENSOR);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.OrientationRelativeList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.ORIENTATIONRELATIVE, index, totalIndex);
                    SensorType.Add(Sensor.ORIENTATIONRELATIVE);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.OrientationGeomagneticList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.ORIENTATIONGEOMAGNETIC, index, totalIndex);
                    SensorType.Add(Sensor.ORIENTATIONGEOMAGNETIC);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.PedometerList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.PEDOMETER, index, totalIndex);
                    SensorType.Add(Sensor.PEDOMETER);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.ProximitySensorList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.PROXIMITYSENSOR, index, totalIndex);
                    SensorType.Add(Sensor.PROXIMITYSENSOR);
                    indices.Add(index);
                }
                for (int index = 0; index < Sensor.SimpleOrientationSensorList.Count; index++)
                {
                    totalIndex++;
                    AddPivotItem(Sensor.SIMPLEORIENTATIONSENSOR, index, totalIndex);
                    SensorType.Add(Sensor.SIMPLEORIENTATIONSENSOR);
                    indices.Add(0);
                }

                var resourceLoader = ResourceLoader.GetForCurrentView();
                rootPage.NotifyUser(resourceLoader.GetString("NumberOfSensors") + ": " + 
                    pivotSensor.Items.Count + "\nNumber of sensors failed to enumerate: " + Sensor.NumFailedEnumerations, 
                    NotifyType.StatusMessage);
                if (pivotSensor.Items.Count > 0)
                {
                    pivotSensor.SelectionChanged += PivotSensorSelectionChanged;
                    pivotSensor.SelectedIndex = 0;
                    PivotSensorSelectionChanged(null, null);
                }
                else
                {
                    textBlockNoSensor.Text = resourceLoader.GetString("CannotFindSensor");
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }

        private void TestButtonClick(object sender, RoutedEventArgs e)
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            switch (((Button)sender).Content)
            {
                case "Orientation Test":
                    testType = "Orientation";
                    for (int i = 0; i < numQuadrants; i++)
                    {
                        quadrants[i] = 0;
                    }
                    testsCompleted = 0;
                    arrowDir = (int)Directions.nothing;
                    accelerometerInitialized = false;
                    inclinometerInitialized = false;
                    orientationInitialized = false;
                    orientationAmInitialized = false;
                    simpleOrientationInitialized = false;
                    instruction.Text = "Please disable auto-rotation on your device.";
                    startButtonOrientation.Visibility = Visibility.Visible;
                    TestBeginOrientation();
                    break;
                case "Frequency Test":
                    testType = "Frequency";
                    DisplayPrecondition();
                    break;
                case "Offset Test":
                    testType = "Offset";
                    DisplayPrecondition();
                    break;
                case "Jitter Test":
                    testType = "Jitter";
                    DisplayPrecondition();
                    break;
                case "Drift Test":
                    testType = "Drift";
                    if (type == Sensor.GYROMETER) testType = "GyroDrift";
                    DisplayPrecondition();
                    break;
                case "Packet Loss Test":
                    testType = "PacketLoss";
                    DisplayPrecondition();
                    break;
                case "Static Accuracy Test":
                    testType = "StaticAccuracy";
                    DisplayPrecondition();
                    break;
                case "MagInterference Test":
                    testType = "MagInterference";
                    DisplayPrecondition();
                    break;
            }
        }

        private void DisplayPrecondition()
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            switch (testType)
            {
                case "Frequency":
                    instruction.Text = "Put device on a level surface, isolated from outside vibration.\n" +
                                       "Keep it in stationary state.";
                    break;
                case "Offset":
                    if (type == Sensor.LIGHTSENSOR)
                    {
                        instruction.Text = "Put device on a static level surface, with its ambient light sensor covered.\n";
                    }
                    else
                    {
                        instruction.Text = "Put device on a level surface, isolated from outside vibration.\n" +
                                           "Place the screen face up, with top side (Y axis) pointing to the magnetic north.\n" +
                                           "Keep it in stationary state.";
                    }
                    break;
                case "Jitter":
                    if (type == Sensor.LIGHTSENSOR)
                    {
                        instruction.Text = "Put device on a static level surface, with its ambient light sensor covered.\n";
                    }
                    else
                    {
                        instruction.Text = "Put device on a level surface, isolated from outside vibration.\n" +
                                           "Place the screen face up, with top side (Y axis) pointing to the magnetic north.\n" +
                                           "Keep it in stationary state.";
                    }
                    break;
                case "Drift":
                    instruction.Text = "Put device on a level surface, isolated from outside vibration.\n" +
                                       "Keep it in stationary state.";
                    break;
                case "GyroDrift":
                    instruction.Text = "Put device on a level surface, isolated from outside vibration.\n" +
                                       "Keep it in stationary state.";
                    break;
                case "PacketLoss":
                    instruction.Text = "Put device on a level surface, isolated from outside vibration.\n" +
                                       "Keep it in stationary state.";
                    break;
                case "StaticAccuracy":
                    instruction.Text = "Put device on a level surface with its Y axis pointing to the magnet north.\n"+
                                       "Suggest turn rotation lock on.";
                    break;
                case "MagInterference":
                    instruction.Text = "Please move a magnet (~1G) pass the stationary device at a speed of approximately 0.25 m/s .\n" +
                    "You have 30 seconds to do the test.";
                    break;
            }
            pivotSensor.Visibility = Visibility.Collapsed;
            startButton.Visibility = Visibility.Visible;
        }

        private void AddPivotItem(int sensorType, int index, int totalIndex)
        {
            PivotItem pivotItemSensor = new PivotItem();
            ScrollViewer scrollViewerSensor = new ScrollViewer();
            scrollViewerSensor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewerSensor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            StackPanel stackPanel = new StackPanel();
            Button orientationTestButton = CreateTestButton("Orientation Test");
            Button frequencyTestButton = CreateTestButton("Frequency Test");
            Button offsetTestButton = CreateTestButton("Offset Test");
            Button jitterTestButton = CreateTestButton("Jitter Test");
            Button driftTestButton = CreateTestButton("Drift Test");
            Button packetLossTestButton = CreateTestButton("Packet Loss Test");
            Button staticAccuracyButton = CreateTestButton("Static Accuracy Test");
            Button magInterferenceButton = CreateTestButton("MagInterference Test", 28);
            TextBlock noTestAvailable = new TextBlock() { Text = "No tests available for this sensor." };
            pivotItemSensor.Header = Constants.SensorName[sensorType] + " " + (index + 1);
            if (sensorType == Sensor.ACCELEROMETER)
            {
                stackPanel.Children.Add(orientationTestButton);
                stackPanel.Children.Add(frequencyTestButton);
                stackPanel.Children.Add(offsetTestButton);
                stackPanel.Children.Add(jitterTestButton);
                stackPanel.Children.Add(packetLossTestButton);
            }
            else if (sensorType == Sensor.GYROMETER)
            {
                stackPanel.Children.Add(frequencyTestButton);
                stackPanel.Children.Add(offsetTestButton);
                stackPanel.Children.Add(jitterTestButton);
                stackPanel.Children.Add(driftTestButton);
                stackPanel.Children.Add(packetLossTestButton);
            }
            else if (sensorType == Sensor.INCLINOMETER)
            {
                stackPanel.Children.Add(orientationTestButton);
            }
            else if (sensorType == Sensor.LIGHTSENSOR)
            {
                stackPanel.Children.Add(frequencyTestButton);
                stackPanel.Children.Add(offsetTestButton);
                stackPanel.Children.Add(jitterTestButton);
                stackPanel.Children.Add(packetLossTestButton);
            }
            else if (sensorType == Sensor.MAGNETOMETER)
            {
                stackPanel.Children.Add(frequencyTestButton);
                stackPanel.Children.Add(packetLossTestButton);
            }
            else if (sensorType == Sensor.ORIENTATIONSENSOR)
            {
                stackPanel.Children.Add(orientationTestButton);
                stackPanel.Children.Add(frequencyTestButton);
                stackPanel.Children.Add(offsetTestButton);
                stackPanel.Children.Add(jitterTestButton);
                stackPanel.Children.Add(driftTestButton);
                stackPanel.Children.Add(packetLossTestButton);
                stackPanel.Children.Add(staticAccuracyButton);
                stackPanel.Children.Add(magInterferenceButton);
            }
            else if (sensorType == Sensor.ORIENTATIONRELATIVE)
            {
                stackPanel.Children.Add(orientationTestButton);
                stackPanel.Children.Add(frequencyTestButton);
                stackPanel.Children.Add(jitterTestButton);
                stackPanel.Children.Add(driftTestButton);
                stackPanel.Children.Add(packetLossTestButton);
                stackPanel.Children.Add(staticAccuracyButton);
                stackPanel.Children.Add(magInterferenceButton);
            }
            else if (sensorType == Sensor.SIMPLEORIENTATIONSENSOR)
            {
                stackPanel.Children.Add(orientationTestButton);
            }
            else if (sensorType == Sensor.ORIENTATIONGEOMAGNETIC)
            {
                stackPanel.Children.Add(frequencyTestButton);
                stackPanel.Children.Add(offsetTestButton);
                stackPanel.Children.Add(jitterTestButton);
                stackPanel.Children.Add(driftTestButton);
                stackPanel.Children.Add(packetLossTestButton);
                stackPanel.Children.Add(staticAccuracyButton);
                stackPanel.Children.Add(magInterferenceButton);
            }
            else if (sensorType == Sensor.COMPASS)
            {
                stackPanel.Children.Add(frequencyTestButton);
                stackPanel.Children.Add(packetLossTestButton);
            }
            else
            {
                stackPanel.Children.Add(noTestAvailable);
            }
            scrollViewerSensor.Content = stackPanel;
            pivotItemSensor.Content = scrollViewerSensor;
            pivotSensor.Items.Add(pivotItemSensor);
        }

        private Button CreateTestButton(string title, int FontSize = 30)
        {
            Button testButton = new Button();
            testButton.Content = title;
            testButton.HorizontalAlignment = HorizontalAlignment.Center;
            testButton.Click += TestButtonClick;
            testButton.Height = 85;
            testButton.Width = 280;
            testButton.FontSize = FontSize;
            testButton.Margin = new Thickness(10);

            return testButton;
        }

        private void TestBeginOrientation()
        {
            pivotSensor.Visibility = Visibility.Collapsed;
            rootPage.NotifyUser("", NotifyType.StatusMessage);
            IsSimpleOrientationSensor = false;
            int type = SensorType[pivotSensor.SelectedIndex];
            if (type == Sensor.ACCELEROMETER)
            {
                currentAccelerometer = Sensor.AccelerometerStandardList[indices[pivotSensor.SelectedIndex]];
                instruction.Text = Constants.SensorName[type] + " ready\n" + currentAccelerometer.DeviceId;
                currentAccelerometer.ReportInterval = Math.Max(currentAccelerometer.MinimumReportInterval, 500);
                currentAccelerometer.ReadingChanged += AccelerometerReadingChangedOrientation;
            }
            else if (type == Sensor.INCLINOMETER)
            {
                currentInclinometer = Sensor.InclinometerList[indices[pivotSensor.SelectedIndex]];
                instruction.Text = "Inclinometer ready\n" + currentInclinometer.DeviceId;
                currentInclinometer.ReportInterval = Math.Max(currentInclinometer.MinimumReportInterval, 200);
                currentInclinometer.ReadingChanged += InclinometerReadingChangedOrientation;
            }
            else if (type == Sensor.ORIENTATIONSENSOR)
            {
                currentOrientationSensor = Sensor.OrientationAbsoluteList[indices[pivotSensor.SelectedIndex]];
                instruction.Text = "Orientation sensor ready\n" + currentOrientationSensor.DeviceId;
                currentOrientationSensor.ReportInterval = Math.Max(currentOrientationSensor.MinimumReportInterval, 200);
                currentOrientationSensor.ReadingChanged += OrientationReadingChangedOrientation;
            }
            else if (type == Sensor.ORIENTATIONGEOMAGNETIC)
            {
                currentOrientationSensor = Sensor.OrientationGeomagneticList[indices[pivotSensor.SelectedIndex]];
                instruction.Text = "Geomagnetic orientation sensor ready\n" + currentOrientationSensor.DeviceId;
                currentOrientationSensor.ReportInterval = Math.Max(currentOrientationSensor.MinimumReportInterval, 200);
                currentOrientationSensor.ReadingChanged += OrientationReadingChangedOrientation;
            }
            else if (type == Sensor.SIMPLEORIENTATIONSENSOR)
            {
                IsSimpleOrientationSensor = true;
                currentSimpleOrientationSensor = Sensor.SimpleOrientationSensorList[indices[pivotSensor.SelectedIndex]];
                instruction.Text = "Simple orientation sensor ready\n" + currentSimpleOrientationSensor.DeviceId;
                currentSimpleOrientationSensor.OrientationChanged += SimpleOrientationChangedOrientation;
            }
        }

        private void TestBegin()
        {
            staticAccuracyCount = 0;
            dataList = new List<double[]>();
            orientationSensorFirstMinuteDataList = new List<double[]>();
            orientationSensorLastMinuteDataList = new List<double[]>();
            timestampList = new List<DateTime>();
            cancelButtonClicked = false;
            rootPage.NotifyUser("", NotifyType.StatusMessage);
            int count = 0;
            int type = SensorType[pivotSensor.SelectedIndex];
            if (type == Sensor.ACCELEROMETER)
            {
                currentAccelerometer = Sensor.AccelerometerStandardList[indices[pivotSensor.SelectedIndex]];
                currentAccInfo = Sensor.AccelerometerStandardDeviceInfo[indices[pivotSensor.SelectedIndex]];
                // Set to minimum report interval (try 10 times)
                while (currentAccelerometer.ReportInterval != currentAccelerometer.MinimumReportInterval && count < 10)
                {
                    currentAccelerometer.ReportInterval = currentAccelerometer.MinimumReportInterval;
                    count++;
                }
                if (currentAccelerometer.ReportInterval != currentAccelerometer.MinimumReportInterval)
                {
                    rootPage.NotifyUser("Failed to set to the minimum report interval.", NotifyType.ErrorMessage);
                    restartButton.Visibility = Visibility.Visible;
                }
                else
                {
                    rootPage.DisableScenarioSelect();
                    cancelButton.Visibility = Visibility.Visible;
                    if (testType == "Jitter")
                    {
                        accelerometerInitialReading = currentAccelerometer.GetCurrentReading();
                    }
                    countdown = new Countdown(testLength[testType], testType);
                    startTime = DateTime.Now;
                    currentAccelerometer.ReadingChanged += AccelerometerReadingChanged;
                    instruction.Text = "Accelerometer " + testType + " Test in progress...";
                }
            }
            else if (type == Sensor.GYROMETER)
            {
                currentGyrometer = Sensor.GyrometerList[indices[pivotSensor.SelectedIndex]];

                // Set to minimum report interval (try 10 times)
                while (currentGyrometer.ReportInterval != currentGyrometer.MinimumReportInterval && count < 10)
                {
                    currentGyrometer.ReportInterval = currentGyrometer.MinimumReportInterval;
                    count++;
                }
                if (currentGyrometer.ReportInterval != currentGyrometer.MinimumReportInterval)
                {
                    rootPage.NotifyUser("Failed to set to the minimum report interval.", NotifyType.ErrorMessage);
                    restartButton.Visibility = Visibility.Visible;
                }
                else
                {
                    rootPage.DisableScenarioSelect();
                    cancelButton.Visibility = Visibility.Visible;

                    if (testType == "Jitter")
                    {
                        gyrometerInitialReading = currentGyrometer.GetCurrentReading();
                    }
                    countdown = new Countdown(testLength[testType], testType);
                    startTime = DateTime.Now;
                    currentGyrometer.ReadingChanged += GyrometerReadingChanged;
                    instruction.Text = "Gyrometer " + testType + " Test in progress...";
                }
            }
            else if (type == Sensor.LIGHTSENSOR)
            {
                currentLightSensor = Sensor.LightSensorList[indices[pivotSensor.SelectedIndex]];

                // Set to minimum report interval (try 10 times)
                while (currentLightSensor.ReportInterval != currentLightSensor.MinimumReportInterval && count < 10)
                {
                    currentLightSensor.ReportInterval = currentLightSensor.MinimumReportInterval;
                    count++;
                }
                if (currentLightSensor.ReportInterval != currentLightSensor.MinimumReportInterval)
                {
                    rootPage.NotifyUser("Failed to set to the minimum report interval.", NotifyType.ErrorMessage);
                    restartButton.Visibility = Visibility.Visible;
                }
                else
                {
                    rootPage.DisableScenarioSelect();
                    cancelButton.Visibility = Visibility.Visible;
                    if (testType == "Jitter")
                    {
                        lightSensorInitialReading = currentLightSensor.GetCurrentReading();
                    }
                    countdown = new Countdown(testLength[testType], testType);
                    startTime = DateTime.Now;
                    currentLightSensor.ReadingChanged += LightSensorReadingChanged;
                    instruction.Text = "Light Sensor " + testType + " Test in progress...";
                }
            }
            else if (type == Sensor.MAGNETOMETER)
            {
                currentMagnetometer = Sensor.MagnetometerList[indices[pivotSensor.SelectedIndex]];

                // Set to minimum report interval (try 10 times)
                while (currentMagnetometer.ReportInterval != currentMagnetometer.MinimumReportInterval && count < 10)
                {
                    currentMagnetometer.ReportInterval = currentMagnetometer.MinimumReportInterval;
                    count++;
                }
                if (currentMagnetometer.ReportInterval != currentMagnetometer.MinimumReportInterval)
                {
                    rootPage.NotifyUser("Failed to set to the minimum report interval.", NotifyType.ErrorMessage);
                    restartButton.Visibility = Visibility.Visible;
                }
                else
                {
                    rootPage.DisableScenarioSelect();
                    cancelButton.Visibility = Visibility.Visible;
                    countdown = new Countdown(testLength[testType], testType);
                    startTime = DateTime.Now;
                    currentMagnetometer.ReadingChanged += MagnetometerReadingChanged;
                    instruction.Text = "Magnetometer " + testType + " Test in progress...";
                }
            }
            else if (type == Sensor.ORIENTATIONSENSOR)
            {
                currentOrientationSensor = Sensor.OrientationAbsoluteList[indices[pivotSensor.SelectedIndex]];

                // Set to minimum report interval (try 10 times)
                while (currentOrientationSensor.ReportInterval != currentOrientationSensor.MinimumReportInterval && count < 10)
                {
                    currentOrientationSensor.ReportInterval = currentOrientationSensor.MinimumReportInterval;
                    count++;
                }
                if (currentOrientationSensor.ReportInterval != currentOrientationSensor.MinimumReportInterval)
                {
                    rootPage.NotifyUser("Failed to set to the minimum report interval.", NotifyType.ErrorMessage);
                    restartButton.Visibility = Visibility.Visible;
                }
                else if (testType == "StaticAccuracy")
                {
                    StaticAccuracyHandler();
                }
                else
                {
                    rootPage.DisableScenarioSelect();
                    cancelButton.Visibility = Visibility.Visible;
                    if (testType == "Jitter")
                    {
                        orientationSensorInitialReading = currentOrientationSensor.GetCurrentReading();
                    }
                    countdown = new Countdown(testLength[testType], testType);
                    startTime = DateTime.Now;
                    currentOrientationSensor.ReadingChanged += OrientationSensorReadingChanged;
                    instruction.Text = "Orientation Sensor" + " " + testType + " Test in progress...";
                }
            }
            else if (type == Sensor.ORIENTATIONGEOMAGNETIC)
            {
                currentOrientationSensor = Sensor.OrientationGeomagneticList[indices[pivotSensor.SelectedIndex]];
                while (currentOrientationSensor.ReportInterval != currentOrientationSensor.MinimumReportInterval && count < 10)
                {
                    currentOrientationSensor.ReportInterval = currentOrientationSensor.MinimumReportInterval;
                    count++;
                }
                if (currentOrientationSensor.ReportInterval != currentOrientationSensor.MinimumReportInterval)
                {
                    rootPage.NotifyUser("Failed to set to the minimum report interval.", NotifyType.ErrorMessage);
                    restartButton.Visibility = Visibility.Visible;
                }
                else if (testType == "StaticAccuracy")
                {
                    StaticAccuracyHandler();
                }
                else
                {
                    rootPage.DisableScenarioSelect();
                    cancelButton.Visibility = Visibility.Visible;
                    if (testType == "Jitter")
                    {
                        orientationSensorInitialReading = currentOrientationSensor.GetCurrentReading();
                    }
                    countdown = new Countdown(testLength[testType], testType);
                    startTime = DateTime.Now;
                    currentOrientationSensor.ReadingChanged += OrientationSensorReadingChanged;
                    instruction.Text = "Geomagnetic orientation Sensor " + testType + " Test in progress...";
                }
            }
            else if (type == Sensor.ORIENTATIONRELATIVE)
            {
                currentOrientationSensor = Sensor.OrientationRelativeList[indices[pivotSensor.SelectedIndex]];

                // Set to minimum report interval (try 10 times)
                while (currentOrientationSensor.ReportInterval != currentOrientationSensor.MinimumReportInterval && count < 10)
                {
                    currentOrientationSensor.ReportInterval = currentOrientationSensor.MinimumReportInterval;
                    count++;
                }
                if (currentOrientationSensor.ReportInterval != currentOrientationSensor.MinimumReportInterval)
                {
                    rootPage.NotifyUser("Failed to set to the minimum report interval.", NotifyType.ErrorMessage);
                    restartButton.Visibility = Visibility.Visible;
                }
                else if (testType == "StaticAccuracy")
                {
                    StaticAccuracyHandler();
                }
                else
                {
                    rootPage.DisableScenarioSelect();
                    cancelButton.Visibility = Visibility.Visible;
                    if (testType == "Jitter")
                    {
                        orientationSensorInitialReading = currentOrientationSensor.GetCurrentReading();
                    }
                    countdown = new Countdown(testLength[testType], testType);
                    startTime = DateTime.Now;
                    currentOrientationSensor.ReadingChanged += OrientationSensorReadingChanged;
                    instruction.Text = "Relative Orientation Sensor" + " " + testType + " Test in progress...";
                }
            }
            else if (type == Sensor.ORIENTATIONGEOMAGNETIC)
            {
                currentOrientationSensor = Sensor.OrientationGeomagneticList[indices[pivotSensor.SelectedIndex]];
                while (currentOrientationSensor.ReportInterval != currentOrientationSensor.MinimumReportInterval && count < 10)
                {
                    currentOrientationSensor.ReportInterval = currentOrientationSensor.MinimumReportInterval;
                    count++;
                }
                if (currentOrientationSensor.ReportInterval != currentOrientationSensor.MinimumReportInterval)
                {
                    rootPage.NotifyUser("Failed to set to the minimum report interval.", NotifyType.ErrorMessage);
                    restartButton.Visibility = Visibility.Visible;
                }
                else
                {
                    rootPage.DisableScenarioSelect();
                    cancelButton.Visibility = Visibility.Visible;
                    if (testType == "Jitter")
                    {
                        orientationSensorInitialReading = currentOrientationSensor.GetCurrentReading();
                    }
                    countdown = new Countdown(testLength[testType], testType);
                    startTime = DateTime.Now;
                    currentOrientationSensor.ReadingChanged += OrientationSensorReadingChanged;
                    instruction.Text = "Geomagnetic orientation Sensor " + testType + " Test in progress...";
                }
            }
            if (type == Sensor.COMPASS)
            {
                currentCompass = Sensor.CompassList[indices[pivotSensor.SelectedIndex]];

                // Set to minimum report interval (try 10 times)
                while (currentCompass.ReportInterval != currentCompass.MinimumReportInterval && count < 10)
                {
                    currentCompass.ReportInterval = currentCompass.MinimumReportInterval;
                    count++;
                }
                if (currentCompass.ReportInterval != currentCompass.MinimumReportInterval)
                {
                    rootPage.NotifyUser("Failed to set to the minimum report interval.", NotifyType.ErrorMessage);
                    restartButton.Visibility = Visibility.Visible;
                }
                else
                {
                    rootPage.DisableScenarioSelect();
                    cancelButton.Visibility = Visibility.Visible;
                    if (testType == "Jitter")
                    {
                        compassInitialReading = currentCompass.GetCurrentReading();
                    }
                    countdown = new Countdown(testLength[testType], testType);
                    startTime = DateTime.Now;
                    currentCompass.ReadingChanged += CompassReadingChanged;
                    instruction.Text = "Compass " + testType + " Test in progress...";
                }
            }
        }
        public async void TestEnd()
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            if (type == Sensor.ACCELEROMETER)
            {
                currentAccelerometer.ReadingChanged -= AccelerometerReadingChanged;
            }
            else if (type == Sensor.GYROMETER)
            {
                currentGyrometer.ReadingChanged -= GyrometerReadingChanged;
            }
            else if (type == Sensor.LIGHTSENSOR)
            {
                currentLightSensor.ReadingChanged -= LightSensorReadingChanged;
            }
            else if (type == Sensor.MAGNETOMETER)
            {
                currentMagnetometer.ReadingChanged -= MagnetometerReadingChanged;
            }
            else if (type == Sensor.ORIENTATIONSENSOR)
            {
                currentOrientationSensor.ReadingChanged -= OrientationSensorReadingChanged;
            }
            else if (type == Sensor.ORIENTATIONGEOMAGNETIC)
            {
                currentOrientationSensor.ReadingChanged -= OrientationSensorReadingChanged;
            }
            else if (type == Sensor.ORIENTATIONRELATIVE)
            {
                currentOrientationSensor.ReadingChanged -= OrientationSensorReadingChanged;
            }
            else if (type == Sensor.COMPASS)
            {
                currentCompass.ReadingChanged -= CompassReadingChanged;
            }
            cancelButton.Visibility = Visibility.Collapsed;
            instruction.Text = "Calculating result...";
            await Task.Delay(5000);
            output.Text = "";
            LogDataList();

            if (testType == "Frequency")
            {
                CalculateFrequencyTest();
            }
            else if (testType == "Offset")
            {
                CalculateOffsetTest();
            }
            else if (testType == "Jitter")
            {
                CalculateJitterTest();
            }
            else if (testType == "Drift" || testType == "GyroDrift")
            {
                CalculateDriftTest();
            }
            else if (testType == "PacketLoss")
            {
                CalculatePacketLossTest();
            }
            else if (testType == "StaticAccuracy")
            {
                CalculateStaticAccuracy();
            }
            else if (testType == "MagInterference")
            {
                CalculateMagInterference();
            }

            DisplayRestart();
        }

        private void CalculateMagInterference()
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            double result = 0;
            double[] expectedValue = { 1, 0, 0, 0 };
            int globalStep = 0;
            double temp = 0.0, maxE = 0, minE = 180, orientationError = 0;
            foreach (double[] array in dataList)
            {
                if (globalStep < 10)
                {
                    temp += Angle4(array, expectedValue);
                    if (globalStep == 9)
                    {
                        temp /= 10;
                        maxE = minE = temp;
                    }
                }
                else
                {
                    orientationError = Angle4(array, expectedValue);
                    maxE = Math.Max(maxE, orientationError);
                    minE = Math.Min(minE, orientationError);
                }
                globalStep++;
            }
            result = Math.Max(maxE - temp, temp - minE);
            instruction.Text = "The result of Magnetic Interference is\n" + " " + result + " " + "degrees";
        }

        private void CalculateStaticAccuracy()
        {
            oriAngles.Sort();
            oriAngles.Reverse();
            double result = oriAngles[0];
            instruction.Text = "The result of Static Accuracy is\n" + " " + result + " " + "degrees";
        }

        private void LogDataList()
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            for (int i = 0; i < dataList.Count; i++)
            {
                if (type == Sensor.ACCELEROMETER)
                {
                    rootPage.loggingChannelTests.LogMessage(timestampList[i] + ": AccelerationX=" + dataList[i][0] + ", AccelerationY=" + dataList[i][1] + ", AccelerationZ=" + dataList[i][2]);
                }
                else if (type == Sensor.GYROMETER)
                {
                    rootPage.loggingChannelTests.LogMessage(timestampList[i] + ": AngularVelocityX=" + dataList[i][0] + ", AngularVelocityY=" + dataList[i][1] + ", AngularVelocityZ=" + dataList[i][2]);
                }
                else if (type == Sensor.LIGHTSENSOR)
                {
                    rootPage.loggingChannelTests.LogMessage(timestampList[i] + ": IlluminanceInLux" + dataList[i][0]);
                }
                else if (type == Sensor.MAGNETOMETER)
                {
                    rootPage.loggingChannelTests.LogMessage(timestampList[i] + ": MagneticFieldX=" + dataList[i][0] + ", MagneticFieldY=" + dataList[i][1] + ", MagneticFieldZ=" + dataList[i][2]);
                }
                else if (type == Sensor.ORIENTATIONSENSOR)
                {
                    rootPage.loggingChannelTests.LogMessage(timestampList[i] + ": QuaternionW=" + dataList[i][0] + ", QuaternionX=" + dataList[i][1] + ", QuaternionY=" + dataList[i][2] + ", QuaternionZ=" + dataList[i][3]);
                }
            }
        }

        private void DisplayRestart()
        {
            rootPage.EnableScenarioSelect();
            timerLog.Text = "";
            restartButton.Visibility = Visibility.Visible;
            saveButton.Visibility = Visibility.Visible;
            saveButton.IsEnabled = true;
        }

        private void RestartButton(object sender, RoutedEventArgs e)
        {
            restartButton.Visibility = Visibility.Collapsed;
            saveButton.Visibility = Visibility.Collapsed;
            restartButtonOrientation.Visibility = Visibility.Collapsed;
            saveButtonOrientation.Visibility = Visibility.Collapsed;
            instruction.Text = "";
            run.Text = "";
            output.Text = "";
            pivotSensor.Visibility = Visibility.Visible;
            rootPage.NotifyUser("Please select a test", NotifyType.StatusMessage);
        }

        private async void CancelButton(object sender, RoutedEventArgs e)
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            cancelButtonClicked = true;
            if (type == Sensor.ACCELEROMETER)
            {
                currentAccelerometer.ReadingChanged -= AccelerometerReadingChanged;
            }
            else if (type == Sensor.GYROMETER)
            {
                currentGyrometer.ReadingChanged -= GyrometerReadingChanged;
            }
            else if (type == Sensor.LIGHTSENSOR)
            {
                currentLightSensor.ReadingChanged -= LightSensorReadingChanged;
            }
            else if (type == Sensor.MAGNETOMETER)
            {
                currentMagnetometer.ReadingChanged -= MagnetometerReadingChanged;
            }
            else if (type == Sensor.ORIENTATIONSENSOR || type == Sensor.ORIENTATIONGEOMAGNETIC
                    || type == Sensor.ORIENTATIONRELATIVE)
            {
                currentOrientationSensor.ReadingChanged -= OrientationSensorReadingChanged;
            }
            cancelButton.Visibility = Visibility.Collapsed;
            restartButton.Visibility = Visibility.Collapsed;
            rootPage.EnableScenarioSelect();
            timerLog.Text = "";
            instruction.Text = "";
            run.Text = "";
            output.Text = "";
            pivotSensor.Visibility = Visibility.Visible;
            if (countdown != null)
            {
                countdown.Stop();
            }
            rootPage.NotifyUser("Please select a test", NotifyType.StatusMessage);
            await Task.Delay(1000);
            output.Text = "";
        }

        private void CalculateFrequencyTest()
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            string str = Constants.SensorName[type] + " " + testType + " Test Result: " + (dataList.Count / testLength[testType]) + " Hz\n\n";
            rootPage.loggingChannelTests.LogMessage(str);
            hyperlink.NavigateUri = new Uri("https://aka.ms/sensorexplorerblog");
            run.Text = "https://aka.ms/sensorexplorerblog";
            instruction.Text = str + "For more information, please visit:";
        }

        private void CalculateOffsetTest()
        {
            string str = string.Empty;
            int type = SensorType[pivotSensor.SelectedIndex];
            if (type == Sensor.ACCELEROMETER)
            {
                string connectionType = Constants.SensorConnectionTypes[int.Parse(currentAccInfo.Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())];
                double[] errorSum = new double[3]; // x, y, z
                if (connectionType == "Integrated")
                {
                    foreach (double[] array in dataList)
                    {
                        errorSum[0] += Math.Abs(array[0] - Constants.OffsetTestExpectedValue[type][0]);
                        errorSum[1] += Math.Abs(array[1] - Constants.OffsetTestExpectedValue[type][1]);
                        errorSum[2] += Math.Abs(array[2] - Constants.OffsetTestExpectedValue[type][2]);
                    }
                }
                else
                {
                    foreach (double[] array in dataList)
                    {
                        errorSum[0] += Math.Abs(array[0] - Constants.OffsetTestExpectedValue[type][0]);
                        errorSum[1] += Math.Abs(array[1] - Constants.OffsetTestExpectedValue[type][1]);
                        errorSum[2] += Math.Abs(array[2] + Constants.OffsetTestExpectedValue[type][2]);
                    }
                }
                double avgErrorX = errorSum[0] / dataList.Count;
                double avgErrorY = errorSum[1] / dataList.Count;
                double avgErrorZ = errorSum[2] / dataList.Count;
                double result = Math.Sqrt(avgErrorX * avgErrorX + avgErrorY * avgErrorY + avgErrorZ * avgErrorZ);
                str = Constants.SensorName[type] + " " + testType + " Test Result: " + result + " G\n";
            }
            else if (type == Sensor.GYROMETER)
            {
                double[] errorSum = new double[3]; // x, y, z
                foreach (double[] array in dataList)
                {
                    errorSum[0] += Math.Abs(array[0] - Constants.OffsetTestExpectedValue[type][0]);
                    errorSum[1] += Math.Abs(array[1] - Constants.OffsetTestExpectedValue[type][1]);
                    errorSum[2] += Math.Abs(array[2] - Constants.OffsetTestExpectedValue[type][2]);
                }
                double avgErrorX = errorSum[0] / dataList.Count;
                double avgErrorY = errorSum[1] / dataList.Count;
                double avgErrorZ = errorSum[2] / dataList.Count;
                double result = Math.Sqrt(avgErrorX * avgErrorX + avgErrorY * avgErrorY + avgErrorZ * avgErrorZ);
                str = Constants.SensorName[type] + " " + testType + " Test Result: " + result + " Degrees/s\n";
            }
            else if (type == Sensor.LIGHTSENSOR)
            {
                double errorSum = 0;
                foreach (double[] array in dataList)
                {
                    errorSum += Math.Abs(array[0] - Constants.OffsetTestExpectedValue[type][0]);
                }
                double result = errorSum / dataList.Count;
                str = Constants.SensorName[type] + " " + testType + " Test Result: " + (errorSum / dataList.Count) + " Lux\n";
            }
            else if (type == Sensor.ORIENTATIONSENSOR || type == Sensor.ORIENTATIONGEOMAGNETIC)
            {
                double[] errorSum = new double[4];  // w, x, y, z
                foreach (double[] array in dataList)
                {
                    errorSum[0] += Math.Abs(array[0] - Constants.OffsetTestExpectedValue[type][0]);
                    errorSum[1] += Math.Abs(array[1] - Constants.OffsetTestExpectedValue[type][1]);
                    errorSum[2] += Math.Abs(array[2] - Constants.OffsetTestExpectedValue[type][2]);
                    errorSum[3] += Math.Abs(array[3] - Constants.OffsetTestExpectedValue[type][3]);
                }
                double avgErrorW = errorSum[0] / dataList.Count;
                double avgErrorX = errorSum[1] / dataList.Count;
                double avgErrorY = errorSum[2] / dataList.Count;
                double avgErrorZ = errorSum[3] / dataList.Count;
                double result = Math.Sqrt(avgErrorW * avgErrorW + avgErrorX * avgErrorX + avgErrorY * avgErrorY + avgErrorZ * avgErrorZ);
                str = Constants.SensorName[type] + " " + testType + " Test Result: " + result + " Degrees\n\n";
            }
            rootPage.loggingChannelTests.LogMessage(str);
            hyperlink.NavigateUri = new Uri("https://aka.ms/sensorexplorerblog");
            run.Text = "https://aka.ms/sensorexplorerblog";
            instruction.Text = str + "For more information, please visit:";
        }

        private void CalculateJitterTest()
        {
            string str = string.Empty;
            int type = SensorType[pivotSensor.SelectedIndex];
            if (type == Sensor.ACCELEROMETER)
            {
                double[] maxDifference = new double[3];
                foreach (double[] array in dataList)
                {
                    maxDifference[0] = (maxDifference[0] > Math.Abs(array[0] - accelerometerInitialReading.AccelerationX)) ? maxDifference[0] : Math.Abs(array[0] - accelerometerInitialReading.AccelerationX);
                    maxDifference[1] = (maxDifference[1] > Math.Abs(array[1] - accelerometerInitialReading.AccelerationY)) ? maxDifference[1] : Math.Abs(array[1] - accelerometerInitialReading.AccelerationY);
                    maxDifference[2] = (maxDifference[2] > Math.Abs(array[2] - accelerometerInitialReading.AccelerationZ)) ? maxDifference[2] : Math.Abs(array[2] - accelerometerInitialReading.AccelerationZ);
                }
                str = Constants.SensorName[type] + " " + testType + " Test Result: \n" +
                      "--> Maximum difference in X: " + maxDifference[0] + " G\n" +
                      "--> Maximum difference in Y: " + maxDifference[1] + " G\n" +
                      "--> Maximum difference in Z: " + maxDifference[2] + " G\n";
            }
            else if (type == Sensor.GYROMETER)
            {
                double[] maxDifference = new double[3];
                foreach (double[] array in dataList)
                {
                    maxDifference[0] = (maxDifference[0] > Math.Abs(array[0] - gyrometerInitialReading.AngularVelocityX)) ? maxDifference[0] : Math.Abs(array[0] - gyrometerInitialReading.AngularVelocityX);
                    maxDifference[1] = (maxDifference[1] > Math.Abs(array[1] - gyrometerInitialReading.AngularVelocityY)) ? maxDifference[1] : Math.Abs(array[1] - gyrometerInitialReading.AngularVelocityY);
                    maxDifference[2] = (maxDifference[2] > Math.Abs(array[2] - gyrometerInitialReading.AngularVelocityZ)) ? maxDifference[2] : Math.Abs(array[2] - gyrometerInitialReading.AngularVelocityZ);
                }
                str = Constants.SensorName[type] + " " + testType + " Test Result: \n" +
                      "--> Maximum difference in X: " + maxDifference[0] + " Degrees/s\n" +
                      "--> Maximum difference in Y: " + maxDifference[1] + " Degrees/s\n" +
                      "--> Maximum difference in Z: " + maxDifference[2] + " Degrees/s\n";
            }
            else if (type == Sensor.GYROMETER)
            {
                double maxDifference = 0;
                foreach (double[] array in dataList)
                {
                    maxDifference = (maxDifference > Math.Abs(array[0] - lightSensorInitialReading.IlluminanceInLux)) ? maxDifference : Math.Abs(array[0] - lightSensorInitialReading.IlluminanceInLux);
                }
                str = Constants.SensorName[type] + " " + testType + " Test Result: " + maxDifference + " Lux\n";
            }
            else if (type == Sensor.ORIENTATIONSENSOR || type == Sensor.ORIENTATIONGEOMAGNETIC 
                    || type == Sensor.ORIENTATIONRELATIVE)
            {
                double[] maxDifference = new double[4];
                foreach (double[] array in dataList)
                {
                    maxDifference[0] = (maxDifference[0] > Math.Abs(array[0] - orientationSensorInitialReading.Quaternion.W)) ? 
                        maxDifference[0] : Math.Abs(array[0] - orientationSensorInitialReading.Quaternion.W);
                    maxDifference[1] = (maxDifference[1] > Math.Abs(array[1] - orientationSensorInitialReading.Quaternion.X)) ? 
                        maxDifference[1] : Math.Abs(array[1] - orientationSensorInitialReading.Quaternion.X);
                    maxDifference[2] = (maxDifference[2] > Math.Abs(array[2] - orientationSensorInitialReading.Quaternion.Y)) ? 
                        maxDifference[2] : Math.Abs(array[2] - orientationSensorInitialReading.Quaternion.Y);
                    maxDifference[3] = (maxDifference[3] > Math.Abs(array[3] - orientationSensorInitialReading.Quaternion.Z)) ? 
                        maxDifference[3] : Math.Abs(array[3] - orientationSensorInitialReading.Quaternion.Z);
                }
                str = Constants.SensorName[type] + " " + testType + " Test Result: \n" +
                      "--> Maximum difference in W: " + maxDifference[0] + " Degrees\n" +
                      "--> Maximum difference in X: " + maxDifference[1] + " Degrees\n" +
                      "--> Maximum difference in Y: " + maxDifference[2] + " Degrees\n" +
                      "--> Maximum difference in Z: " + maxDifference[3] + " Degrees \n\n";
            }
            rootPage.loggingChannelTests.LogMessage(str);
            hyperlink.NavigateUri = new Uri("https://aka.ms/sensorexplorerblog");
            run.Text = "https://aka.ms/sensorexplorerblog";
            instruction.Text = str + "For more information, please visit:";
        }

        private void CalculateDriftTest()
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            string str = string.Empty;

            if (type == Sensor.GYROMETER)
            {
                double[] drift = new double[3];
                for (int i = 0; i < dataList.Count - 1; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        drift[j] = dataList[i][j] * (timestampList[i + 1].Millisecond - timestampList[i].Millisecond)/10;
                    }
                }
                str = Constants.SensorName[type] + " " + testType + " Test Result: \n" +
                      "--> Difference in X: " + (drift[0]) + " Degrees\n" +
                      "--> Difference in Y: " + (drift[1]) + " Degrees\n" +
                      "--> Difference in Z: " + (drift[2]) + " Degrees\n"+
                      "--> Total: " + (Math.Sqrt(drift[1] * drift[1] + drift[2] * drift[2] + drift[0] * drift[0])) + " Degrees\n";

                rootPage.loggingChannelTests.LogMessage(str);
                instruction.Text = str + "For more information, please visit https://aka.ms/sensorexplorerblog";

            }
            else
            {
                double[] firstMinuteSum = new double[4];  // w, x, y, z
                double[] lastMinuteSum = new double[4];  // w, x, y, z
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (timestampList[i].Subtract(startTime) <= TimeSpan.FromMinutes(1))
                    {
                        orientationSensorFirstMinuteDataList.Add(dataList[i]);
                    }
                    else if (timestampList[i].Subtract(startTime) >= TimeSpan.FromMinutes(14))
                    {
                        orientationSensorLastMinuteDataList.Add(dataList[i]);
                    }
                }

                foreach (double[] array in orientationSensorFirstMinuteDataList)
                {
                    firstMinuteSum[0] += array[0];
                    firstMinuteSum[1] += array[1];
                    firstMinuteSum[2] += array[2];
                    firstMinuteSum[3] += array[3];
                }

                foreach (double[] array in orientationSensorLastMinuteDataList)
                {
                    lastMinuteSum[0] += array[0];
                    lastMinuteSum[1] += array[1];
                    lastMinuteSum[2] += array[2];
                    lastMinuteSum[3] += array[3];
                }
                double[] firstMinuteAvg = new double[4];  // w, x, y, z
                double[] lastMinuteAvg = new double[4];  // w, x, y, z
                firstMinuteAvg[0] = firstMinuteSum[0] / orientationSensorFirstMinuteDataList.Count;
                firstMinuteAvg[1] = firstMinuteSum[1] / orientationSensorFirstMinuteDataList.Count;
                firstMinuteAvg[2] = firstMinuteSum[2] / orientationSensorFirstMinuteDataList.Count;
                firstMinuteAvg[3] = firstMinuteSum[3] / orientationSensorFirstMinuteDataList.Count;

                lastMinuteAvg[0] = lastMinuteSum[0] / orientationSensorLastMinuteDataList.Count;
                lastMinuteAvg[1] = lastMinuteSum[1] / orientationSensorLastMinuteDataList.Count;
                lastMinuteAvg[2] = lastMinuteSum[2] / orientationSensorLastMinuteDataList.Count;
                lastMinuteAvg[3] = lastMinuteSum[3] / orientationSensorLastMinuteDataList.Count;

                str = Constants.SensorName[type] + " " + testType + " Test Result: \n" +
                      "--> Difference in W: " + (lastMinuteAvg[0] - firstMinuteAvg[0]) + " Degrees\n" +
                      "--> Difference in X: " + (lastMinuteAvg[1] - firstMinuteAvg[1]) + " Degrees\n" +
                      "--> Difference in Y: " + (lastMinuteAvg[2] - firstMinuteAvg[2]) + " Degrees\n" +
                      "--> Difference in Z: " + (lastMinuteAvg[3] - firstMinuteAvg[3]) + " Degrees\n";

                rootPage.loggingChannelTests.LogMessage(str);
                instruction.Text = str + "For more information, please visit https://aka.ms/sensorexplorerblog";
            }
        }

        private void CalculatePacketLossTest()
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            uint reportInterval = 0;
            if (type == Sensor.ACCELEROMETER)
            {
                reportInterval = currentAccelerometer.ReportInterval;
            }
            if (type == Sensor.COMPASS)
            {
                reportInterval = currentCompass.ReportInterval;
            }
            else if (type == Sensor.GYROMETER)
            {
                reportInterval = currentGyrometer.ReportInterval;
            }
            else if (type == Sensor.LIGHTSENSOR)
            {
                reportInterval = currentLightSensor.ReportInterval;
            }
            else if (type == Sensor.MAGNETOMETER)
            {
                reportInterval = currentMagnetometer.ReportInterval;
            }
            else if (type == Sensor.ORIENTATIONSENSOR || type == Sensor.ORIENTATIONGEOMAGNETIC)
            {
                reportInterval = currentOrientationSensor.ReportInterval;
            }
            if (type == Sensor.ORIENTATIONGEOMAGNETIC || type == Sensor.ORIENTATIONRELATIVE)
            {
                reportInterval = currentOrientationSensor.ReportInterval;
            }
            double expectedNumData = testLength[testType] / (reportInterval / 1000.0);
            string str = Constants.SensorName[type] + " " + testType + " Test Result: " + ((expectedNumData - dataList.Count) / expectedNumData) * 100 + " %\n\n";
            rootPage.loggingChannelTests.LogMessage(str);
            hyperlink.NavigateUri = new Uri("https://aka.ms/sensorexplorerblog");
            run.Text = "https://aka.ms/sensorexplorerblog";
            instruction.Text = str + "For more information, please visit:";
        }

        private async void AccelerometerReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            if (e.Reading.Timestamp.Subtract(startTime) <= TimeSpan.FromSeconds(testLength[testType]))
            {
                timestampList.Add(e.Reading.Timestamp.DateTime);
                dataList.Add(new double[] { e.Reading.AccelerationX, e.Reading.AccelerationY, e.Reading.AccelerationZ });
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    output.Text = e.Reading.Timestamp + ": x=" + e.Reading.AccelerationX + ", y=" + e.Reading.AccelerationY + ", z=" + e.Reading.AccelerationZ;
                });
            }
        }
        private async void CompassReadingChanged(object sender, CompassReadingChangedEventArgs e)
        {
            if (e.Reading.Timestamp.Subtract(startTime) <= TimeSpan.FromSeconds(testLength[testType]))
            {
                timestampList.Add(e.Reading.Timestamp.DateTime);
                dataList.Add(new double[] { e.Reading.HeadingMagneticNorth});
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    output.Text = e.Reading.Timestamp + ": Heading=" + e.Reading.HeadingMagneticNorth;
                });
            }
        }

        private async void GyrometerReadingChanged(object sender, GyrometerReadingChangedEventArgs e)
        {
            if (e.Reading.Timestamp.Subtract(startTime) <= TimeSpan.FromSeconds(testLength[testType]))
            {
                timestampList.Add(e.Reading.Timestamp.DateTime);
                dataList.Add(new double[] { e.Reading.AngularVelocityX, e.Reading.AngularVelocityY, e.Reading.AngularVelocityZ });
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    output.Text = e.Reading.Timestamp + ": x=" + e.Reading.AngularVelocityX + ", y=" + e.Reading.AngularVelocityY + ", z=" + e.Reading.AngularVelocityZ;
                });
            }
        }

        private async void LightSensorReadingChanged(object sender, LightSensorReadingChangedEventArgs e)
        {
            if (e.Reading.Timestamp.Subtract(startTime) <= TimeSpan.FromSeconds(testLength[testType]))
            {
                timestampList.Add(e.Reading.Timestamp.DateTime);
                dataList.Add(new double[] { e.Reading.IlluminanceInLux });
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    output.Text = e.Reading.Timestamp + ": " + e.Reading.IlluminanceInLux;
                });
            }
        }

        private async void MagnetometerReadingChanged(object sender, MagnetometerReadingChangedEventArgs e)
        {
            if (e.Reading.Timestamp.Subtract(startTime) <= TimeSpan.FromSeconds(testLength[testType]))
            {
                timestampList.Add(e.Reading.Timestamp.DateTime);
                dataList.Add(new double[] { e.Reading.MagneticFieldX, e.Reading.MagneticFieldY, e.Reading.MagneticFieldZ });
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    output.Text = e.Reading.Timestamp + ": x=" + e.Reading.MagneticFieldX + ", y=" + e.Reading.MagneticFieldY + ", z=" + e.Reading.MagneticFieldZ;
                });
            }
        }

        private async void OrientationSensorReadingChanged(object sender, OrientationSensorReadingChangedEventArgs e)
        {
            if (e.Reading.Timestamp.Subtract(startTime) <= TimeSpan.FromSeconds(testLength[testType]))
            {
                timestampList.Add(e.Reading.Timestamp.DateTime);
                dataList.Add(new double[] { e.Reading.Quaternion.W, e.Reading.Quaternion.X, e.Reading.Quaternion.Y, e.Reading.Quaternion.Z });
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    output.Text = e.Reading.Timestamp + ": w=" + e.Reading.Quaternion.W + ", x=" + e.Reading.Quaternion.X + ", y=" + e.Reading.Quaternion.Y + ", z=" + e.Reading.Quaternion.Z;
                });
            }
        }

        private void PivotSensorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < pivotSensor.Items.Count; i++)
            {
                if (i != pivotSensor.SelectedIndex)
                {
                    (((pivotSensor.Items[i] as PivotItem).Content as ScrollViewer).Content as StackPanel).Visibility = Visibility.Collapsed;
                }
                else
                {
                    (((pivotSensor.Items[i] as PivotItem).Content as ScrollViewer).Content as StackPanel).Visibility = Visibility.Visible;
                }
            }
        }

        private async void FeedbackButton(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"feedback-hub:?
                                referrer=SensorExplorer&
                                tabid=2&
                                contextid=73&
                                appid=Microsoft.SensorExplorer1_8wekyb3d8bbwe!App&
                                newFeedback=true");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            if (!success)
            {
                rootPage.NotifyUser("Cannot launch Feedback Hub", NotifyType.ErrorMessage);
            }
        }

        private async void SaveFileButtonClick(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("ETL", new List<string>() { ".etl" });
            savePicker.SuggestedFileName = "SensorExplorerLog";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                StorageFile logFileGenerated = await rootPage.loggingSessionTests.CloseAndSaveToFileAsync();
                await logFileGenerated.CopyAndReplaceAsync(file);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    rootPage.NotifyUser("File " + file.Name + " was saved.", NotifyType.StatusMessage);
                }
                else if (status == FileUpdateStatus.CompleteAndRenamed)
                {
                    rootPage.NotifyUser("File " + file.Name + " was renamed and saved.", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("File " + file.Name + " couldn't be saved.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("Operation cancelled.", NotifyType.ErrorMessage);
            }

            saveButton.IsEnabled = false;
            saveButtonOrientation.IsEnabled = false;

            // start a new loging session
            rootPage.loggingSessionTests = new FileLoggingSession("SensorExplorerLogTestsNew");
            rootPage.loggingSessionTests.AddLoggingChannel(rootPage.loggingChannelTests);
        }

        public void DisplayCountdown(int remainingTime)
        {
            if (remainingTime > 60)
            {
                timerLog.Text = "Time remaining: " + (remainingTime / 60) + " minutes " + (remainingTime % 60) + " seconds";
            }
            else
            {
                timerLog.Text = "Time remaining: " + remainingTime + " seconds";
            }
        }

        private void LogProperties(DeviceInformation deviceInfo, string deviceId, string minReportInterval, string reportInterval, string eventName)
        {
            LoggingFields loggingFields = new LoggingFields();
            try
            {
                loggingFields.AddString("Sensor_Type", Constants.SensorTypes[deviceInfo.Properties[Constants.Properties["Sensor_Type"]].ToString()]);
            }
            catch { }
            loggingFields.AddString("Sensor_Device_ID", deviceId);
            try
            {
                loggingFields.AddString("Sensor_Category", Constants.SensorCategories[deviceInfo.Properties[Constants.Properties["Sensor_Category"]].ToString()]);
            }
            catch { }
            try
            {
                loggingFields.AddString("Sensor_PersistentUniqueId", deviceInfo.Properties[Constants.Properties["Sensor_PersistentUniqueId"]].ToString());
            }
            catch { }
            try
            {
                loggingFields.AddString("Sensor_Manufacturer", deviceInfo.Properties[Constants.Properties["Sensor_Manufacturer"]].ToString());
            }
            catch { }
            try
            {
                loggingFields.AddString("Sensor_Model", deviceInfo.Properties[Constants.Properties["Sensor_Model"]].ToString());
            }
            catch { }
            try
            {
                loggingFields.AddString("Sensor_ConnectionType", Constants.SensorConnectionTypes[int.Parse(deviceInfo.Properties[Constants.Properties["Sensor_ConnectionType"]].ToString())]);
            }
            catch { }
            try
            {
                loggingFields.AddString("Sensor_Name", deviceInfo.Properties[Constants.Properties["Sensor_Name"]].ToString());
            }
            catch { }
            loggingFields.AddString("Sensor_Min_Report_Interval", minReportInterval);
            loggingFields.AddString("Sensor_Report_Interval", reportInterval);

            rootPage.loggingChannelTests.LogEvent(eventName, loggingFields);
        }

        private void LogTestSuccess(LoggingFields loggingFields, int testNumber, string direction, string eventName)
        {
            loggingFields.AddInt64("Test Number", testNumber);
            loggingFields.AddString("Arrow Direction", direction);
            loggingFields.AddString("Test Result", "Success");
            rootPage.loggingChannelTests.LogEvent(eventName, loggingFields);
        }

        private void LogTestFailure(int testNumber, string direction, string eventName)
        {
            LoggingFields loggingFields = new LoggingFields();
            loggingFields.AddInt64("Test Number", testNumber);
            loggingFields.AddString("Arrow Direction", direction);
            loggingFields.AddString("Reading", sensorDataLog);
            rootPage.loggingChannelTests.LogEvent(eventName, loggingFields);
        }

        private LoggingFields LogAccelerometerReading(AccelerometerReading reading)
        {
            LoggingFields loggingFields = new LoggingFields();
            loggingFields.AddString("Timestamp", reading.Timestamp.ToString());
            loggingFields.AddDouble("Acceleration X", reading.AccelerationX);
            loggingFields.AddDouble("Acceleration Y", reading.AccelerationY);
            loggingFields.AddDouble("Acceleration Z", reading.AccelerationZ);

            return loggingFields;
        }

        private LoggingFields LogInclinometerReading(InclinometerReading reading)
        {
            LoggingFields loggingFields = new LoggingFields();
            loggingFields.AddString("Timestamp", reading.Timestamp.ToString());
            loggingFields.AddDouble("Pitch (Rotation about X-axis)", reading.PitchDegrees);
            loggingFields.AddDouble("Roll (Rotation about Y-axis)", reading.RollDegrees);
            loggingFields.AddDouble("Yaw (Rotation about Z-axis)", reading.YawDegrees);

            return loggingFields;
        }

        private LoggingFields LogOrientationSensorReading(OrientationSensorReading reading)
        {
            LoggingFields loggingFields = new LoggingFields();
            loggingFields.AddString("Timestamp", reading.Timestamp.ToString());
            loggingFields.AddString("Quaternion",
                              "[x = " + reading.Quaternion.X +
                              ", y = " + reading.Quaternion.Y +
                              ", z = " + reading.Quaternion.Z +
                              ", w = " + reading.Quaternion.W + "]");
            loggingFields.AddString("Rotation Matrix",
                              "[M11 = " + reading.RotationMatrix.M11 +
                              ", M12 = " + reading.RotationMatrix.M12 +
                              ", M13 = " + reading.RotationMatrix.M13 +
                              ", M21 = " + reading.RotationMatrix.M21 +
                              ", M22 = " + reading.RotationMatrix.M22 +
                              ", M23 = " + reading.RotationMatrix.M23 +
                              ", M31 = " + reading.RotationMatrix.M31 +
                              ", M32 = " + reading.RotationMatrix.M32 +
                              ", M33 = " + reading.RotationMatrix.M33 + "]");

            return loggingFields;
        }

        private LoggingFields LogSimpleOrientationSensorReading(SimpleOrientation orientation)
        {
            LoggingFields loggingFields = new LoggingFields();
            loggingFields.AddString("SimpleOrientation", orientation.ToString());

            return loggingFields;
        }

        private async void AccelerometerReadingChangedOrientation(object sender, AccelerometerReadingChangedEventArgs e)
        {
            currentAccelerometer.ReadingChanged -= AccelerometerReadingChangedOrientation;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (SensorType[pivotSensor.SelectedIndex] == Sensor.ACCELEROMETER)
                {
                    sensorDataLog = e.Reading.Timestamp.ToString() +
                                     " x = " + e.Reading.AccelerationX +
                                     ", y = " + e.Reading.AccelerationY +
                                     ", z = " + e.Reading.AccelerationZ;
                    rootPage.NotifyUser(sensorDataLog, NotifyType.StatusMessage);

                    if (!accelerometerInitialized)
                    {
                        accelerometerInitialized = true;
                        LoggingFields loggingFields = LogAccelerometerReading(e.Reading);
                        rootPage.loggingChannelTests.LogEvent("AccelerometerInitialized", loggingFields);
                    }
                    else if ((e.Reading.AccelerationX < -0.9 && arrowDir == (int)Directions.left) ||
                             (e.Reading.AccelerationX > 0.9 && arrowDir == (int)Directions.right) ||
                             (e.Reading.AccelerationY > 0.9 && arrowDir == (int)Directions.up) ||
                             (e.Reading.AccelerationY < -0.9 && arrowDir == (int)Directions.down))
                    {
                        arrowDir = (int)Directions.nothing;
                        testsCompleted++;
                        LoggingFields loggingFields = LogAccelerometerReading(e.Reading);
                        LogTestSuccess(loggingFields, testsCompleted, Enum.GetName(typeof(Directions), arrowDir), "AccelerometerSingleTestResult");
                        TestSuccess();
                    }
                    else
                    {
                        currentAccelerometer.ReadingChanged += AccelerometerReadingChangedOrientation;
                    }
                }
            });
        }

        private async void InclinometerReadingChangedOrientation(object sender, InclinometerReadingChangedEventArgs e)
        {
            currentInclinometer.ReadingChanged -= InclinometerReadingChangedOrientation;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (SensorType[pivotSensor.SelectedIndex] == Sensor.INCLINOMETER)
                {
                    sensorDataLog = e.Reading.Timestamp.ToString() +
                                     " x = " + e.Reading.PitchDegrees +
                                     ", y = " + e.Reading.RollDegrees +
                                     ", z = " + e.Reading.YawDegrees;
                    rootPage.NotifyUser(sensorDataLog, NotifyType.StatusMessage);

                    if (!inclinometerInitialized)
                    {
                        inclinometerInitialized = true;
                        LoggingFields loggingFields = LogInclinometerReading(e.Reading);
                        rootPage.loggingChannelTests.LogEvent("InclinometerInitialized", loggingFields);
                    }
                    else if ((e.Reading.RollDegrees > 80 && e.Reading.RollDegrees < 100 && arrowDir == (int)Directions.right) ||
                             (e.Reading.RollDegrees > -100 && e.Reading.RollDegrees < -80 && arrowDir == (int)Directions.left) ||
                             (e.Reading.PitchDegrees > -100 && e.Reading.PitchDegrees < -80 && arrowDir == (int)Directions.up) ||
                             (e.Reading.PitchDegrees > 80 && e.Reading.PitchDegrees < 100 && arrowDir == (int)Directions.down))
                    {
                        arrowDir = (int)Directions.nothing;
                        testsCompleted++;
                        LoggingFields loggingFields = LogInclinometerReading(e.Reading);
                        LogTestSuccess(loggingFields, testsCompleted, Enum.GetName(typeof(Directions), arrowDir), "InclinometerSingleTestResult");
                        TestSuccess();
                    }
                    else
                    {
                        currentInclinometer.ReadingChanged += InclinometerReadingChangedOrientation;
                    }
                }
            });
        }

        private async void OrientationReadingChangedOrientation(object sender, OrientationSensorReadingChangedEventArgs e)
        {
            currentOrientationSensor.ReadingChanged -= OrientationReadingChangedOrientation;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (SensorType[pivotSensor.SelectedIndex] == Sensor.ORIENTATIONSENSOR 
                || SensorType[pivotSensor.SelectedIndex] == Sensor.ORIENTATIONGEOMAGNETIC 
                || SensorType[pivotSensor.SelectedIndex] == Sensor.ORIENTATIONRELATIVE)
                {
                    sensorDataLog = e.Reading.Timestamp.ToString() +
                                     " x = " + e.Reading.Quaternion.X +
                                     ", y = " + e.Reading.Quaternion.Y +
                                     ", z = " + e.Reading.Quaternion.Z +
                                     ", w = " + e.Reading.Quaternion.W;
                    sensorDataLog += ", M11 = " + e.Reading.RotationMatrix.M11 +
                                      ", M12 = " + e.Reading.RotationMatrix.M12 +
                                      ", M13 = " + e.Reading.RotationMatrix.M13 +
                                      ", M21 = " + e.Reading.RotationMatrix.M21 +
                                      ", M22 = " + e.Reading.RotationMatrix.M22 +
                                      ", M23 = " + e.Reading.RotationMatrix.M23 +
                                      ", M31 = " + e.Reading.RotationMatrix.M31 +
                                      ", M32 = " + e.Reading.RotationMatrix.M32 +
                                      ", M33 = " + e.Reading.RotationMatrix.M33;
                    rootPage.NotifyUser(sensorDataLog, NotifyType.StatusMessage);

                    if (!orientationInitialized && !orientationAmInitialized)
                    {
                        orientationInitialized = true;
                        orientationAmInitialized = true;
                        LoggingFields loggingFields = LogOrientationSensorReading(e.Reading);
                        rootPage.loggingChannelTests.LogEvent("OrientationSensorInitialized", loggingFields);
                    }
                    else
                    {
                        double[] eulerAngle = QuaternionToEulerAngle(e.Reading.Quaternion);
                        if ((eulerAngle[0] > (Math.PI / 2 - 0.2) && eulerAngle[0] < (Math.PI / 2 + 0.2) && arrowDir == (int)Directions.right) ||
                            (eulerAngle[0] > (-1) * (Math.PI / 2 + 0.2) && eulerAngle[0] < (-1) * (Math.PI / 2 - 0.2) && arrowDir == (int)Directions.left) ||
                            (eulerAngle[1] > (-1) * (Math.PI / 2 + 0.2) && eulerAngle[1] < (-1) * (Math.PI / 2 - 0.2) && arrowDir == (int)Directions.up) ||
                            (eulerAngle[1] > (Math.PI / 2 - 0.2) && eulerAngle[1] < (Math.PI / 2 + 0.2) && arrowDir == (int)Directions.down))
                        {
                            arrowDir = (int)Directions.nothing;
                            testsCompleted++;
                            LoggingFields loggingFields = LogOrientationSensorReading(e.Reading);
                            LogTestSuccess(loggingFields, testsCompleted, Enum.GetName(typeof(Directions), arrowDir), "OrientationSensorSingleTestResult");
                            TestSuccess();

                        }
                        else
                        {
                            currentOrientationSensor.ReadingChanged += OrientationReadingChangedOrientation;
                        }
                    }
                }
            });
        }

        private double[] QuaternionToEulerAngle(SensorQuaternion quaternion)
        {
            double[] eulerAngle = new double[3];    // [pitch, roll, yaw]
            // pitch (x-axis rotation)
            double sinp = +2.0 * (quaternion.W * quaternion.Y - quaternion.Z * quaternion.X);
            if (Math.Abs(sinp) >= 1)
            {
                eulerAngle[0] = (sinp > 0) ? (Math.PI / 2) : eulerAngle[0] = (-1.0) * Math.PI / 2;
            }
            else
            {
                eulerAngle[0] = Math.Asin(sinp);    // use 90 degrees if out of range
            }
            // roll (y-axis rotation)
            double sinr = 2.0 * (quaternion.W * quaternion.X + quaternion.Y * quaternion.Z);
            double cosr = 1.0 - 2.0 * (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y);
            eulerAngle[1] = Math.Atan2(sinr, cosr);
            // yaw (z-axis rotation)
            double siny = 2.0 * (quaternion.W * quaternion.Z + quaternion.X * quaternion.Y);
            double cosy = 1.0 - 2.0 * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z);
            eulerAngle[2] = Math.Atan2(siny, cosy);
            return eulerAngle;
        }

        private double InnerProduct(double[] quaternionA, double[] quaternionB)
        {
            double inner = 0;
            inner += quaternionA[0] * quaternionB[0];
            inner += quaternionA[1] * quaternionB[1];
            inner += quaternionA[2] * quaternionB[2];
            inner += quaternionA[3] * quaternionB[3];
            return inner;
        }

        private double Norm(double[] quaternion)
        {
            double sumOfSquares = 0;
            sumOfSquares += quaternion[0] * quaternion[0];
            sumOfSquares += quaternion[1] * quaternion[1];
            sumOfSquares += quaternion[2] * quaternion[2];
            sumOfSquares += quaternion[3] * quaternion[3];
            return Math.Sqrt(sumOfSquares);
        }

        private double Angle4(double[] current, double[] expected)
        {
            double value = InnerProduct(current, expected) / (Norm(current)) * (Norm(expected));
            return 2 * Math.Acos(value) * 180 / Math.PI;
        }

        private double OriAngleCalculate(int index)
        {
            int type = SensorType[pivotSensor.SelectedIndex];
            double angleSum = 0;
            double result = 0;
            double[][] allExpectedValues = { new double[] { 1, 0, 0, 0 }, new double[] { 0.707106781, 0, 0, 0.707106781},new double[]{ 0,0,0,-1 },
                                             new double[] {0.707106781,0, 0,-0.707106781 },new double[] { 1, 0, 0, 0 },//Z is over.
                                             new double[] { 1, 0, 0, 0 }, new double[] { 0.707106781, 0.707106781, 0, 0 }, new double[]{ 0,-1, 0, 0 },
                                             new double[]{ 0.707106781 , - 0.707106781, 0, 0 }, new double[] { 1, 0, 0, 0 },//x is over.
                                             new double[] { 1, 0, 0, 0 }, new double[] { 0.707106781, 0, 0.707106781, 0}, new double[]{ 0 , 0, -1, 0 },
                                             new double[] {  0.707106781 , 0, -0.707106781, 0}, new double[]{ 1, 0, 0, 0 }//y is over.
                                            };
            double[] expectedValue = allExpectedValues[index];
            foreach (double[] array in dataList)
            {
                angleSum += Angle4(array, expectedValue);
            }
            result = angleSum / dataList.Count;
            if (result > 180) result = 360 - Math.Abs(result);
            if (Math.Abs(result) >= 30 && Math.Abs(result) <= 60) result = Math.Abs(45 - result);
            return result;
        }

        private async void StaticAccuracyHandler()
        {
            cancelButton.Visibility = Visibility.Visible;
            oriAngles = new List<double>();

            for (int i = 0; i < 15; i++)
            {
                currentOrientationSensor.ReadingChanged -= OrientationSensorReadingChanged;
                dataList.Clear();
                if (cancelButtonClicked)
                {
                    currentOrientationSensor.ReadingChanged -= OrientationSensorReadingChanged;
                    break;
                }
                for (int count = 10; count >= 0; count--)
                {
                    output.Text = "";
                    if (i == 0 || i == 5 || i == 10 || i == 14)
                    {
                        instruction.Text = "You have " + count +
                            " seconds to rotate the device on a level surface with its y axis pointing to magnetic north...";
                    }
                    else if (i < 5)
                    {
                        instruction.Text = "You have " + count +
                            " seconds to rotate 90 degrees counterclockwise around the z axis and stop to a static position...";
                    }
                    else if (i < 10)
                    {
                        instruction.Text = "You have " + count +
                            " seconds to rotate 90 degrees counterclockwise around the x axis and stop to a static position...";
                    }
                    else
                    {
                        instruction.Text = "You have " + count +
                            " seconds to rotate 90 degrees counterclockwise around the y axis and stop to a static position...";
                    }

                    await Task.Delay(1000);

                    if (cancelButtonClicked)
                    {
                        currentOrientationSensor.ReadingChanged -= OrientationSensorReadingChanged;
                        timerLog.Text = "";
                        instruction.Text = "";
                        output.Text = "";
                        break;
                    }
                }

                startTime = DateTime.Now;
                currentOrientationSensor.ReadingChanged += OrientationSensorReadingChanged;

                for (int count = 5; count >= 0; count--)
                {
                    instruction.Text = "Sampling data for " + count + " seconds...";

                    if (cancelButtonClicked)
                    {
                        currentOrientationSensor.ReadingChanged -= OrientationSensorReadingChanged;
                        timerLog.Text = "";
                        instruction.Text = "";
                        output.Text = "";
                        break;
                    }

                    await Task.Delay(1000);
                }

                oriAngles.Add(OriAngleCalculate(i));
            }

            TestEnd();
        }

        private async void SimpleOrientationChangedOrientation(object sender, SimpleOrientationSensorOrientationChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (SensorType[pivotSensor.SelectedIndex] == Sensor.SIMPLEORIENTATIONSENSOR)
                {
                    sensorDataLog = e.Timestamp.ToString() + " " + e.Orientation.ToString();
                    rootPage.NotifyUser(sensorDataLog, NotifyType.StatusMessage);

                    if (!simpleOrientationInitialized)
                    {
                        simpleOrientationInitialized = true;
                        LoggingFields loggingFields = LogSimpleOrientationSensorReading(e.Orientation);
                        rootPage.loggingChannelTests.LogEvent("SimpleOrientationSensorInitialized", loggingFields);
                    }
                }
            });
        }

        public void CurrentSimpleOrientation()
        {
            SimpleOrientation orientation = currentSimpleOrientationSensor.GetCurrentOrientation();
            if ((orientation == SimpleOrientation.Rotated90DegreesCounterclockwise && arrowDir == (int)Directions.left) ||
                (orientation == SimpleOrientation.Rotated270DegreesCounterclockwise && arrowDir == (int)Directions.right) ||
                (orientation == SimpleOrientation.Faceup && arrowDir == (int)Directions.up) ||
                (orientation == SimpleOrientation.Facedown && arrowDir == (int)Directions.down))
            {
                arrowDir = (int)Directions.nothing;
                currentSimpleOrientationSensor.OrientationChanged -= SimpleOrientationChangedOrientation;
                testsCompleted++;
                LoggingFields loggingFields = LogSimpleOrientationSensorReading(orientation);
                LogTestSuccess(loggingFields, testsCompleted, Enum.GetName(typeof(Directions), arrowDir), "SimpleOrientationSensorSingleTestResult");
                TestSuccess();
            }
        }

        private void StartButton(object sender, RoutedEventArgs e)
        {
            startButton.Visibility = Visibility.Collapsed;
            feedbackButton.Visibility = Visibility.Collapsed;
            TestBegin();
        }

        private void StartButtonOrientation(object sender, RoutedEventArgs e)
        {
            startButtonOrientation.Visibility = Visibility.Collapsed;
            rootPage.DisableScenarioSelect();
            instruction.Text = "Please orient your device so that the arrow is pointing down to the ground";
            feedbackButton.Visibility = Visibility.Collapsed;
            Refresh();
        }

        // A single orientation test success
        private async void TestSuccess()
        {
            countdown.Stop();
            HideArrows();
            Hide();
            // Display green checkmark for 2 sec
            imgCheckmark.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            imgCheckmark.Visibility = Visibility.Collapsed;
            Refresh();
        }

        // All orientation tests success
        private void Success()
        {
            rootPage.loggingChannelTests.LogMessage("Orientation Test successfully completed");
            instruction.Text = "Orientation Test successfully completed";
            DisplayRestartOrientation();
        }

        public async void Failed()
        {
            countdown.Stop();
            HideArrows();
            Hide();
            int type = SensorType[pivotSensor.SelectedIndex];
            if (type == Sensor.ACCELEROMETER)
            {
                currentAccelerometer.ReadingChanged -= AccelerometerReadingChangedOrientation;
            }
            else if (type == Sensor.INCLINOMETER)
            {
                currentInclinometer.ReadingChanged -= InclinometerReadingChangedOrientation;
            }
            else if (type == Sensor.ORIENTATIONSENSOR)
            {
                currentOrientationSensor.ReadingChanged -= OrientationReadingChangedOrientation;
            }
            else if (type == Sensor.SIMPLEORIENTATIONSENSOR)
            {
                currentSimpleOrientationSensor.OrientationChanged -= SimpleOrientationChangedOrientation;
            }
            LogTestFailure(testsCompleted, Enum.GetName(typeof(Directions), arrowDir), Constants.SensorName[type] + "SingleTestResult");

            // Display red x for 2 sec
            imgX.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            imgX.Visibility = Visibility.Collapsed;

            rootPage.loggingChannelTests.LogMessage("Orientation Test failed");
            hyperlink.NavigateUri = new Uri("https://docs.microsoft.com/en-us/windows-hardware/drivers/sensors/testing-sensor-landing");
            run.Text = "https://docs.microsoft.com/en-us/windows-hardware/drivers/sensors/testing-sensor-landing";
            instruction.Text = "Orientation Test failed.\n\nFor more information on sensor testing, please refer to:";
            DisplayRestartOrientation();
        }

        private void DisplayRestartOrientation()
        {
            rootPage.EnableScenarioSelect();
            feedbackButton.Visibility = Visibility.Visible;
            restartButtonOrientation.Visibility = Visibility.Visible;
            saveButtonOrientation.Visibility = Visibility.Visible;
            saveButtonOrientation.IsEnabled = true;
        }

        private void Refresh()
        {
            // Completed testing for the required number of iterations
            if (testsCompleted == testIterations)
            {
                Success();
            }
            else
            {
                instruction.Text = "Please orient your device so that the arrow is pointing down to the ground";

                // Generate a random direction
                Random rnd = new Random();
                int dir = rnd.Next(0, 3);

                if (testsCompleted != 0 && testsCompleted % 4 == 0)
                {
                    for (int i = 0; i < numQuadrants; i++)
                    {
                        quadrants[i] = 0;
                    }
                    while (dir == arrowDir)
                    {
                        dir = rnd.Next(0, 3);
                    }
                }

                // If testing has been completed twice in this quadrant, generate a new random direction
                while (quadrants[dir] == 1)
                {
                    if (testsCompleted % 4 == 3)
                    {
                        dir = 0;
                        while (quadrants[dir] == 1)
                        {
                            dir++;
                        }
                    }
                    else
                    {
                        dir = rnd.Next(0, 3);
                    }
                }

                arrowDir = dir;
                quadrants[dir]++;
                RotateArrow(dir);

                countdown = new Countdown(countdownTime, testType);

                int type = SensorType[pivotSensor.SelectedIndex];
                if (type == Sensor.ACCELEROMETER)
                {
                    currentAccelerometer.ReadingChanged += AccelerometerReadingChangedOrientation;
                }
                else if (type == Sensor.INCLINOMETER)
                {
                    currentInclinometer.ReadingChanged += InclinometerReadingChangedOrientation;
                }
                else if (type == Sensor.ORIENTATIONSENSOR)
                {
                    currentOrientationSensor.ReadingChanged += OrientationReadingChangedOrientation;
                }
                else if (type == Sensor.SIMPLEORIENTATIONSENSOR)
                {
                    currentSimpleOrientationSensor.OrientationChanged += SimpleOrientationChangedOrientation;
                }
            }
        }

        private void RotateArrow(int dir)
        {
            circle.Visibility = Visibility.Visible;
            if (dir == (int)Directions.left)
            {
                arrowLeft.Visibility = Visibility.Visible;
            }
            else if (dir == (int)Directions.right)
            {
                arrowRight.Visibility = Visibility.Visible;
            }
            else if (dir == (int)Directions.up)
            {
                if (SensorType[pivotSensor.SelectedIndex] == Sensor.SIMPLEORIENTATIONSENSOR)
                {
                    instruction.Text = "Place your device face up.";
                    circle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    arrowUp.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (SensorType[pivotSensor.SelectedIndex] == Sensor.SIMPLEORIENTATIONSENSOR)
                {
                    instruction.Text = "Place your device face down.";
                    circle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    arrowDown.Visibility = Visibility.Visible;
                }
            }
        }

        private void HideArrows()
        {
            arrowLeft.Visibility = Visibility.Collapsed;
            arrowRight.Visibility = Visibility.Collapsed;
            arrowUp.Visibility = Visibility.Collapsed;
            arrowDown.Visibility = Visibility.Collapsed;
            circle.Visibility = Visibility.Collapsed;
        }

        private void Hide()
        {
            timerLog.Text = "";
            instruction.Text = "";
            run.Text = "";
        }
    }
}
