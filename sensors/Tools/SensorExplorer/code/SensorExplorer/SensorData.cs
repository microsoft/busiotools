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
            public DateTime timestamp;
            public double[] value;

            public Reading(DateTime timestamp, double[] value)
            {
                this.timestamp = timestamp;
                this.value = value;
            }
        }

        public List<Reading> _reading = new List<Reading>();
        public int _sensorType = -1;
        public int _count = -1;
        public string _name = string.Empty;
        public string[] _property;
        public double[] _maxValue;
        public double[] _minValue;
        public uint _defaultReportInterval = 0;
        public string _deviceId;
        public string _deviceName;
        public uint _reportInterval = 0;
        public uint _minReportInterval = 0;
        public uint _reportLatency = 0;
        public bool _reportIntervalChanged = false;
        public string _category;
        public string _persistentUniqueId;
        public string _manufacturer;
        public string _model;
        public string _connectionType;
        public string _isPrimary;
        public string _vendorDefinedSubType;
        public string _state;

        public string _panelId;
        public string _panelGroup;
        public string _panelSide;
        public string _panelWidth;
        public string _panelHeight;
        public string _panelLength;
        public string _panelPositionX;
        public string _panelPositionY;
        public string _panelPositionZ;
        public string _panelRotationX;
        public string _panelRotationY;
        public string _panelRotationZ;
        public string _panelColor;
        public string _panelShape;
        public string _panelVisible;

        public SensorData(int sensorType, int count, string name, string[] property)
        {
            _sensorType = sensorType;
            _count = count;
            _name = name;
            _property = property;
            _maxValue = new double[_property.Length];
            _minValue = new double[_property.Length];
        }

        public void AddProperty(string deviceId, string deviceName, uint reportInterval, uint minReportInterval, uint reportLatency, string[] properties)
        {
            if (_defaultReportInterval == 0)
            {
                _defaultReportInterval = reportInterval;
            }

            _deviceId = deviceId;
            _deviceName = deviceName;
            _reportInterval = reportInterval;
            _minReportInterval = minReportInterval;
            _reportLatency = reportLatency;
            _category = properties[0];
            _persistentUniqueId = properties[1];
            _manufacturer = properties[2];
            _model = properties[3];
            _connectionType = properties[4];
            _isPrimary = properties[5];
            _vendorDefinedSubType = properties[6];
            _state = properties[7];
        }

        public void AddPLDProperty(string[] PLD)
        {
            _panelId = PLD[0];
            _panelGroup = PLD[1];
            _panelSide = PLD[2];
            _panelWidth = PLD[3];
            _panelHeight = PLD[4];
            _panelLength = PLD[5];
            _panelPositionX = PLD[6];
            _panelPositionY = PLD[7];
            _panelPositionZ = PLD[8];
            _panelRotationX = PLD[9];
            _panelRotationY = PLD[10];
            _panelRotationZ = PLD[11];
            _panelColor = PLD[12];
            _panelShape = PLD[13];
            _panelVisible = PLD[14];
        }

        public void UpdateReportInterval(uint reportInterval)
        {
            if (reportInterval != _reportInterval)
            {
                _reportInterval = reportInterval;
                _reportIntervalChanged = true;
            }
        }

        public bool AddReading(DateTime timestamp, double[] value)
        {
            try
            {
                int count = _reading.Count;

                if (count == 0)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        _maxValue[i] = value[i];
                        _minValue[i] = value[i];
                    }
                }
                else
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i] > _maxValue[i])
                        {
                            _maxValue[i] = value[i];
                        }
                        else if (value[i] < _minValue[i])
                        {
                            _minValue[i] = value[i];
                        }
                    }
                }

                if (count == 0 || (timestamp - _reading[count - 1].timestamp).TotalMilliseconds >= _reportInterval)
                {
                    Reading reading = new Reading(timestamp, value);
                    _reading.Add(reading);

                    return true;
                }
            }
            catch { }

            return false;
        }

        public void ClearReading()
        {
            _reading = new List<Reading>();
            for (int i = 0; i < _property.Length; i++)
            {
                _maxValue[i] = 0;
                _minValue[i] = 0;
            }
        }
    }
}