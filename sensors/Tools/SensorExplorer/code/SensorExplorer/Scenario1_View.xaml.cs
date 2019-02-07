// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
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

        // MALT
        private const string buttonNameDisconnectFromDevice = "Disconnect from device";
        private const string buttonNameDisableReconnectToDevice = "Do not automatically reconnect to device that was just closed";
        private SuspendingEventHandler appSuspendEventHandler;
        private EventHandler<Object> appResumeEventHandler;
        private Dictionary<DeviceWatcher, string> mapDeviceWatchersToDeviceSelector;
        private Boolean watchersSuspended;
        private Boolean watchersStarted;
        private Boolean isAllDevicesEnumerated;
        private Boolean IsNavigatedAway;
        private StorageFile tmp, file;
        private DataReader DataReaderObject = null;
        private DataWriter DataWriterObject = null;
        private List<string> conversionValues = new List<string> { "100", "800" };

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
                for (int index = 0; index < Sensor.LightSensorList.Count; index++)
                {
                    totalIndex++;
                    SensorData _lightSensorData = new SensorData(Sensor.LIGHTSENSOR, totalIndex, "LightSensor", new string[] { "Illuminance (lux)", "Chromaticity X", "Chromaticity Y" });
                    SensorDisplay _lightSensorDisplay = new SensorDisplay(Sensor.LIGHTSENSOR, index, totalIndex, "LightSensor", 0, 1000, 2, new Color[] { Colors.DarkRed, Colors.DarkOrange, Colors.DarkCyan });
                    _sensorDisplay.Add(_lightSensorDisplay);
                    _sensorData.Add(_lightSensorData);
                    AddPivotItem(Sensor.LIGHTSENSOR, index, totalIndex);
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
                for (int index = 0; index < Sensor.SimpleOrientationSensorList.Count; index++)
                {
                    totalIndex++;
                    SensorData _simpleOrientationSensorData = new SensorData(Sensor.SIMPLEORIENTATIONSENSOR, totalIndex, "SimpleOrientationSensor", new string[] { "SimpleOrientation" });
                    SensorDisplay _simpleOrientationSensorDisplay = new SensorDisplay(Sensor.SIMPLEORIENTATIONSENSOR, index, totalIndex, "SimpleOrientationSensor", 0, 5, 5, new Color[] { Colors.Lime });
                    _sensorDisplay.Add(_simpleOrientationSensorDisplay);
                    _sensorData.Add(_simpleOrientationSensorData);
                    AddPivotItem(Sensor.PROXIMITYSENSOR, index, totalIndex);
                }

                AddSummaryPage();

                var resourceLoader = ResourceLoader.GetForCurrentView();
                rootPage.NotifyUser(resourceLoader.GetString("NumberOfSensors") + ": " + (PivotSensor.Items.Count - 1) + "\nNumber of sensors failed to enumerate: " + Sensor.NumFailedEnumerations, NotifyType.StatusMessage);
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

        public void MALTButton(object sender, RoutedEventArgs e)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];
            selected.MALTButton.Visibility = Visibility.Collapsed;
            selected.stackPanelMALT.Visibility = Visibility.Visible;
            connectDevices.Visibility = Visibility.Visible;

            selected.listOfDevices = new ObservableCollection<DeviceListEntry>();
            mapDeviceWatchersToDeviceSelector = new Dictionary<DeviceWatcher, string>();
            watchersStarted = false;
            watchersSuspended = false;
            isAllDevicesEnumerated = false;

            // If we are connected to the device or planning to reconnect, we should disable the list of devices
            // to prevent the user from opening a device without explicitly closing or disabling the auto reconnect
            if (EventHandlerForDevice.Current.IsDeviceConnected
                || (EventHandlerForDevice.Current.IsEnabledAutoReconnect
                && EventHandlerForDevice.Current.DeviceInformation != null))
            {
                UpdateConnectDisconnectButtonsAndList(false);

                // These notifications will occur if we are waiting to reconnect to device when we start the page
                EventHandlerForDevice.Current.OnDeviceConnected = this.OnDeviceConnected;
                EventHandlerForDevice.Current.OnDeviceClose = this.OnDeviceClosing;
            }
            else
            {
                UpdateConnectDisconnectButtonsAndList(true);
            }

            // Begin watching out for events
            StartHandlingAppEvents();

            // Initialize the desired device watchers so that we can watch for when devices are connected/removed
            InitializeDeviceWatchers();
            StartDeviceWatchers();

            DeviceListSource.Source = selected.listOfDevices;
        }

        public async void ConnectToDeviceClick(Object sender, RoutedEventArgs eventArgs)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            var selection = connectDevices.SelectedItems;
            DeviceListEntry entry = null;

            if (selection.Count > 0)
            {
                var obj = selection[0];
                entry = (DeviceListEntry)obj;

                if (entry != null)
                {
                    // Create an EventHandlerForDevice to watch for the device we are connecting to
                    EventHandlerForDevice.CreateNewEventHandlerForDevice();

                    // Get notified when the device was successfully connected to or about to be closed
                    EventHandlerForDevice.Current.OnDeviceConnected = OnDeviceConnected;
                    EventHandlerForDevice.Current.OnDeviceClose = OnDeviceClosing;

                    // It is important that the FromIdAsync call is made on the UI thread because the consent prompt, when present,
                    // can only be displayed on the UI thread. Since this method is invoked by the UI, we are already in the UI thread.
                    Boolean openSuccess = await EventHandlerForDevice.Current.OpenDeviceAsync(entry.DeviceInformation, entry.DeviceSelector);

                    // Disable connect button if we connected to the device
                    UpdateConnectDisconnectButtonsAndList(!openSuccess);

                    if (openSuccess)
                    {
                        selected.stackPanelMALT.Visibility = Visibility.Collapsed;
                        connectDevices.Visibility = Visibility.Collapsed;
                        //initialize();
                        selected.stackPanelMALTData.Visibility = Visibility.Visible;
                        PeriodicTimer.CreateScenario1();
                    }
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
            StackPanel stackpanelFailed = new StackPanel();
            TextBlock[] TextBlockProperties = new TextBlock[19];
            TextBlock[] TextBlockValues = new TextBlock[19];
            TextBlock[] TextBlockFailed = new TextBlock[19];
            for (int i = 0; i < 19; i++)
            {
                TextBlockProperties[i] = new TextBlock();
                TextBlockProperties[i].FontSize = 20;
                stackpanelProperty.Children.Add(TextBlockProperties[i]);

                TextBlockValues[i] = new TextBlock();
                TextBlockValues[i].FontSize = 20;
                TextBlockValues[i].HorizontalAlignment = HorizontalAlignment.Center;
                stackpanelValue.Children.Add(TextBlockValues[i]);

                TextBlockFailed[i] = new TextBlock();
                TextBlockFailed[i].Margin = new Thickness() { Left = 20 };
                TextBlockFailed[i].FontSize = 20;
                TextBlockFailed[i].HorizontalAlignment = HorizontalAlignment.Center;
                stackpanelFailed.Children.Add(TextBlockFailed[i]);
            }

            stackpanel.Children.Add(stackpanelProperty);
            stackpanel.Children.Add(stackpanelValue);
            stackpanel.Children.Add(stackpanelFailed);

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
            TextBlockProperties[18].Text = "Other";

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
            TextBlockValues[10].Text = Sensor.LightSensorList.Count.ToString();
            TextBlockValues[11].Text = Sensor.MagnetometerList.Count.ToString();
            TextBlockValues[12].Text = Sensor.OrientationAbsoluteList.Count.ToString();
            TextBlockValues[13].Text = Sensor.OrientationGeomagneticList.Count.ToString();
            TextBlockValues[14].Text = Sensor.OrientationRelativeList.Count.ToString();
            TextBlockValues[15].Text = Sensor.PedometerList.Count.ToString();
            TextBlockValues[16].Text = Sensor.ProximitySensorList.Count.ToString();
            TextBlockValues[17].Text = Sensor.SimpleOrientationSensorList.Count.ToString();
            TextBlockValues[18].Text = Sensor.SensorClassDevice.Count.ToString();

            TextBlockFailed[0].Text = "Any Failed Enumerations";
            TextBlockFailed[0].FontWeight = FontWeights.Bold;
            TextBlockFailed[1].Text = Sensor.AccelerometerStandardFailed ? "Yes" : "No";
            TextBlockFailed[2].Text = Sensor.AccelerometerGravityFailed ? "Yes" : "No";
            TextBlockFailed[3].Text = Sensor.AccelerometerLinearFailed ? "Yes" : "No";
            TextBlockFailed[4].Text = Sensor.ActivitySensorFailed ? "Yes" : "No";
            TextBlockFailed[5].Text = Sensor.AltimeterFailed ? "Yes" : "No";
            TextBlockFailed[6].Text = Sensor.BarometerFailed ? "Yes" : "No";
            TextBlockFailed[7].Text = Sensor.CompassFailed ? "Yes" : "No";
            TextBlockFailed[8].Text = Sensor.GyrometerFailed ? "Yes" : "No";
            TextBlockFailed[9].Text = Sensor.InclinometerFailed ? "Yes" : "No";
            TextBlockFailed[10].Text = Sensor.LightSensorFailed ? "Yes" : "No";
            TextBlockFailed[11].Text = Sensor.MagnetometerFailed ? "Yes" : "No";
            TextBlockFailed[12].Text = Sensor.OrientationAbsoluteFailed ? "Yes" : "No";
            TextBlockFailed[13].Text = Sensor.OrientationGeomagneticFailed ? "Yes" : "No";
            TextBlockFailed[14].Text = Sensor.OrientationRelativeFailed ? "Yes" : "No";
            TextBlockFailed[15].Text = Sensor.PedometerFailed ? "Yes" : "No";
            TextBlockFailed[16].Text = Sensor.ProximitySensorFailed ? "Yes" : "No";
            TextBlockFailed[17].Text = Sensor.SimpleOrientationSensorFailed ? "Yes" : "No";
            TextBlockFailed[18].Text = Sensor.OtherSensorFailed ? "Yes" : "No";

            PivotItemSensor.Header = "Summary";
            scrollViewerSensor.Content = stackpanel;
            PivotItemSensor.Content = scrollViewerSensor;
            PivotSensor.Items.Add(PivotItemSensor);
        }

        public void ReportIntervalButton(object sender, RoutedEventArgs e)
        {
            try
            {
                SensorDisplay selected = _sensorDisplay[Sensor.currentId];
                if (selected._sensorType == Sensor.ACCELEROMETER)
                {
                    Sensor.AccelerometerStandardList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ACCELEROMETERLINEAR)
                {
                    Sensor.AccelerometerLinearList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ACCELEROMETERGRAVITY)
                {
                    Sensor.AccelerometerGravityList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                // ActivitySensor doesn't have ReportInterval
                else if (selected._sensorType == Sensor.ALTIMETER)
                {
                    Sensor.Altimeter.ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.BAROMETER)
                {
                    Sensor.BarometerList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.COMPASS)
                {
                    Sensor.CompassList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.GYROMETER)
                {
                    Sensor.GyrometerList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.INCLINOMETER)
                {
                    Sensor.InclinometerList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.LIGHTSENSOR)
                {
                    Sensor.LightSensorList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.MAGNETOMETER)
                {
                    Sensor.MagnetometerList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ORIENTATIONSENSOR)
                {
                    Sensor.OrientationAbsoluteList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ORIENTATIONGEOMAGNETIC)
                {
                    Sensor.OrientationGeomagneticList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.ORIENTATIONRELATIVE)
                {
                    Sensor.OrientationRelativeList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                else if (selected._sensorType == Sensor.PEDOMETER)
                {
                    Sensor.PedometerList[selected._index].ReportInterval = uint.Parse(selected.textboxReportInterval.Text);
                    _sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(selected.textboxReportInterval.Text));
                }
                //ProximitySensor doesn't have ReportInterval
                //SimpleOrientationSensor doesn't have ReportInterval               
            }
            catch { }
        }

        // MALT
        /// <summary>
        /// Initialize device watchers to watch for the Serial Devices.
        /// GetDeviceSelector return an AQS string that can be passed directly into DeviceWatcher.createWatcher() or DeviceInformation.createFromIdAsync(). 
        /// In this sample, a DeviceWatcher will be used to watch for devices because we can detect surprise device removals.
        /// </summary>
        private void InitializeDeviceWatchers()
        {
            string deviceSelector = SerialDevice.GetDeviceSelectorFromUsbVidPid(ArduinoDevice.Vid, ArduinoDevice.Pid);
            var deviceWatcher = DeviceInformation.CreateWatcher(deviceSelector);
            // Allow the EventHandlerForDevice to handle device watcher events that relates or effects our device (i.e. device removal, addition, app suspension/resume)
            AddDeviceWatcher(deviceWatcher, deviceSelector);
        }

        private void StartHandlingAppEvents()
        {
            appSuspendEventHandler = new SuspendingEventHandler(this.OnAppSuspension);
            appResumeEventHandler = new EventHandler<Object>(this.OnAppResume);

            // This event is raised when the app is exited and when the app is suspended
            Application.Current.Suspending += appSuspendEventHandler;
            Application.Current.Resuming += appResumeEventHandler;
        }

        private void StopHandlingAppEvents()
        {
            // This event is raised when the app is exited and when the app is suspended
            Application.Current.Suspending -= appSuspendEventHandler;
            Application.Current.Resuming -= appResumeEventHandler;
        }

        /// <summary>
        /// Registers for Added, Removed, and Enumerated events on the provided deviceWatcher before adding it to an internal list.
        /// </summary>
        private void AddDeviceWatcher(DeviceWatcher deviceWatcher, string deviceSelector)
        {
            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(this.OnDeviceAdded);
            deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(this.OnDeviceRemoved);
            deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(this.OnDeviceEnumerationComplete);
            mapDeviceWatchersToDeviceSelector.Add(deviceWatcher, deviceSelector);
        }

        /// <summary>
        /// Starts all device watchers including ones that have been individually stopped.
        /// </summary>
        private void StartDeviceWatchers()
        {
            // Start all device watchers
            watchersStarted = true;
            isAllDevicesEnumerated = false;

            foreach (DeviceWatcher deviceWatcher in mapDeviceWatchersToDeviceSelector.Keys)
            {
                if ((deviceWatcher.Status != DeviceWatcherStatus.Started) && (deviceWatcher.Status != DeviceWatcherStatus.EnumerationCompleted))
                {
                    deviceWatcher.Start();
                }
            }
        }

        /// <summary>
        /// Stops all device watchers.
        /// </summary>
        private void StopDeviceWatchers()
        {
            // Stop all device watchers
            foreach (DeviceWatcher deviceWatcher in mapDeviceWatchersToDeviceSelector.Keys)
            {
                if ((deviceWatcher.Status == DeviceWatcherStatus.Started) || (deviceWatcher.Status == DeviceWatcherStatus.EnumerationCompleted))
                {
                    deviceWatcher.Stop();
                }
            }

            // Clear the list of devices so we don't have potentially disconnected devices around
            ClearDeviceEntries();

            watchersStarted = false;
        }

        /// <summary>
        /// Creates a DeviceListEntry for a device and adds it to the list of devices in the UI
        /// </summary>
        private void AddDeviceToList(DeviceInformation deviceInformation, string deviceSelector)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            // search the device list for a device with a matching interface ID
            var match = FindDevice(deviceInformation.Id);

            // Add the device if it's new
            if (match == null)
            {
                // Create a new element for this device interface, and queue up the query of its device information
                match = new DeviceListEntry(deviceInformation, deviceSelector);

                // Add the new element to the end of the list of devices
                selected.listOfDevices.Add(match);
            }
        }

        private void RemoveDeviceFromList(string deviceId)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            // Removes the device entry from the interal list; therefore the UI
            var deviceEntry = FindDevice(deviceId);

            selected.listOfDevices.Remove(deviceEntry);
        }

        private void ClearDeviceEntries()
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            selected.listOfDevices.Clear();
        }

        /// <summary>
        /// Searches through the existing list of devices for the first DeviceListEntry that has the specified device Id.
        /// </summary>
        private DeviceListEntry FindDevice(string deviceId)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            if (deviceId != null)
            {
                foreach (DeviceListEntry entry in selected.listOfDevices)
                {
                    if (entry.DeviceInformation.Id == deviceId)
                    {
                        return entry;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// We must stop the DeviceWatchers because device watchers will continue to raise events even if
        /// the app is in suspension, which is not desired (drains battery). We resume the device watcher once the app resumes again.
        /// </summary>
        private void OnAppSuspension(Object sender, SuspendingEventArgs args)
        {
            if (watchersStarted)
            {
                watchersSuspended = true;
                StopDeviceWatchers();
            }
            else
            {
                watchersSuspended = false;
            }
        }

        /// <summary>
        /// See OnAppSuspension for why we are starting the device watchers again
        /// </summary>
        private void OnAppResume(Object sender, Object args)
        {
            if (watchersSuspended)
            {
                watchersSuspended = false;
                StartDeviceWatchers();
            }
        }

        /// <summary>
        /// We will remove the device from the UI
        /// </summary>
        private async void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                rootPage.NotifyUser("Device removed - " + deviceInformationUpdate.Id, NotifyType.StatusMessage);
                RemoveDeviceFromList(deviceInformationUpdate.Id);
            }));
        }

        /// <summary>
        /// This function will add the device to the listOfDevices so that it shows up in the UI
        /// </summary>
        private async void OnDeviceAdded(DeviceWatcher sender, DeviceInformation deviceInformation)
        {
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                rootPage.NotifyUser("Device added - " + deviceInformation.Id, NotifyType.StatusMessage);
                AddDeviceToList(deviceInformation, mapDeviceWatchersToDeviceSelector[sender]);
            }));
        }

        /// <summary>
        /// Notify the UI whether or not we are connected to a device
        /// </summary>
        private async void OnDeviceEnumerationComplete(DeviceWatcher sender, Object args)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                isAllDevicesEnumerated = true;

                // If we finished enumerating devices and the device has not been connected yet, the OnDeviceConnected method
                // is responsible for selecting the device in the device list (UI); otherwise, this method does that.
                if (EventHandlerForDevice.Current.IsDeviceConnected)
                {
                    SelectDeviceInList(EventHandlerForDevice.Current.DeviceInformation.Id);
                    selected.disconnectFromDeviceButton.Content = buttonNameDisconnectFromDevice;

                    if (EventHandlerForDevice.Current.Device.PortName != "")
                    {
                        rootPage.NotifyUser("Connected to - " + EventHandlerForDevice.Current.Device.PortName + " - " +
                                            EventHandlerForDevice.Current.DeviceInformation.Id, NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser("Connected to - " + EventHandlerForDevice.Current.DeviceInformation.Id, NotifyType.StatusMessage);
                    }
                }
                else if (EventHandlerForDevice.Current.IsEnabledAutoReconnect && EventHandlerForDevice.Current.DeviceInformation != null)
                {
                    // We will be reconnecting to a device
                    selected.disconnectFromDeviceButton.Content = buttonNameDisableReconnectToDevice;
                    rootPage.NotifyUser("Waiting to reconnect to device -  " + EventHandlerForDevice.Current.DeviceInformation.Id, NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("No device is currently connected", NotifyType.StatusMessage);
                }
            }));
        }

        /// <summary>
        /// If all the devices have been enumerated, select the device in the list we connected to. Otherwise let the EnumerationComplete event
        /// from the device watcher handle the device selection
        /// </summary>
        private void OnDeviceConnected(EventHandlerForDevice sender, DeviceInformation deviceInformation)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            // Find and select our connected device
            if (isAllDevicesEnumerated)
            {
                SelectDeviceInList(EventHandlerForDevice.Current.DeviceInformation.Id);
                selected.disconnectFromDeviceButton.Content = buttonNameDisconnectFromDevice;
            }

            if (EventHandlerForDevice.Current.Device.PortName != "")
            {
                rootPage.NotifyUser("Connected to - " + EventHandlerForDevice.Current.Device.PortName + " - " +
                                    EventHandlerForDevice.Current.DeviceInformation.Id, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Connected to - " + EventHandlerForDevice.Current.DeviceInformation.Id, NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// The device was closed. If we will autoreconnect to the device, reflect that in the UI
        /// </summary>
        private async void OnDeviceClosing(EventHandlerForDevice sender, DeviceInformation deviceInformation)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                // We were connected to the device that was unplugged, so change the "Disconnect from device" button
                // to "Do not reconnect to device"
                if (selected.disconnectFromDeviceButton.IsEnabled && EventHandlerForDevice.Current.IsEnabledAutoReconnect)
                {
                    selected.disconnectFromDeviceButton.Content = buttonNameDisableReconnectToDevice;
                }
            }));
        }

        /// <summary>
        /// Selects the item in the UI's listbox that corresponds to the provided device id. If there are no
        /// matches, we will deselect anything that is selected.
        /// </summary>
        private void SelectDeviceInList(string deviceIdToSelect)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            // Don't select anything by default.
            connectDevices.SelectedIndex = -1;

            for (int deviceListIndex = 0; deviceListIndex < selected.listOfDevices.Count; deviceListIndex++)
            {
                if (selected.listOfDevices[deviceListIndex].DeviceInformation.Id == deviceIdToSelect)
                {
                    connectDevices.SelectedIndex = deviceListIndex;
                    break;
                }
            }
        }

        /// <summary>
        /// When ButtonConnectToDevice is disabled, ConnectDevices list will also be disabled.
        /// </summary>
        private void UpdateConnectDisconnectButtonsAndList(Boolean enableConnectButton)
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            selected.connectToDeviceButton.IsEnabled = enableConnectButton;
            selected.disconnectFromDeviceButton.IsEnabled = !selected.connectToDeviceButton.IsEnabled;
            connectDevices.IsEnabled = selected.connectToDeviceButton.IsEnabled;
        }

        public async void GetMALTData()
        {
            SensorDisplay selected = _sensorDisplay[Sensor.currentId];

            await WriteCommandAsync("READCOLORSENSOR 1\n");
            string[] result = await ReadColorSensor("READCOLORSENSOR 1\n");
            if (result != null && result.Length == 5)
            {
                selected.textBlockMALTPropertyValue1[3].Text = result[1];
                selected.textBlockMALTPropertyValue1[4].Text = result[2];
                selected.textBlockMALTPropertyValue1[5].Text = result[3];
                selected.textBlockMALTPropertyValue1[6].Text = result[4];
            }

            await WriteCommandAsync("READCOLORSENSOR 2\n");
            string[] result2 = await ReadColorSensor("READCOLORSENSOR 2\n");
            if (result2 != null && result.Length == 5)
            {
                selected.textBlockMALTPropertyValue2[3].Text = result2[1];
                selected.textBlockMALTPropertyValue2[4].Text = result2[2];
                selected.textBlockMALTPropertyValue2[5].Text = result2[3];
                selected.textBlockMALTPropertyValue2[6].Text = result2[4];
            }
        }

        private async Task WriteCommandAsync(string command)
        {
            if (EventHandlerForDevice.Current.IsDeviceConnected)
            {
                try
                {
                    DataWriterObject = new DataWriter(EventHandlerForDevice.Current.Device.OutputStream);
                    DataWriterObject.WriteString(command);
                    uint x = await DataWriterObject.StoreAsync().AsTask();
                    DataWriterObject.DetachStream();
                    DataWriterObject = null;
                }
                catch { }
            }
        }

        private async Task<string[]> ReadColorSensor(string command)
        {
            try
            {
                string data = await ReadLines(5);
                data = data.Replace("\n", "");
                string[] delim = new string[1] { "\r" };
                string[] split = data.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                //OutputError(command, split[0]);

                return split;
            }
            catch { return new string[] { }; }
        }

        private async Task<string> ReadLines(int numLines)
        {
            int newLines = 0;
            string data = string.Empty;

            DataReaderObject = new DataReader(EventHandlerForDevice.Current.Device.InputStream);
            while (newLines != numLines)
            {
                uint x = await DataReaderObject.LoadAsync(1);
                string s = DataReaderObject.ReadString(1);
                data += s;
                if (s == "\n")
                {
                    newLines++;
                }
            }
            DataReaderObject.DetachStream();
            DataReaderObject = null;

            return data;
        }
    }
}