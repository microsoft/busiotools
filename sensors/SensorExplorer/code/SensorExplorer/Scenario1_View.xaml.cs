using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Sensors;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace SensorExplorer
{
    public sealed partial class Scenario1View : Page
    {
        public static Scenario1View Current;
        private MainPage rootPage = MainPage.Current;

        private List<SensorDisplay> _sensorDisplay;
        private List<SensorData> _sensorData;

        private ApplicationDataContainer _localState = ApplicationData.Current.LocalSettings;
        private Popup settingsPopup; // This is the container that will hold our custom content

        public Scenario1View()
        {
            this.InitializeComponent();
            Current = this;

            this.SizeChanged += MainPageSizeChanged;

            _sensorDisplay = new List<SensorDisplay>();
            _sensorData = new List<SensorData>();

            EnumerateSensors();

            PeriodicTimer.sensorData = _sensorData;
            PeriodicTimer.sensorDisplay = _sensorDisplay;

            Sensor.sensorData = _sensorData;
            Sensor.sensorDisplay = _sensorDisplay;

            var resourceLoader = ResourceLoader.GetForCurrentView();

            saveFileButton.Click += SaveFileButtonClick;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (Sensor.currentId >= 0 && Sensor.currentId != PivotSensor.Items.Count - 1)
            {
                Sensor.DisableSensor(Sensor.sensorDisplay[Sensor.currentId]._sensorType, Sensor.sensorDisplay[Sensor.currentId]._index);
            }
            rootPage.NotifyUser("", NotifyType.StatusMessage);
        }

        private async void EnumerateSensors()
        {
            try
            {
                await Sensor.GetDefault();
                int totalIndex = -1;
                for (int index = 0; index < Sensor.AccelerometerStandardList.Count; index++)
                {
                    totalIndex++;
                    SensorData _accelerometerData = new SensorData(Sensor.ACCELEROMETER, totalIndex, "Accelerometer (Standard)", new string[] { "AccelerationX (g)", "AccelerationY (g)", "AccelerationZ (g)" });
                    SensorDisplay _accelerometerDisplay = new SensorDisplay(Sensor.ACCELEROMETER, index, totalIndex, "Accelerometer (Standard)", -2, 2, 2, new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan });
                    _sensorDisplay.Add(_accelerometerDisplay);
                    _sensorData.Add(_accelerometerData);
                    AddPivotItem(Sensor.ACCELEROMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.AccelerometerLinearList.Count; index++)
                {
                    totalIndex++;
                    SensorData _accelerometerLinearData = new SensorData(Sensor.ACCELEROMETERLINEAR, totalIndex, "Accelerometer (Linear)", new string[] { "AccelerationX (g)", "AccelerationY (g)", "AccelerationZ (g)" });
                    SensorDisplay _accelerometerLinearDisplay = new SensorDisplay(Sensor.ACCELEROMETERLINEAR, index, totalIndex, "Accelerometer (Linear)", -2, 2, 2, new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan });
                    _sensorDisplay.Add(_accelerometerLinearDisplay);
                    _sensorData.Add(_accelerometerLinearData);
                    AddPivotItem(Sensor.ACCELEROMETERLINEAR, index, totalIndex);
                }
                for (int index = 0; index < Sensor.AccelerometerGravityList.Count; index++)
                {
                    totalIndex++;
                    SensorData _accelerometerGravityData = new SensorData(Sensor.ACCELEROMETERGRAVITY, totalIndex, "Accelerometer (Gravity)", new string[] { "AccelerationX (g)", "AccelerationY (g)", "AccelerationZ (g)" });
                    SensorDisplay _accelerometerGravityDisplay = new SensorDisplay(Sensor.ACCELEROMETERGRAVITY, index, totalIndex, "Accelerometer (Gravity)", -2, 2, 2, new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan });
                    _sensorDisplay.Add(_accelerometerGravityDisplay);
                    _sensorData.Add(_accelerometerGravityData);
                    AddPivotItem(Sensor.ACCELEROMETERGRAVITY, index, totalIndex);
                }
                for (int index = 0; index < Sensor.ActivitySensorList.Count; index++)
                {
                    totalIndex++;
                    SensorData _activitySensorData = new SensorData(Sensor.ACTIVITYSENSOR, totalIndex, "ActivitySensor", new string[] { "AccelerationX (g)", "AccelerationY (g)", "AccelerationZ (g)" });
                    SensorDisplay _activitySensorDisplay = new SensorDisplay(Sensor.ACTIVITYSENSOR, index, totalIndex, "ActivitySensor", 2, 0, 2, new Color[] { Colors.Gray, Colors.Brown, Colors.DarkRed, Colors.Orange, Colors.DarkOrange, Colors.Lime, Colors.DarkCyan, Colors.DarkViolet });
                    _sensorDisplay.Add(_activitySensorDisplay);
                    _sensorData.Add(_activitySensorData);
                    AddPivotItem(Sensor.ACTIVITYSENSOR, index, totalIndex);
                }
                if (Sensor.Altimeter != null)
                {
                    totalIndex++;
                    SensorData _altimeterData = new SensorData(Sensor.ALTIMETER, totalIndex, "Altimeter", new string[] { "AltitudeChange (m)" });
                    SensorDisplay _altimeterDisplay = new SensorDisplay(Sensor.ALTIMETER, 0, totalIndex, "Altimeter", -10, 10, 2, new Color[] { Colors.DarkRed });
                    _sensorDisplay.Add(_altimeterDisplay);
                    _sensorData.Add(_altimeterData);
                    AddPivotItem(Sensor.ALTIMETER, 0, totalIndex);
                }
                for (int index = 0; index < Sensor.BarometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData _barometerData = new SensorData(Sensor.BAROMETER, totalIndex, "Barometer", new string[] { "Pressure (hPa)" });
                    SensorDisplay _barometerDisplay = new SensorDisplay(Sensor.BAROMETER, index, totalIndex, "Barometer", 950, 1050, 2, new Color[] { Colors.Lime });
                    _sensorDisplay.Add(_barometerDisplay);
                    _sensorData.Add(_barometerData);
                    AddPivotItem(Sensor.BAROMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.CompassList.Count; index++)
                {
                    totalIndex++;
                    SensorData _compassData = new SensorData(Sensor.COMPASS, totalIndex, "Compass", new string[] { "MagneticNorth (°)", "TrueNorth (°)", "HeadingAccuracy" });
                    SensorDisplay _compassDisplay = new SensorDisplay(Sensor.COMPASS, index, totalIndex, "Compass", 0, 360, 2, new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan });
                    _sensorDisplay.Add(_compassDisplay);
                    _sensorData.Add(_compassData);
                    AddPivotItem(Sensor.COMPASS, index, totalIndex);
                }
                for (int index = 0; index < Sensor.GyrometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData _gyrometerData = new SensorData(Sensor.GYROMETER, totalIndex, "Gyrometer", new string[] { "AngularVelocityX (°/s)", "AngularVelocityY (°/s)", "AngularVelocityZ (°/s)" });
                    SensorDisplay _gyrometerDisplay = new SensorDisplay(Sensor.GYROMETER, index, totalIndex, "Gyrometer", -200, 200, 2, new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan });
                    _sensorDisplay.Add(_gyrometerDisplay);
                    _sensorData.Add(_gyrometerData);
                    AddPivotItem(Sensor.GYROMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.InclinometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData _inclinometerData = new SensorData(Sensor.INCLINOMETER, totalIndex, "Inclinometer", new string[] { "Pitch (°)", "Roll (°)", "Yaw (°)", "YawAccuracy" });
                    SensorDisplay _inclinometerDisplay = new SensorDisplay(Sensor.INCLINOMETER, index, totalIndex, "Inclinometer", -180, 360, 3, new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan, Colors.Black });
                    _sensorDisplay.Add(_inclinometerDisplay);
                    _sensorData.Add(_inclinometerData);
                    AddPivotItem(Sensor.INCLINOMETER, index, totalIndex);
                }
                if (Sensor.LightSensor != null)
                {
                    totalIndex++;
                    SensorData _lightSensorData = new SensorData(Sensor.LIGHTSENSOR, totalIndex, "LightSensor", new string[] { "Illuminance (lux)" });
                    SensorDisplay _lightSensorDisplay = new SensorDisplay(Sensor.LIGHTSENSOR, 0, totalIndex, "LightSensor", 0, 1000, 2, new Color[] { Colors.DarkOrange });
                    _sensorDisplay.Add(_lightSensorDisplay);
                    _sensorData.Add(_lightSensorData);
                    AddPivotItem(Sensor.LIGHTSENSOR, 0, totalIndex);
                }
                for (int index = 0; index < Sensor.MagnetometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData _magnetometerData = new SensorData(Sensor.MAGNETOMETER, totalIndex, "Magnetometer", new string[] { "MagneticFieldX (µT)", "MagneticFieldY (µT)", "MagneticFieldZ (µT)" });
                    SensorDisplay _magnetometerDisplay = new SensorDisplay(Sensor.MAGNETOMETER, index, totalIndex, "Magnetometer", -500, 500, 2, new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan });
                    _sensorDisplay.Add(_magnetometerDisplay);
                    _sensorData.Add(_magnetometerData);
                    AddPivotItem(Sensor.MAGNETOMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.OrientationAbsoluteList.Count; index++)
                {
                    totalIndex++;
                    SensorData _orientationAbsoluteData = new SensorData(Sensor.ORIENTATIONSENSOR, totalIndex, "Orientation (Absolute)",
                                                                   new string[] { "QuaternionX", "QuaternionY", "QuaternionZ", "QuaternionW",
                                                                                  "RotationMatrixM11", "RotationMatrixM12", "RotationMatrixM13",
                                                                                  "RotationMatrixM21", "RotationMatrixM22", "RotationMatrixM23",
                                                                                  "RotationMatrixM31", "RotationMatrixM32", "RotationMatrixM33" });
                    SensorDisplay _orientationAbsoluteDisplay = new SensorDisplay(Sensor.ORIENTATIONSENSOR, index, totalIndex, "Orientation (Absolute)", -1, 1, 2,
                                                                            new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan, Colors.DarkViolet,
                                                                                          Colors.Black, Colors.Black, Colors.Black,
                                                                                          Colors.Black, Colors.Black, Colors.Black,
                                                                                          Colors.Black, Colors.Black, Colors.Black });
                    _sensorDisplay.Add(_orientationAbsoluteDisplay);
                    _sensorData.Add(_orientationAbsoluteData);
                    AddPivotItem(Sensor.ORIENTATIONSENSOR, index, totalIndex);
                }
                for (int index = 0; index < Sensor.OrientationRelativeList.Count; index++)
                {
                    totalIndex++;
                    SensorData _orientationRelativeData = new SensorData(Sensor.ORIENTATIONRELATIVE, totalIndex, "Orientation (Relative)",
                                                                   new string[] { "QuaternionX", "QuaternionY", "QuaternionZ", "QuaternionW",
                                                                                  "RotationMatrixM11", "RotationMatrixM12", "RotationMatrixM13",
                                                                                  "RotationMatrixM21", "RotationMatrixM22", "RotationMatrixM23",
                                                                                  "RotationMatrixM31", "RotationMatrixM32", "RotationMatrixM33" });
                    SensorDisplay _orientationRelativeDisplay = new SensorDisplay(Sensor.ORIENTATIONRELATIVE, index, totalIndex, "Orientation (Relative)", -1, 1, 2,
                                                                            new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan, Colors.DarkViolet,
                                                                                          Colors.Black, Colors.Black, Colors.Black,
                                                                                          Colors.Black, Colors.Black, Colors.Black,
                                                                                          Colors.Black, Colors.Black, Colors.Black });
                    _sensorDisplay.Add(_orientationRelativeDisplay);
                    _sensorData.Add(_orientationRelativeData);
                    AddPivotItem(Sensor.ORIENTATIONRELATIVE, index, totalIndex);
                }
                for (int index = 0; index < Sensor.OrientationGeomagneticList.Count; index++)
                {
                    totalIndex++;
                    SensorData _orientationGeomagneticData = new SensorData(Sensor.ORIENTATIONGEOMAGNETIC, totalIndex, "Orientation (Geomagnetic)",
                                                                   new string[] { "QuaternionX", "QuaternionY", "QuaternionZ", "QuaternionW",
                                                                                  "RotationMatrixM11", "RotationMatrixM12", "RotationMatrixM13",
                                                                                  "RotationMatrixM21", "RotationMatrixM22", "RotationMatrixM23",
                                                                                  "RotationMatrixM31", "RotationMatrixM32", "RotationMatrixM33" });
                    SensorDisplay _orientationGeomagneticDisplay = new SensorDisplay(Sensor.ORIENTATIONGEOMAGNETIC, index, totalIndex, "Orientation (Geomagnetic)", -1, 1, 2,
                                                                            new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan, Colors.DarkViolet,
                                                                                          Colors.Black, Colors.Black, Colors.Black,
                                                                                          Colors.Black, Colors.Black, Colors.Black,
                                                                                          Colors.Black, Colors.Black, Colors.Black });
                    _sensorDisplay.Add(_orientationGeomagneticDisplay);
                    _sensorData.Add(_orientationGeomagneticData);
                    AddPivotItem(Sensor.ORIENTATIONGEOMAGNETIC, index, totalIndex);
                }
                for (int index = 0; index < Sensor.PedometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData _pedometerData = new SensorData(Sensor.PEDOMETER, totalIndex, "Pedometer", new string[] { "CumulativeSteps", "CumulativeStepsDuration (s)", "StepKind" });
                    SensorDisplay _pedometerDisplay = new SensorDisplay(Sensor.PEDOMETER, index, totalIndex, "Pedometer", 0, 50, 2, new Color[] { Colors.DarkCyan, Colors.Black, Colors.Black });
                    _sensorDisplay.Add(_pedometerDisplay);
                    _sensorData.Add(_pedometerData);
                    AddPivotItem(Sensor.PEDOMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.ProximitySensorList.Count; index++)
                {
                    totalIndex++;
                    SensorData _proximitySensorData = new SensorData(Sensor.PROXIMITYSENSOR, totalIndex, "ProximitySensor", new string[] { "IsDetected", "Distance (mm)" });
                    SensorDisplay _proximitySensorDisplay = new SensorDisplay(Sensor.PROXIMITYSENSOR, index, totalIndex, "ProximitySensor", 0, 1, 1, new Color[] { Colors.DarkOrange, Colors.Black });
                    _sensorDisplay.Add(_proximitySensorDisplay);
                    _sensorData.Add(_proximitySensorData);
                    AddPivotItem(Sensor.PROXIMITYSENSOR, index, totalIndex);
                }
                if (Sensor.SimpleOrientationSensor != null)
                {
                    totalIndex++;
                    SensorData _simpleOrientationSensorData = new SensorData(Sensor.SIMPLEORIENTATIONSENSOR, totalIndex, "SimpleOrientationSensor", new string[] { "SimpleOrientation" });
                    SensorDisplay _simpleOrientationSensorDisplay = new SensorDisplay(Sensor.SIMPLEORIENTATIONSENSOR, 0, totalIndex, "SimpleOrientationSensor", 0, 5, 5, new Color[] { Colors.Lime });
                    _sensorDisplay.Add(_simpleOrientationSensorDisplay);
                    _sensorData.Add(_simpleOrientationSensorData);
                    AddPivotItem(Sensor.PROXIMITYSENSOR, 0, totalIndex);
                }

                AddSummaryPage();

                var resourceLoader = ResourceLoader.GetForCurrentView();
                rootPage.NotifyUser(resourceLoader.GetString("NumberOfSensors") + ": " + (PivotSensor.Items.Count - 1), NotifyType.StatusMessage);
                ProgressRingSensor.IsActive = false;

                if (PivotSensor.Items.Count > 0)
                {
                    PivotSensor.SelectionChanged += PivotSensorSelectionChanged;
                    PivotSensor.SelectedIndex = 0;
                    PivotSensorSelectionChanged(null, null);
                }
                else
                {
                    TextBlockNoSensor.Text = resourceLoader.GetString("CannotFindSensor");
                    return;
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }

            ProgressRingSensor.IsActive = false;
        }

        private void AddPivotItem(int sensorType, int index, int totalIndex)
        {
            PivotItem PivotItemSensor = new PivotItem();
            ScrollViewer scrollViewerSensor = new ScrollViewer();
            scrollViewerSensor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewerSensor.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            PivotItemSensor.Header = _sensorData[totalIndex]._name + " " + (index + 1);
            scrollViewerSensor.Content = _sensorDisplay[totalIndex].StackPanelSensor;
            PivotItemSensor.Content = scrollViewerSensor;
            PivotSensor.Items.Add(PivotItemSensor);
        }

        /// <summary>
        /// This the event handler for the "Defaults" button added to the settings charm. This method
        /// is responsible for creating the Popup window will use as the container for our settings Flyout.
        /// The reason we use a Popup is that it gives us the "light dismiss" behavior that when a user clicks away 
        /// from our custom UI it just dismisses.  This is a principle in the Settings experience and you see the
        /// same behavior in other experiences like AppBar. 
        /// </summary>
        void onSettingsCommand(IUICommand command)
        {
            // Create a Popup window which will contain our flyout
            settingsPopup = new Popup();
            settingsPopup.IsLightDismissEnabled = true;
            settingsPopup.Height = Window.Current.Bounds.Height;

            // Add the proper animation for the panel
            settingsPopup.ChildTransitions = new TransitionCollection();
            settingsPopup.ChildTransitions.Add(new PaneThemeTransition());

            // Let's define the location of our Popup
            settingsPopup.SetValue(Canvas.TopProperty, 0);
            settingsPopup.IsOpen = true;
        }

        void MainPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width - 50;
            double actualWidth = 0;
            for (int i = 0; i < _sensorDisplay.Count; i++)
            {
                actualWidth = _sensorDisplay[i].SetWidth(e.NewSize.Width, e.NewSize.Height);
            }
        }

        private void PivotSensorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < PivotSensor.Items.Count; i++)
            {
                if (i != PivotSensor.SelectedIndex)
                {
                    (((PivotSensor.Items[i] as PivotItem).Content as ScrollViewer).Content as StackPanel).Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (Sensor.currentId != -1 && Sensor.currentId != PivotSensor.Items.Count - 1) // diable previous sensor
                    {
                        Sensor.DisableSensor(Sensor.sensorDisplay[Sensor.currentId]._sensorType, Sensor.sensorDisplay[Sensor.currentId]._index);
                    }
                    Sensor.currentId = i;   // sensor being displayed
                    if (i != PivotSensor.Items.Count - 1)
                    {
                        _sensorDisplay[i].EnableSensor();
                    }
                    (((PivotSensor.Items[i] as PivotItem).Content as ScrollViewer).Content as StackPanel).Visibility = Visibility.Visible;
                    if ((PivotSensor.Items[i] as PivotItem).Header.ToString().Contains("LightSensor"))
                    {
                        saveFileButton.IsEnabled = true;
                    }
                    else
                    {
                        saveFileButton.IsEnabled = false;
                    }
                }
            }
        }

        public void LogDataLightSensor(LightSensorReading reading)
        {
            LoggingFields loggingFields = new LoggingFields();
            loggingFields.AddString("Timestamp", reading.Timestamp.ToString());
            loggingFields.AddDouble("IlluminanceInLux", reading.IlluminanceInLux);
            rootPage.loggingChannelView.LogEvent("LightSensorData", loggingFields);
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

        public async void SaveFileButtonClick(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("ETL", new List<string>() { ".etl" });
            savePicker.SuggestedFileName = "SensorExplorerLog";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                StorageFile logFileGenerated = await rootPage.loggingSessionView.CloseAndSaveToFileAsync();
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
        }

        private void AddSummaryPage()
        {
            PivotItem PivotItemSensor = new PivotItem();
            ScrollViewer scrollViewerSensor = new ScrollViewer();
            scrollViewerSensor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewerSensor.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            StackPanel stackpanel = new StackPanel();
            stackpanel.Orientation = Orientation.Horizontal;
            StackPanel stackpanelProperty = new StackPanel();
            StackPanel stackpanelValue = new StackPanel();
            TextBlock[] TextBlockProperties = new TextBlock[18];
            TextBlock[] TextBlockValues = new TextBlock[18];
            for (int i = 0; i < 18; i++)
            {
                TextBlockProperties[i] = new TextBlock();
                TextBlockProperties[i].FontSize = 20;
                stackpanelProperty.Children.Add(TextBlockProperties[i]);

                TextBlockValues[i] = new TextBlock();
                TextBlockValues[i].FontSize = 20;
                TextBlockValues[i].HorizontalAlignment = HorizontalAlignment.Center;
                stackpanelValue.Children.Add(TextBlockValues[i]);
            }

            stackpanel.Children.Add(stackpanelProperty);
            stackpanel.Children.Add(stackpanelValue);

            TextBlockProperties[0].Text = "Sensor Category";
            TextBlockProperties[0].FontWeight = FontWeights.Bold;
            TextBlockProperties[1].Text = "Accelerometer (Standard)";
            TextBlockProperties[2].Text = "Accelerometer (Gravity)";
            TextBlockProperties[3].Text = "Accelerometer (Relative)";
            TextBlockProperties[4].Text = "Activity Sensor";
            TextBlockProperties[5].Text = "Altimeter";
            TextBlockProperties[6].Text = "Barometer";
            TextBlockProperties[7].Text = "Compass";
            TextBlockProperties[8].Text = "Gyrometer";
            TextBlockProperties[9].Text = "Inclinometer";
            TextBlockProperties[10].Text = "Light Sensor";
            TextBlockProperties[11].Text = "Magnetometer";
            TextBlockProperties[12].Text = "Orientation Sensor (Absolute)";
            TextBlockProperties[13].Text = "Orientation Sensor (Geomagnetic)";
            TextBlockProperties[14].Text = "Orientation Sensor (Relative)";
            TextBlockProperties[15].Text = "Pedometer";
            TextBlockProperties[16].Text = "Proximity Sensor";
            TextBlockProperties[17].Text = "Simple Orientation Sensor";

            TextBlockValues[0].Text = "Number of Sensor(s) Available";
            TextBlockValues[0].FontWeight = FontWeights.Bold;
            TextBlockValues[1].Text = Sensor.AccelerometerStandardList.Count.ToString();
            TextBlockValues[2].Text = Sensor.AccelerometerGravityList.Count.ToString();
            TextBlockValues[3].Text = Sensor.AccelerometerLinearList.Count.ToString();
            TextBlockValues[4].Text = Sensor.ActivitySensorList.Count.ToString();
            TextBlockValues[5].Text = (Sensor.Altimeter == null) ? ("0") : ("1");
            TextBlockValues[6].Text = Sensor.BarometerList.Count.ToString();
            TextBlockValues[7].Text = Sensor.CompassList.Count.ToString();
            TextBlockValues[8].Text = Sensor.GyrometerList.Count.ToString();
            TextBlockValues[9].Text = Sensor.InclinometerList.Count.ToString();
            TextBlockValues[10].Text = (Sensor.LightSensor == null) ? ("0") : ("1");
            TextBlockValues[11].Text = Sensor.MagnetometerList.Count.ToString();
            TextBlockValues[12].Text = Sensor.OrientationAbsoluteList.Count.ToString();
            TextBlockValues[13].Text = Sensor.OrientationGeomagneticList.Count.ToString();
            TextBlockValues[14].Text = Sensor.OrientationRelativeList.Count.ToString();
            TextBlockValues[15].Text = Sensor.PedometerList.Count.ToString();
            TextBlockValues[16].Text = Sensor.ProximitySensorList.Count.ToString();
            TextBlockValues[17].Text = (Sensor.SimpleOrientationSensor == null) ? ("0") : ("1");

            PivotItemSensor.Header = "Summary";
            scrollViewerSensor.Content = stackpanel;
            PivotItemSensor.Content = scrollViewerSensor;
            PivotSensor.Items.Add(PivotItemSensor);
        }

        private void ReportIntervalButton(object sender, RoutedEventArgs e)
        {
            try
            {
                SensorDisplay selected = _sensorDisplay[Sensor.currentId];
                if (selected._sensorType == Sensor.ACCELEROMETER)
                {
                    Sensor.AccelerometerStandardList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ACCELEROMETERLINEAR)
                {
                    Sensor.AccelerometerLinearList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ACCELEROMETERGRAVITY)
                {
                    Sensor.AccelerometerGravityList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                // ActivitySensor doesn't have ReportInterval
                else if (selected._sensorType == Sensor.ALTIMETER)
                {
                    Sensor.Altimeter.ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.BAROMETER)
                {
                    Sensor.BarometerList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.COMPASS)
                {
                    Sensor.CompassList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.GYROMETER)
                {
                    Sensor.GyrometerList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.INCLINOMETER)
                {
                    Sensor.InclinometerList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.LIGHTSENSOR)
                {
                    Sensor.LightSensor.ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.MAGNETOMETER)
                {
                    Sensor.MagnetometerList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ORIENTATIONSENSOR)
                {
                    Sensor.OrientationAbsoluteList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ORIENTATIONGEOMAGNETIC)
                {
                    Sensor.OrientationGeomagneticList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ORIENTATIONRELATIVE)
                {
                    Sensor.OrientationRelativeList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.PEDOMETER)
                {
                    Sensor.PedometerList[selected._index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                //ProximitySensor doesn't have ReportInterval
                //SimpleOrientationSensor doesn't have ReportInterval               
            }
            catch { }
        }
    }
}