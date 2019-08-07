// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SensorExplorer
{
    public sealed partial class Scenario2MALT : Page
    {
        public static Scenario2MALT Scenario2;

        private const string buttonNameDisableReconnectToDevice = "Do not automatically reconnect to device that was just closed";
        private const string buttonNameDisconnectFromDevice = "Disconnect from device";

        private bool isAllDevicesEnumerated;
        private bool IsNavigatedAway;
        private bool watchersStarted;
        private bool watchersSuspended;
        private DataReader DataReaderObject = null;
        private DataWriter DataWriterObject = null;
        private Dictionary<DeviceWatcher, string> mapDeviceWatchersToDeviceSelector;
        private EventHandler<object> appResumeEventHandler;
        private LightSensor lightSensor;
        private List<string> conversionValues = new List<string> { "100", "800" };
        private MainPage rootPage = MainPage.Current;
        private ObservableCollection<DeviceListEntry> listOfDevices;
        private StorageFile tmp, file;  
        private SuspendingEventHandler appSuspendEventHandler;

        // MALTERROR
        private const int E_SUCCESS = 0;
        private const int E_INVALID_PARAM = 1;
        private const int E_UNRECOGNIZED_COMMAND = 2;

        // Track Read Operation
        private CancellationTokenSource ReadCancellationTokenSource;
        private object ReadCancelLock = new object();

        // Track Write Operation
        private CancellationTokenSource WriteCancellationTokenSource;
        private object WriteCancelLock = new object();

        public Scenario2MALT()
        {
            InitializeComponent();
            Scenario2 = this;

            comboBox.ItemsSource = conversionValues;
            comboBox.SelectionChanged += OnSelectionChanged;

            listOfDevices = new ObservableCollection<DeviceListEntry>();
            mapDeviceWatchersToDeviceSelector = new Dictionary<DeviceWatcher, string>();
            watchersStarted = false;
            watchersSuspended = false;
            isAllDevicesEnumerated = false;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// Create the DeviceWatcher objects when the user navigates to this page so the UI list of devices is populated.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            // If we are connected to the device or planning to reconnect, we should disable the list of devices
            // to prevent the user from opening a device without explicitly closing or disabling the auto reconnect
            if (EventHandlerForDevice.Current.IsDeviceConnected
                || (EventHandlerForDevice.Current.IsEnabledAutoReconnect
                && EventHandlerForDevice.Current.DeviceInformation != null))
            {
                UpdateConnectDisconnectButtonsAndList(false);

                // These notifications will occur if we are waiting to reconnect to device when we start the page
                EventHandlerForDevice.Current.OnDeviceConnected = OnDeviceConnected;
                EventHandlerForDevice.Current.OnDeviceClose = OnDeviceClosing;
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

            DeviceListSource.Source = listOfDevices;
        }

        /// <summary>
        /// Unregister from App events and DeviceWatcher events because this page will be unloaded.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs eventArgs)
        {
            if(lightSensor != null)
            {
                lightSensor.ReadingChanged -= LightSensorReadingChanged;
            }
            PeriodicTimer.Cancel2();

            DisconnectFromDeviceClick(null, null);

            StopDeviceWatchers();
            StopHandlingAppEvents();

            // We no longer care about the device being connected
            EventHandlerForDevice.Current.OnDeviceConnected = null;
            EventHandlerForDevice.Current.OnDeviceClose = null;

            IsNavigatedAway = true;
            CancelAllIoTasks();
        }

        private async void initialize()
        {
            if (EventHandlerForDevice.Current.Device == null)
            {
                stackpanel2.Visibility = Visibility.Collapsed;
                MainPage.Current.NotifyUser("Device is not connected", NotifyType.ErrorMessage);
            }
            else
            {
                MainPage.Current.NotifyUser("Connected to " + EventHandlerForDevice.Current.DeviceInformation.Id, NotifyType.StatusMessage);

                // So we can reset future tasks
                ResetReadCancellationTokenSource();
                ResetWriteCancellationTokenSource();
            }

            DeviceInformationCollection deviceInfoCollection = await DeviceInformation.FindAllAsync(LightSensor.GetDeviceSelector(), Constants.RequestedProperties);
            foreach (DeviceInformation deviceInfo in deviceInfoCollection)
            {
                lightSensor = await LightSensor.FromIdAsync(deviceInfo.Id);
            }
        }

        private async void ConnectToDeviceClick(object sender, RoutedEventArgs eventArgs)
        {
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
                    bool openSuccess = await EventHandlerForDevice.Current.OpenDeviceAsync(entry.DeviceInformation, entry.DeviceSelector);

                    // Disable connect button if we connected to the device
                    UpdateConnectDisconnectButtonsAndList(!openSuccess);

                    if (openSuccess)
                    {
                        stackpanel1.Visibility = Visibility.Collapsed;
                        initialize();
                        stackpanel2.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void ShowConnectionButton(object sender, RoutedEventArgs eventArgs)
        {
            stackpanel2.Visibility = Visibility.Collapsed;
            stackpanel1.Visibility = Visibility.Visible;
        }

        private void DisconnectFromDeviceClick(object sender, RoutedEventArgs eventArgs)
        {
            var selection = connectDevices.SelectedItems;
            DeviceListEntry entry = null;

            // Prevent auto reconnect because we are voluntarily closing it
            // Re-enable the ConnectDevice list and ConnectToDevice button if the connected/opened device was removed.
            EventHandlerForDevice.Current.IsEnabledAutoReconnect = false;

            if (selection.Count > 0)
            {
                var obj = selection[0];
                entry = (DeviceListEntry)obj;

                if (entry != null)
                {
                    EventHandlerForDevice.Current.CloseDevice();
                }
            }

            UpdateConnectDisconnectButtonsAndList(true);
        }

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
            appSuspendEventHandler = new SuspendingEventHandler(OnAppSuspension);
            appResumeEventHandler = new EventHandler<object>(OnAppResume);

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
            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(OnDeviceAdded);
            deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(OnDeviceRemoved);
            deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, object>(OnDeviceEnumerationComplete);
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
            // search the device list for a device with a matching interface ID
            var match = FindDevice(deviceInformation.Id);

            // Add the device if it's new
            if (match == null)
            {
                // Create a new element for this device interface, and queue up the query of its device information
                match = new DeviceListEntry(deviceInformation, deviceSelector);

                // Add the new element to the end of the list of devices
                listOfDevices.Add(match);
            }
        }

        private void RemoveDeviceFromList(string deviceId)
        {
            // Removes the device entry from the interal list; therefore the UI
            var deviceEntry = FindDevice(deviceId);

            listOfDevices.Remove(deviceEntry);
        }

        private void ClearDeviceEntries()
        {
            listOfDevices.Clear();
        }

        /// <summary>
        /// Searches through the existing list of devices for the first DeviceListEntry that has the specified device Id.
        /// </summary>
        private DeviceListEntry FindDevice(string deviceId)
        {
            if (deviceId != null)
            {
                foreach (DeviceListEntry entry in listOfDevices)
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
        private void OnAppSuspension(object sender, SuspendingEventArgs args)
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
        private void OnAppResume(object sender, object args)
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
        private async void OnDeviceEnumerationComplete(DeviceWatcher sender, object args)
        {
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                isAllDevicesEnumerated = true;

                // If we finished enumerating devices and the device has not been connected yet, the OnDeviceConnected method
                // is responsible for selecting the device in the device list (UI); otherwise, this method does that.
                if (EventHandlerForDevice.Current.IsDeviceConnected)
                {
                    SelectDeviceInList(EventHandlerForDevice.Current.DeviceInformation.Id);
                    disconnectFromDeviceButton.Content = buttonNameDisconnectFromDevice;

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
                    disconnectFromDeviceButton.Content = buttonNameDisableReconnectToDevice;
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
            // Find and select our connected device
            if (isAllDevicesEnumerated)
            {
                SelectDeviceInList(EventHandlerForDevice.Current.DeviceInformation.Id);
                disconnectFromDeviceButton.Content = buttonNameDisconnectFromDevice;
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
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                // We were connected to the device that was unplugged, so change the "Disconnect from device" button
                // to "Do not reconnect to device"
                if (disconnectFromDeviceButton.IsEnabled && EventHandlerForDevice.Current.IsEnabledAutoReconnect)
                {
                    disconnectFromDeviceButton.Content = buttonNameDisableReconnectToDevice;
                }
            }));
        }

        /// <summary>
        /// Selects the item in the UI's listbox that corresponds to the provided device id. If there are no
        /// matches, we will deselect anything that is selected.
        /// </summary>
        private void SelectDeviceInList(string deviceIdToSelect)
        {
            // Don't select anything by default.
            connectDevices.SelectedIndex = -1;

            for (int deviceListIndex = 0; deviceListIndex < listOfDevices.Count; deviceListIndex++)
            {
                if (listOfDevices[deviceListIndex].DeviceInformation.Id == deviceIdToSelect)
                {
                    connectDevices.SelectedIndex = deviceListIndex;
                    break;
                }
            }
        }

        /// <summary>
        /// When ButtonConnectToDevice is disabled, ConnectDevices list will also be disabled.
        /// </summary>
        private void UpdateConnectDisconnectButtonsAndList(bool enableConnectButton)
        {
            connectToDeviceButton.IsEnabled = enableConnectButton;
            disconnectFromDeviceButton.IsEnabled = !connectToDeviceButton.IsEnabled;
            connectDevices.IsEnabled = connectToDeviceButton.IsEnabled;
        }

        private async void ButtonLIGHT(object sender, RoutedEventArgs e)
        {
            if (textboxLIGHT.Text.Length != 0)
            {
                buttonLIGHT.IsEnabled = false;

                char[] buffer = new char[textboxLIGHT.Text.Length];
                textboxLIGHT.Text.CopyTo(0, buffer, 0, textboxLIGHT.Text.Length);
                uint lightLevel;
                try
                {
                    lightLevel = Convert.ToUInt32(new string(buffer));
                }
                catch
                {
                    rootPage.NotifyUser("Please enter a valid value.", NotifyType.ErrorMessage);
                    buttonLIGHT.IsEnabled = true;
                    return;
                }

                await SetLight(lightLevel);
                buttonLIGHT.IsEnabled = true;
            }
        }

        private async Task SetLight(uint lightLevel)
        {
            if (lightLevel > 2600)
            {
                rootPage.NotifyUser("Light panel only supports values from 0 to 2600.", NotifyType.ErrorMessage);
                return;
            }

            string command = "LIGHT " + lightLevel + "\n";
            await WriteCommandAsync(command);
            await ReadErrorCode(command);
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedValue != null)
            {
                await SetConversionTime(Convert.ToUInt32(comboBox.SelectedValue));
            }
        }

        private async Task SetConversionTime(uint conversionTime)
        {
            string command = "CONVERSIONTIME " + conversionTime + "\n";
            await WriteCommandAsync(command);
            await ReadErrorCode(command);
        }

        private async void ButtonMALTVERSION(object sender, RoutedEventArgs e)
        {
            buttonMALTVERSION.IsEnabled = false;

            string command = "MALTVERSION\n";
            await WriteCommandAsync(command);
            await ReadVersion(command);

            buttonMALTVERSION.IsEnabled = true;
        }

        // get ambient lux
        private async void ButtonREADALSSENSOR1(object sender, RoutedEventArgs e)
        {
            buttonREADALSSENSOR1.IsEnabled = false;

            string command = "READALSSENSOR 1\n";
            await WriteCommandAsync(command);
            double value = await ReadLightSensor(command);
            textblockLightSensor1.Text = value + " Lux";

            buttonREADALSSENSOR1.IsEnabled = true;
        }

        // get screen lux
        private async void ButtonREADALSSENSOR2(object sender, RoutedEventArgs e)
        {
            buttonREADALSSENSOR2.IsEnabled = false;

            string command = "READALSSENSOR 2\n";
            await WriteCommandAsync(command);
            double value = await ReadLightSensor(command);
            textblockLightSensor2.Text = value + " Lux";

            buttonREADALSSENSOR2.IsEnabled = true;
        }

        // get ambient RGB
        private async void ButtonREADCOLORSENSOR1(object sender, RoutedEventArgs e)
        {
            buttonREADCOLORSENSOR1.IsEnabled = false;

            string command = "READCOLORSENSOR 1\n";
            await WriteCommandAsync(command);
            string[] result = await ReadColorSensor(command);
            textblockColorSensor1.Text = "Clear: " + result[1] +
                                         ", Red: " + result[2] +
                                         ", Green: " + result[3] +
                                         ", Blue: " + result[4];

            buttonREADCOLORSENSOR1.IsEnabled = true;
        }

        // get screen RGB
        private async void ButtonREADCOLORSENSOR2(object sender, RoutedEventArgs e)
        {
            buttonREADCOLORSENSOR2.IsEnabled = false;

            string command = "READCOLORSENSOR 2\n";
            await WriteCommandAsync(command);
            string[] result = await ReadColorSensor(command);
            textblockColorSensor2.Text = "Clear: " + result[1] +
                                    ", Red: " + result[2] +
                                    ", Green: " + result[3] +
                                    ", Blue: " + result[4];

            buttonREADCOLORSENSOR2.IsEnabled = true;
        }

        private void ButtonInternalExternal(object sender, RoutedEventArgs e)
        {
            stackpanel2.Visibility = Visibility.Collapsed;
            stackpanel4.Visibility = Visibility.Visible;

            if (lightSensor != null)
            {
                lightSensor.ReadingChanged += LightSensorReadingChanged;
            }

            PeriodicTimer.Create();
        }

        public async void GetMALTData()
        {
            await WriteCommandAsync("READALSSENSOR 1\n");
            double ambientLux = await ReadLightSensor("READALSSENSOR 1\n");
            textblockLux2.Text = ((int)ambientLux).ToString();

            await WriteCommandAsync("READCOLORSENSOR 1\n");
            string[] result = await ReadColorSensor("READCOLORSENSOR 1\n");
            if (result != null && result.Length == 5)
            {
                textblockClear2.Text = result[1];
                textblockR2.Text = result[2];
                textblockG2.Text = result[3];
                textblockB2.Text = result[4];

                /*
                double[] RGB = new double[] { Convert.ToDouble(result[2]), Convert.ToDouble(result[3]), Convert.ToDouble(result[4]) };
                double[] XYZ = RGBToXYZ(RGB, redPrimary, greenPrimary, bluePrimary, white);
                double[] xyY = XYZToxyY(XYZ);
                textblockChromaticityx2.Text = xyY[0].ToString();
                textblockChromaticityy2.Text = xyY[1].ToString();
                textblockChromaticityY2.Text = xyY[2].ToString();
                */
            }

            await WriteCommandAsync("READALSSENSOR 2\n");
            double screenLux = await ReadLightSensor("READALSSENSOR 2\n");
            textblockLux3.Text = ((int)screenLux).ToString();

            await WriteCommandAsync("READCOLORSENSOR 2\n");
            string[] result2 = await ReadColorSensor("READCOLORSENSOR 2\n");
            if (result2 != null && result.Length == 5)
            {
                textblockClear3.Text = result2[1];
                textblockR3.Text = result2[2];
                textblockG3.Text = result2[3];
                textblockB3.Text = result2[4];

                /*
                double[] RGB = new double[] { Convert.ToDouble(result2[2]), Convert.ToDouble(result2[3]), Convert.ToDouble(result2[4]) };
                double[] XYZ = RGBToXYZ(RGB, redPrimary, greenPrimary, bluePrimary, white);
                double[] xyY = XYZToxyY(XYZ);
                textblockChromaticityx3.Text = xyY[0].ToString();
                textblockChromaticityy3.Text = xyY[1].ToString();
                textblockChromaticityY3.Text = xyY[2].ToString();
                */
            }
        }

        private async void LightSensorReadingChanged(object sender, LightSensorReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                LightSensorReading reading = e.Reading;
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

                textblockLux.Text = reading.IlluminanceInLux.ToString();
                textblockChromaticityx.Text = chromaticity_x.ToString();
                textblockChromaticityy.Text = chromaticity_y.ToString();
            });
        }

        private void ButtonBackToMenu(object sender, RoutedEventArgs e)
        {
            if (lightSensor != null)
            {
                lightSensor.ReadingChanged -= LightSensorReadingChanged;
            }

            PeriodicTimer.Cancel2();

            stackpanel4.Visibility = Visibility.Collapsed;
            stackpanel2.Visibility = Visibility.Visible;
        }

        // get auto-brightness curve
        private async void ButtonAUTOCURVE(object sender, RoutedEventArgs e)
        {
            stackpanel2.Visibility = Visibility.Collapsed;
            stackpanel3.Visibility = Visibility.Visible;
            rootPage.DisableScenarioSelect();

            output.Text = "Please specify where the .csv file will be saved to.";
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("csv", new List<string>() { ".csv" });
            savePicker.SuggestedFileName = "AutoCurve";
            file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                tmp = await storageFolder.CreateFileAsync("tmp.csv", CreationCollisionOption.ReplaceExisting);

                stackpanelWaitTime1.Visibility = Visibility.Visible;
                output.Text = "Please specify the wait time (in seconds) before test begins.";
            }
            else
            {
                stackpanel3.Visibility = Visibility.Collapsed;
                stackpanel2.Visibility = Visibility.Visible;
            }
        }

        private async void ButtonAUTOCURVE2(object sender, RoutedEventArgs e)
        {
            stackpanelWaitTime1.Visibility = Visibility.Collapsed;

            if (textboxAUTOCURVE.Text.Length != 0)
            {
                char[] buffer = new char[textboxAUTOCURVE.Text.Length];
                textboxAUTOCURVE.Text.CopyTo(0, buffer, 0, textboxAUTOCURVE.Text.Length);
                double waitTime = 0;
                try
                {
                    waitTime = Convert.ToDouble(new string(buffer));
                }
                catch
                {
                    rootPage.NotifyUser("Please enter a valid value.", NotifyType.ErrorMessage);
                    stackpanelWaitTime1.Visibility = Visibility.Visible;
                    return;
                }

                output.Text = "Preparation time...";
                await SetLight(0);
                await Task.Delay((int)waitTime * 1000);
            }

            double ambientLux1 = 0, ambientLux2 = 0, ambientLuxCurrent = 0;
            double screenLux1 = 0, screenLux2 = 0, screenLuxCurrent = 0;

            try
            {
                var csv = new System.Text.StringBuilder();
                await SetConversionTime(100);
                csv.AppendLine("Light Level,Ambient Lux,Screen Lux");

                output.Text = "Auto Brightness test running...";

                // Move the light up in increments of 5 to speed the test up further.
                for (uint i = 500; i <= 2600; i += 5)
                {
                    await Task.Delay(150);
                    await SetLight(i);

                    await Task.Delay(150);
                    ambientLux1 = await GetAmbientLux();
                    screenLux1 = await GetScreenLux();

                    await Task.Delay(150);
                    ambientLux2 = await GetAmbientLux();
                    screenLux2 = await GetScreenLux();

                    await Task.Delay(150);
                    ambientLuxCurrent = await GetAmbientLux();
                    screenLuxCurrent = await GetScreenLux();

                    while (screenLux1 != screenLux2 || screenLux2 != screenLuxCurrent)
                    {
                        ambientLux1 = ambientLux2;
                        screenLux1 = screenLux2;

                        ambientLux2 = ambientLuxCurrent;
                        screenLux2 = screenLuxCurrent;

                        await Task.Delay(150);
                        ambientLuxCurrent = await GetAmbientLux();
                        screenLuxCurrent = await GetScreenLux();
                    }

                    csv.AppendLine(string.Format("{0},{1},{2}", i, ambientLuxCurrent, screenLuxCurrent));
                }

                await Task.Run(() => { File.WriteAllText(tmp.Path, csv.ToString()); });
                await tmp.CopyAndReplaceAsync(file);

                output.Text = "Successfully written data to " + file.Path.ToString();
                restartButton.Visibility = Visibility.Visible;

                var mediaElement = new MediaElement();
                var folder = await Package.Current.InstalledLocation.GetFolderAsync("Music");
                var wav = await folder.GetFileAsync("Alarm05.wav");
                var stream = await wav.OpenAsync(FileAccessMode.Read);
                mediaElement.SetSource(stream, "");
                mediaElement.Play();

                rootPage.EnableScenarioSelect();
            }
            catch (Exception exception)
            {
                output.Text = exception.Message;
                rootPage.EnableScenarioSelect();
            }
        }

        private void RestartButton(object sender, RoutedEventArgs e)
        {
            restartButton.Visibility = Visibility.Collapsed;
            stackpanel3.Visibility = Visibility.Collapsed;
            stackpanel2.Visibility = Visibility.Visible;
        }

        private async Task<double> GetAmbientLux()
        {
            string command = "READALSSENSOR 1\n";
            await WriteCommandAsync(command);
            double value = await ReadLightSensor(command);

            return value;
        }

        private async Task<double> GetScreenLux()
        {
            string command = "READALSSENSOR 2\n";
            await WriteCommandAsync(command);
            double value = await ReadLightSensor(command);

            return value;
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

        private async Task<double> ReadLightSensor(string command)
        {
            try
            {
                string data = await ReadLines(3);
                data = data.Replace("\n", "");
                string[] delim = new string[1] { "\r" };
                string[] split = data.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                OutputError(command, split[0]);

                return RawToLux(Convert.ToInt32(split[2]), Convert.ToInt32(split[1]));
            }
            catch { return -1; }
        }

        private async Task<string[]> ReadColorSensor(string command)
        {
            try
            {
                string data = await ReadLines(5);
                data = data.Replace("\n", "");
                string[] delim = new string[1] { "\r" };
                string[] split = data.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                OutputError(command, split[0]);

                return split;
            }
            catch { return new string[] { }; }
        }

        private async Task ReadVersion(string command)
        {
            string data = await ReadLines(2);
            data = data.Replace("\n", "");
            string[] delim = new string[1] { "\r" };
            string[] split = data.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            OutputError(command, split[0]);
            textblockVersion.Text = "Version: " + split[1];
        }

        private async Task ReadErrorCode(string command)
        {
            string data = await ReadLines(1);
            data = data.Replace("\n", "");
            data = data.Replace("\r", "");
            OutputError(command, data);
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

        private void OutputError(string command, string data)
        {
            if (data.Trim() == E_SUCCESS.ToString())
            {
                if (command.Contains("LIGHT"))
                {
                    rootPage.NotifyUser("Success - Light set to the input value", NotifyType.StatusMessage);
                }
                else if (command.Contains("CONVERSIONTIME"))
                {
                    rootPage.NotifyUser("Success - Conversion time set to the input value", NotifyType.StatusMessage);
                }
                else if (command.Contains("MALTVERSION"))
                {
                    rootPage.NotifyUser("Success - Get version", NotifyType.StatusMessage);
                }
                else if (command.Contains("READALSSENSOR"))
                {
                    rootPage.NotifyUser("Success - Reading sensor value...", NotifyType.StatusMessage);
                }
            }
            else if (data.Trim() == E_INVALID_PARAM.ToString())
            {
                rootPage.NotifyUser("Error: Invalid Parameter", NotifyType.ErrorMessage);
            }
            else if (data.Trim() == E_UNRECOGNIZED_COMMAND.ToString())
            {
                rootPage.NotifyUser("Error: Unrecognized command", NotifyType.ErrorMessage);
            }
        }

        private double RawToLux(int result, int exponent)
        {
            // Formula to convert raw sensor output to lux is defined in the OPT 3001 spec.
            // If you are using a different part, this calculation will be different.
            double lsbSize = .01 * (Math.Pow(2, exponent));
            return lsbSize * result;
        }

        /// It is important to be able to cancel tasks that may take a while to complete. Cancelling tasks is the only way to stop any pending IO
        /// operations asynchronously. If the Serial Device is closed/deleted while there are pending IOs, the destructor will cancel all pending IO 
        /// operations.
        private void CancelReadTask()
        {
            lock (ReadCancelLock)
            {
                if (ReadCancellationTokenSource != null)
                {
                    if (!ReadCancellationTokenSource.IsCancellationRequested)
                    {
                        ReadCancellationTokenSource.Cancel();

                        // Existing IO already has a local copy of the old cancellation token so this reset won't affect it
                        ResetReadCancellationTokenSource();
                    }
                }
            }
        }

        private void CancelWriteTask()
        {
            lock (WriteCancelLock)
            {
                if (WriteCancellationTokenSource != null)
                {
                    if (!WriteCancellationTokenSource.IsCancellationRequested)
                    {
                        WriteCancellationTokenSource.Cancel();

                        // Existing IO already has a local copy of the old cancellation token so this reset won't affect it
                        ResetWriteCancellationTokenSource();
                    }
                }
            }
        }

        private void CancelAllIoTasks()
        {
            CancelReadTask();
            CancelWriteTask();
        }

        private void ResetReadCancellationTokenSource()
        {
            // Create a new cancellation token source so that can cancel all the tokens again
            ReadCancellationTokenSource = new CancellationTokenSource();

            // Hook the cancellation callback (called whenever Task.cancel is called)
            ReadCancellationTokenSource.Token.Register(() => NotifyReadCancelingTask());
        }

        private void ResetWriteCancellationTokenSource()
        {
            // Create a new cancellation token source so that can cancel all the tokens again
            WriteCancellationTokenSource = new CancellationTokenSource();

            // Hook the cancellation callback (called whenever Task.cancel is called)
            WriteCancellationTokenSource.Token.Register(() => NotifyWriteCancelingTask());
        }

        /// <summary>
        /// Print a status message saying we are canceling a task and disable all buttons to prevent multiple cancel requests.
        /// <summary>
        private async void NotifyReadCancelingTask()
        {
            // Setting the dispatcher priority to high allows the UI to handle disabling of all the buttons
            // before any of the IO completion callbacks get a chance to modify the UI; that way this method
            // will never get the opportunity to overwrite UI changes made by IO callbacks
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
            {
                if (!IsNavigatedAway)
                {
                    rootPage.NotifyUser("Canceling Read... Please wait...", NotifyType.StatusMessage);
                }
            }));
        }

        private async void NotifyWriteCancelingTask()
        {
            // Setting the dispatcher priority to high allows the UI to handle disabling of all the buttons
            // before any of the IO completion callbacks get a chance to modify the UI; that way this method
            // will never get the opportunity to overwrite UI changes made by IO callbacks
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
            {
                if (!IsNavigatedAway)
                {
                    rootPage.NotifyUser("Canceling Write... Please wait...", NotifyType.StatusMessage);
                }
            }));
        }

        private double[] XYZToxyY(double[] XYZ)
        {
            double[] xyY = new double[3];
            xyY[0] = XYZ[0] / (XYZ[0] + XYZ[1] + XYZ[2]);
            xyY[1] = XYZ[1] / (XYZ[0] + XYZ[1] + XYZ[2]);
            xyY[2] = XYZ[2];

            return xyY;
        }

        // white is in XYZ
        private double[] RGBToXYZ(double[] RGB, double[] redPrimary, double[] greenPrimary, double[] bluePrimary, double[] white)
        {
            double Xr = redPrimary[0] / redPrimary[1];
            double Yr = 1.0;
            double Zr = (1.0 - redPrimary[0] - redPrimary[1]) / redPrimary[1];
            double Xg = greenPrimary[0] / greenPrimary[1];
            double Yg = 1.0;
            double Zg = (1.0 - greenPrimary[0] - greenPrimary[1]) / greenPrimary[1];
            double Xb = bluePrimary[0] / bluePrimary[1];
            double Yb = 1.0;
            double Zb = (1.0 - bluePrimary[0] - bluePrimary[1]) / bluePrimary[1];

            double determinant = (Xr * Yg * Zb) + (Xg * Yb * Zr) + (Xb * Yr * Zg) - (Xb * Yg * Zr) - (Xg * Yr * Zb) - (Xr * Yb * Zg);

            //                      -1
            // | Sr |   | Xr Xg Xb |   | Xw | 
            // | Sg | = | Yr Yg Yb |   | Yw |
            // | Sb |   | Zr Zg Zb |   | Zw |
            // inv12 means first row second column of the inverse matrix
            double inv11 = 1.0 / determinant * ((Yg * Zb) - (Yb * Zg));
            double inv12 = 1.0 / determinant * ((Xb * Zg) - (Xg * Zb));
            double inv13 = 1.0 / determinant * ((Xg * Yb) - (Xb * Yg));
            double inv21 = 1.0 / determinant * ((Yb * Zr) - (Yr * Zb));
            double inv22 = 1.0 / determinant * ((Xr * Zb) - (Xb * Zr));
            double inv23 = 1.0 / determinant * ((Xb * Yr) - (Xr * Yb));
            double inv31 = 1.0 / determinant * ((Yr * Zg) - (Yg * Zr));
            double inv32 = 1.0 / determinant * ((Xg * Zr) - (Xr * Zg));
            double inv33 = 1.0 / determinant * ((Xr * Yg) - (Xg * Yr));

            double Sr = (inv11 * white[0]) + (inv12 * white[1]) + (inv13 * white[2]);
            double Sg = (inv21 * white[0]) + (inv22 * white[1]) + (inv23 * white[2]);
            double Sb = (inv31 * white[0]) + (inv32 * white[1]) + (inv33 * white[2]);

            //       | M11 M12 M13 |   | SrXr SgXg SbXb |
            // [M] = | M21 M22 M23 | = | SrYr SgYg SbYb |
            //       | M31 M32 M33 |   | SrZr SgZg SbZb |
            double M11 = Sr * Xr;
            double M12 = Sg * Xg;
            double M13 = Sb * Xb;
            double M21 = Sr * Yr;
            double M22 = Sg * Yg;
            double M23 = Sb * Yb;
            double M31 = Sr * Zr;
            double M32 = Sg * Zg;
            double M33 = Sb * Zb;

            // | X |       | R |
            // | Y | = [M] | G |
            // | Z |       | B |
            double[] XYZ = new double[3];
            XYZ[0] = M11 * RGB[0] + M12 * RGB[1] + M13 * RGB[2];
            XYZ[1] = M21 * RGB[0] + M22 * RGB[1] + M23 * RGB[2];
            XYZ[2] = M31 * RGB[0] + M32 * RGB[1] + M33 * RGB[2];

            return XYZ;
        }
    }
}