using System;
using System.Collections.Generic;

namespace SensorExplorer
{
    class SensorData
    {
        public class Reading
        {
            public DateTime timestamp;
            public double[] value;

            public Reading(DateTime timestamp, double[] value)
            {
                this.timestamp = timestamp;
                this.value = value;
            }
        }

        public List<Reading> ReadingList = new List<Reading>();
        public int SensorType = -1;
        public int Count = -1;
        public string Name = string.Empty;
        public string[] Property;
        public double[] MaxValue;
        public double[] MinValue;
        public uint DefaultReportInterval = 0;
        public string DeviceId;
        public string DeviceName;
        public uint ReportInterval = 0;
        public uint MinReportInterval = 0;
        public uint ReportLatency = 0;
        public bool ReportIntervalChanged = false;
        public string Category;
        public string PersistentUniqueId;
        public string Manufacturer;
        public string Model;
        public string ConnectionType;

        public SensorData(int sensorType, int count, string name, string[] property)
        {
            SensorType = sensorType;
            Count = count;
            Name = name;
            Property = property;
            MaxValue = new double[Property.Length];
            MinValue = new double[Property.Length];
        }

        public void AddProperty(string deviceId, string deviceName, uint reportInterval, uint minReportInterval, uint reportLatency,
                                string category, string persistentUniqueId, string manufacturer, string model, string connectionType)
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
            Category = category;
            PersistentUniqueId = persistentUniqueId;
            Manufacturer = manufacturer;
            Model = model;
            ConnectionType = connectionType;
    }

        public void UpdateReportInterval(uint reportInterval)
        {
            if (ReportInterval != reportInterval)
            {
                ReportInterval = reportInterval;
                ReportIntervalChanged = true;
            }
        }

        public bool AddReading(DateTime timestamp, double[] value)
        {
            try
            {
                int count = ReadingList.Count;

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

                if (count == 0 || (timestamp - ReadingList[count - 1].timestamp).TotalMilliseconds >= ReportInterval)
                {
                    Reading reading = new Reading(timestamp, value);
                    ReadingList.Add(reading);

                    return true;
                }
            }
            catch { }

            return false;
        }

        public void ClearReading()
        {
            ReadingList = new List<Reading>();
            for (int i = 0; i < Property.Length; i++)
            {
                MaxValue[i] = 0;
                MinValue[i] = 0;
            }
        }
    }
}