// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.Devices.Enumeration;

namespace SensorExplorer
{
    /// <summary>
    /// The class will only expose properties from DeviceInformation that are going to be used here.
    /// Each instance of this class provides information about a single device.
    ///
    /// This class is used by the UI to display device specific information so that
    /// the user can identify which device to use.
    /// </summary>
    public class DeviceListEntry
    {
        public string InstanceId
        {
            get
            {
                return DeviceInformation.Properties[DeviceProperties.DeviceInstanceId] as string;
            }
        }

        public DeviceInformation DeviceInformation { get; }

        public string DeviceSelector { get; }

        /// <summary>
        /// The class is mainly used as a DeviceInformation wrapper so that the UI can bind to a list of these.
        /// </summary>
        public DeviceListEntry(DeviceInformation deviceInformation, string deviceSelector)
        {
            DeviceInformation = deviceInformation;
            DeviceSelector = deviceSelector;
        }
    }
}
