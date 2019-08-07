// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace SensorExplorer
{
    class SensorData
    {
        public class Reading
        {
            public DateTime Timestamp;
            public double[] Value;

            public Reading(DateTime timestamp, double[] value)
            {
                Timestamp = timestamp;
                Value = value;
            }
        }

        public bool ReportIntervalChanged = false;
        public double[] MaxValue;
        public double[] MinValue;
        public int Count = -1;
        public int SensorType = -1;
        public List<Reading> Readings = new List<Reading>();
        public string Category;
        public string ConnectionType;
        public string DeviceId;
        public string DeviceName;
        public string IsPrimary;
        public string Manufacturer;
        public string Model;
        public string Name = string.Empty;
        public string PanelColor;
        public string PanelGroup;
        public string PanelHeight;
        public string PanelId;
        public string PanelLength;
        public string PanelPositionX;
        public string PanelPositionY;
        public string PanelPositionZ;
        public string PanelRotationX;
        public string PanelRotationY;
        public string PanelRotationZ;
        public string PanelShape;
        public string PanelSide;
        public string PanelVisible;
        public string PanelWidth;
        public string PersistentUniqueId;
        public string[] Property;
        public string State;
        public string VendorDefinedSubType;
        public uint DefaultReportInterval = 0;
        public uint ReportInterval = 0;
        public uint MinReportInterval = 0;
        public uint ReportLatency = 0;

        public SensorData(int sensorType, int count, string name, string[] property)
        {
            SensorType = sensorType;
            Count = count;
            Name = name;
            Property = property;
            MaxValue = new double[Property.Length];
            MinValue = new double[Property.Length];
        }

        public void AddProperty(string deviceId, string deviceName, uint reportInterval, uint minReportInterval, uint reportLatency, string[] properties)
        {
            if (DefaultReportInterval == 0)
            {
                DefaultReportInterval = reportInterval;
            }

            DeviceId = deviceId;
            DeviceName = deviceName;
            ReportInterval = reportInterval;
            MinReportInterval = minReportInterval;
            ReportLatency = reportLatency;
            Category = properties[0];
            PersistentUniqueId = properties[1];
            Manufacturer = properties[2];
            Model = properties[3];
            ConnectionType = properties[4];
            IsPrimary = properties[5];
            VendorDefinedSubType = properties[6];
            State = properties[7];
        }

        public void AddPLDProperty(string[] PLD)
        {
            PanelId = PLD[0];
            PanelGroup = PLD[1];
            PanelSide = PLD[2];
            PanelWidth = PLD[3];
            PanelHeight = PLD[4];
            PanelLength = PLD[5];
            PanelPositionX = PLD[6];
            PanelPositionY = PLD[7];
            PanelPositionZ = PLD[8];
            PanelRotationX = PLD[9];
            PanelRotationY = PLD[10];
            PanelRotationZ = PLD[11];
            PanelColor = PLD[12];
            PanelShape = PLD[13];
            PanelVisible = PLD[14];
        }

        public void UpdateReportInterval(uint reportInterval)
        {
            if (reportInterval != ReportInterval)
            {
                ReportInterval = reportInterval;
                ReportIntervalChanged = true;
            }
        }

        public bool AddReading(DateTime timestamp, double[] value)
        {
            try
            {
                int count = Readings.Count;

                if (count == 0)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        MaxValue[i] = value[i];
                        MinValue[i] = value[i];
                    }
                }
                else
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] > MaxValue[i])
                        {
                            MaxValue[i] = value[i];
                        }
                        else if (value[i] < MinValue[i])
                        {
                            MinValue[i] = value[i];
                        }
                    }
                }

                if (count == 0 || (timestamp - Readings[count - 1].Timestamp).TotalMilliseconds >= ReportInterval)
                {
                    Reading reading = new Reading(timestamp, value);
                    Readings.Add(reading);

                    return true;
                }
            }
            catch { }

            return false;
        }

        public void ClearReading()
        {
            Readings = new List<Reading>();
            for (int i = 0; i < Property.Length; i++)
            {
                MaxValue[i] = 0;
                MinValue[i] = 0;
            }
        }
    }
}