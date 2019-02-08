// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SensorExplorer
{
    /// <summary>
    /// The purpose of this class is to demonstrate the expected application behavior for app events 
    /// such as suspension and resume or when the device is disconnected. 
    /// 
    /// This class will also demonstrate how to handle device watcher events.
    /// 
    /// For simplicity, this class will only allow at most one device to be connected at any given time.
    /// </summary>
    public class EventHandlerForDevice
    {
        // Allows for singleton EventHandlerForDevice
        private static EventHandlerForDevice eventHandlerForDevice;

        // Used to synchronize threads to avoid multiple instantiations of eventHandlerForDevice.
        private static object singletonCreationLock = new Object();

        private DeviceWatcher deviceWatcher;
        private SuspendingEventHandler appSuspendEventHandler;
        private EventHandler<object> appResumeEventHandler;
        private TypedEventHandler<DeviceWatcher, DeviceInformation> deviceAddedEventHandler;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> deviceRemovedEventHandler;
        private TypedEventHandler<DeviceAccessInformation, DeviceAccessChangedEventArgs> deviceAccessEventHandler;
        private bool watcherSuspended;
        private bool watcherStarted;
        private MainPage rootPage = MainPage.Current;

        /// <summary>
        /// Enforces the singleton pattern so that there is only one object handling app events
        /// as it relates to the SerialDevice because this sample app only supports communicating with one device at a time. 
        ///
        /// An instance of EventHandlerForDevice is globally available because the device needs to persist across scenario pages.
        ///
        /// If there is no instance of EventHandlerForDevice created before this property is called,
        /// an EventHandlerForDevice will be created.
        /// </summary>
        public static EventHandlerForDevice Current
        {
            get
            {
                if (eventHandlerForDevice == null)
                {
                    lock (singletonCreationLock)
                    {
                        if (eventHandlerForDevice == null)
                        {
                            CreateNewEventHandlerForDevice();
                        }
                    }
                }

                return eventHandlerForDevice;
            }
        }

        /// <summary>
        /// Creates a new instance of EventHandlerForDevice, enables auto reconnect, and uses it as the Current instance.
        /// </summary>
        public static void CreateNewEventHandlerForDevice()
        {
            eventHandlerForDevice = new EventHandlerForDevice();
        }

        public TypedEventHandler<EventHandlerForDevice, DeviceInformation> OnDeviceClose { get; set; }

        public TypedEventHandler<EventHandlerForDevice, DeviceInformation> OnDeviceConnected { get; set; }

        public bool IsDeviceConnected
        {
            get
            {
                return (Device != null);
            }
        }

        public SerialDevice Device { get; private set; }

        /// <summary>
        /// This DeviceInformation represents which device is connected or which device will be reconnected when
        /// the device is plugged in again (if IsEnabledAutoReconnect is true);.
        /// </summary>
        public DeviceInformation DeviceInformation { get; private set; }

        /// <summary>
        /// Returns DeviceAccessInformation for the device that is currently connected using this EventHandlerForDevice object.
        /// </summary>
        public DeviceAccessInformation DeviceAccessInformation { get; private set; }

        /// <summary>
        /// DeviceSelector AQS used to find this device
        /// </summary>
        public string DeviceSelector { get; private set; }

        /// <summary>
        /// True if EventHandlerForDevice will attempt to reconnect to the device once it is plugged into the computer again
        /// </summary>
        public bool IsEnabledAutoReconnect { get; set; }

        /// <summary>
        /// This method opens the device using the WinRT Serial API. After the device is opened, save the device
        /// so that it can be used across scenarios.
        ///
        /// It is important that the FromIdAsync call is made on the UI thread because the consent prompt can only be displayed
        /// on the UI thread.
        /// 
        /// This method is used to reopen the device after the device reconnects to the computer and when the app resumes.
        /// </summary>
        /// <returns>
        /// True if the device was successfully opened, false if the device could not be opened for well known reasons.
        /// An exception may be thrown if the device could not be opened for extraordinary reasons.
        /// </returns>
        public async Task<bool> OpenDeviceAsync(DeviceInformation deviceInfo, string deviceSelector)
        {
            bool successfullyOpenedDevice = false;
            NotifyType notificationStatus;
            string notificationMessage = null;

            Device = await SerialDevice.FromIdAsync(deviceInfo.Id);
            // Device could have been blocked by user or the device has already been opened by another app.
            if (Device != null)
            {
                successfullyOpenedDevice = true;
                DeviceInformation = deviceInfo;
                DeviceSelector = deviceSelector;

                // necessary for the MALT read/write to work
                try
                {
                    Device.BaudRate = 9600;
                    Device.DataBits = 8;
                    Device.Handshake = SerialHandshake.None;
                }
                catch { }

                // should be set by default
                try
                {
                    Device.StopBits = SerialStopBitCount.One;
                    Device.Parity = SerialParity.None;
                }
                catch { }

                notificationStatus = NotifyType.StatusMessage;
                notificationMessage = "Device " + DeviceInformation.Id + " opened";

                // Notify registered callback handle that the device has been opened
                OnDeviceConnected?.Invoke(this, DeviceInformation);

                if (appSuspendEventHandler == null || appResumeEventHandler == null)
                {
                    RegisterForAppEvents();
                }

                // Register for DeviceAccessInformation.AccessChanged event and react to any changes to the
                // user access after the device handle was opened.
                if (deviceAccessEventHandler == null)
                {
                    RegisterForDeviceAccessStatusChange();
                }

                // Create and register device watcher events for the device to be opened unless we're reopening the device
                if (deviceWatcher == null)
                {
                    deviceWatcher = DeviceInformation.CreateWatcher(deviceSelector);
                    RegisterForDeviceWatcherEvents();
                }

                if (!watcherStarted)
                {
                    // Start the device watcher after we made sure that the device is opened.
                    StartDeviceWatcher();
                }
            }
            else
            {
                successfullyOpenedDevice = false;
                notificationStatus = NotifyType.ErrorMessage;

                var deviceAccessStatus = DeviceAccessInformation.CreateFromId(deviceInfo.Id).CurrentStatus;
                if (deviceAccessStatus == DeviceAccessStatus.DeniedByUser)
                {
                    notificationMessage = "Access to the device was blocked by the user : " + deviceInfo.Id;
                }
                else if (deviceAccessStatus == DeviceAccessStatus.DeniedBySystem)
                {
                    // This status is most likely caused by app permissions (did not declare the device in the app's package.appxmanifest)
                    // This status does not cover the case where the device is already opened by another app.
                    notificationMessage = "Access to the device was blocked by the system : " + deviceInfo.Id;
                }
                else
                {
                    // Most likely the device is opened by another app, but cannot be sure
                    notificationMessage = "Unknown error, possibly opened by another app : " + deviceInfo.Id;
                }
            }

            MainPage.Current.NotifyUser(notificationMessage, notificationStatus);
            return successfullyOpenedDevice;
        }

        /// <summary>
        /// Closes the device, stops the device watcher, stops listening for app events, and resets object state to before a device
        /// was ever connected.
        /// </summary>
        public void CloseDevice()
        {
            if (IsDeviceConnected)
            {
                CloseCurrentlyConnectedDevice();
            }

            if (deviceWatcher != null)
            {
                if (watcherStarted)
                {
                    StopDeviceWatcher();
                    UnregisterFromDeviceWatcherEvents();
                }

                deviceWatcher = null;
            }

            if (DeviceAccessInformation != null)
            {
                UnregisterFromDeviceAccessStatusChange();
                DeviceAccessInformation = null;
            }

            if (appSuspendEventHandler != null || appResumeEventHandler != null)
            {
                UnregisterFromAppEvents();
            }

            DeviceInformation = null;
            DeviceSelector = null;
            OnDeviceConnected = null;
            OnDeviceClose = null;
            IsEnabledAutoReconnect = true;
        }

        private EventHandlerForDevice()
        {
            watcherStarted = false;
            watcherSuspended = false;
            IsEnabledAutoReconnect = true;
        }

        /// <summary>
        /// This method demonstrates how to close the device properly using the WinRT Serial API.
        ///
        /// When the SerialDevice is closing, it will cancel all IO operations that are still pending (not complete).
        /// The close will not wait for any IO completion callbacks to be called, so the close call may complete before any of
        /// the IO completion callbacks are called.
        /// The pending IO operations will still call their respective completion callbacks with either a task 
        /// cancelled error or the operation completed.
        /// </summary>
        private async void CloseCurrentlyConnectedDevice()
        {
            if (Device != null)
            {
                // Notify callback that we're about to close the device
                OnDeviceClose?.Invoke(this, DeviceInformation);

                // This closes the handle to the device
                Device.Dispose();

                // Save the deviceInformation.Id in case deviceInformation is set to null when closing the device
                Device = null;

                string deviceId = DeviceInformation.Id;

                await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                {
                    MainPage.Current.NotifyUser(deviceId + " is closed", NotifyType.StatusMessage);
                }));
            }
        }

        /// <summary>
        /// Register for app suspension/resume events. See the comments
        /// for the event handlers for more information on the exact device operation performed.
        ///
        /// We will also register for when the app exists so that we may close the device handle.
        /// </summary>
        private void RegisterForAppEvents()
        {
            appSuspendEventHandler = new SuspendingEventHandler(Current.OnAppSuspension);
            appResumeEventHandler = new EventHandler<Object>(Current.OnAppResume);

            // This event is raised when the app is exited and when the app is suspended
            Application.Current.Suspending += appSuspendEventHandler;
            Application.Current.Resuming += appResumeEventHandler;
        }

        private void UnregisterFromAppEvents()
        {
            // This event is raised when the app is exited and when the app is suspended
            Application.Current.Suspending -= appSuspendEventHandler;
            appSuspendEventHandler = null;
            Application.Current.Resuming -= appResumeEventHandler;
            appResumeEventHandler = null;
        }

        /// <summary>
        /// Register for Added and Removed events.
        /// Note that, when disconnecting the device, the device may be closed by the system before the OnDeviceRemoved callback is invoked.
        /// </summary>
        private void RegisterForDeviceWatcherEvents()
        {
            deviceAddedEventHandler = new TypedEventHandler<DeviceWatcher, DeviceInformation>(OnDeviceAdded);
            deviceRemovedEventHandler = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(OnDeviceRemoved);
            deviceWatcher.Added += deviceAddedEventHandler;
            deviceWatcher.Removed += deviceRemovedEventHandler;
        }

        private void UnregisterFromDeviceWatcherEvents()
        {
            deviceWatcher.Added -= deviceAddedEventHandler;
            deviceAddedEventHandler = null;
            deviceWatcher.Removed -= deviceRemovedEventHandler;
            deviceRemovedEventHandler = null;
        }

        /// <summary>
        /// Listen for any changed in device access permission. The user can block access to the device while the device is in use.
        /// If the user blocks access to the device while the device is opened, the device's handle will be closed automatically by
        /// the system; it is still a good idea to close the device explicitly so that resources are cleaned up.
        /// 
        /// Note that by the time the AccessChanged event is raised, the device handle may already be closed by the system.
        /// </summary>
        private void RegisterForDeviceAccessStatusChange()
        {
            // Enable the following registration ONLY if the Serial device under test is non-internal.
            //deviceAccessInformation = DeviceAccessInformation.CreateFromId(deviceInformation.Id);
            //deviceAccessEventHandler = new TypedEventHandler<DeviceAccessInformation, DeviceAccessChangedEventArgs>(this.OnDeviceAccessChanged);
            //deviceAccessInformation.AccessChanged += deviceAccessEventHandler;
        }

        private void UnregisterFromDeviceAccessStatusChange()
        {
            DeviceAccessInformation.AccessChanged -= deviceAccessEventHandler;
            deviceAccessEventHandler = null;
        }

        private void StartDeviceWatcher()
        {
            watcherStarted = true;
            if ((deviceWatcher.Status != DeviceWatcherStatus.Started) && (deviceWatcher.Status != DeviceWatcherStatus.EnumerationCompleted))
            {
                deviceWatcher.Start();
            }
        }

        private void StopDeviceWatcher()
        {
            if ((deviceWatcher.Status == DeviceWatcherStatus.Started) || (deviceWatcher.Status == DeviceWatcherStatus.EnumerationCompleted))
            {
                deviceWatcher.Stop();
            }

            watcherStarted = false;
        }

        /// <summary>
        /// If a Serial object has been instantiated (a handle to the device is opened), we must close it before the app 
        /// goes into suspension because the API automatically closes it for us if we don't. When resuming, the API will
        /// not reopen the device automatically, so we need to explicitly open the device in the app.
        ///
        /// Since we have to reopen the device ourselves when the app resumes, it is good practice to explicitly call the close
        /// in the app as well (For every open there is a close).
        /// 
        /// We must stop the DeviceWatcher because it will continue to raise events even if
        /// the app is in suspension, which is not desired (drains battery). We resume the device watcher once the app resumes again.
        /// </summary>
        private void OnAppSuspension(object sender, SuspendingEventArgs args)
        {
            if (watcherStarted)
            {
                watcherSuspended = true;
                StopDeviceWatcher();
            }
            else
            {
                watcherSuspended = false;
            }

            CloseCurrentlyConnectedDevice();
        }

        /// <summary>
        /// When resume into the application, we should reopen a handle to the Serial device again. This will automatically
        /// happen when we start the device watcher again; the device will be re-enumerated and we will attempt to reopen it
        /// if IsEnabledAutoReconnect property is enabled.
        /// 
        /// See OnAppSuspension for why we are starting the device watcher again
        /// </summary>
        private void OnAppResume(object sender, object args)
        {
            if (watcherSuspended)
            {
                watcherSuspended = false;
                StartDeviceWatcher();
            }
        }

        /// <summary>
        /// Close the device that is opened so that all pending operations are canceled properly.
        /// </summary>
        private void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {
            if (IsDeviceConnected && (deviceInformationUpdate.Id == DeviceInformation.Id))
            {
                // The main reasons to close the device explicitly is to clean up resources, to properly handle errors,
                // and stop talking to the disconnected device.
                CloseCurrentlyConnectedDevice();
            }
        }

        /// <summary>
        /// Open the device that the user wanted to open if it hasn't been opened yet and auto reconnect is enabled.
        /// </summary>
        private async void OnDeviceAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            if ((DeviceInformation != null) && (deviceInfo.Id == DeviceInformation.Id) && !IsDeviceConnected && IsEnabledAutoReconnect)
            {
                await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(async () =>
                {
                    await OpenDeviceAsync(DeviceInformation, DeviceSelector);
                    // Any app specific device intialization should be done here because we don't know the state of the device when it is re-enumerated.
                }));
            }
        }
    }
}
