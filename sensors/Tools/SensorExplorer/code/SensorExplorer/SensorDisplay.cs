// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace SensorExplorer
{
    class SensorDisplay
    {
        public static Dictionary<ActivityType, string> DictionaryActivity = new Dictionary<ActivityType, string>
        {
            { ActivityType.Unknown,    "❓" },
            { ActivityType.Idle,       "⚲" },
            { ActivityType.Stationary, "⤓" },
            { ActivityType.Fidgeting,  "📳" },
            { ActivityType.Walking,    "🚶" },
            { ActivityType.Running,    "🏃" },
            { ActivityType.InVehicle,  "🚗" },
            { ActivityType.Biking,     "🚲" },
        };

        public static Dictionary<PedometerStepKind, string> DictionaryStepKind = new Dictionary<PedometerStepKind, string>
        {
            { PedometerStepKind.Unknown, "❓" },
            { PedometerStepKind.Walking, "🚶" },
            { PedometerStepKind.Running, "🏃" },
        };

        public int _sensorType;
        public int _index;
        public bool _isOn = false;

        public StackPanel StackPanelSensor = new StackPanel() { Orientation = Orientation.Vertical, Visibility = Visibility.Collapsed, Margin = new Thickness() { Right = 18, Bottom = 20, Top = 0 } };
        public PlotCanvas _plotCanvas;
        public TextBox TextboxReportInterval = new TextBox() { Height = 32, Width = 100, Margin = new Thickness() { Left = 40, Right = 10, Top = 20, Bottom = 10 } };

        //MALT
        public Button MALTButton = new Button() { Content = "Include MALT", Height = 50, Width = 200, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness() { Top = 50 }, FontSize = 20 };
        public StackPanel StackPanelMALTData = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness() { Left = 50 }, Background = new SolidColorBrush(Colors.AliceBlue), Visibility = Visibility.Collapsed };
        public TextBlock[] TextBlockMALTPropertyValue1;
        public TextBlock[] TextBlockMALTPropertyValue2;

        private int _totalIndex;
        private string _name;
        private string[] _properties = new string[] { "\r\nReport Interval", "Min Report Interval", "Category", "PersistentUniqueID", "Manufacturer", "Model", "ConnectionType", "Device ID" };

        private StackPanel stackPanelSwitch = new StackPanel();
        private Button buttonReportInterval = new Button() { Height = 32, Content = "Change", Margin = new Thickness() { Left = 10, Right = 10, Top = 20, Bottom = 10 } };
        private Button buttonSensor = new Button();
        private StackPanel stackPanelTop = new StackPanel() { Orientation = Orientation.Horizontal, Height = 100, HorizontalAlignment = HorizontalAlignment.Center };
        private Ellipse ellipseAccelerometer = new Ellipse() { Width = 20, Height = 20, Fill = new SolidColorBrush(Colors.DarkRed), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 };
        private Image imageCompass = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Compass.png")) };
        private Image imageGyrometerX = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image imageGyrometerY = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image imageGyrometerZ = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image imageInclinometerPitch = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private Image imageInclinometerRoll = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private Image imageInclinometerYaw = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private TextBlock textBlockSensor = new TextBlock() { Foreground = new SolidColorBrush(Colors.Black), FontSize = 72 };

        private StackPanel stackPanelBottom = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stackPanelBottomData = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stackPanelBottomRightCol = new StackPanel() { Orientation = Orientation.Vertical };

        private StackPanel stackPanelDataName = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(40, 0, 0, 0) };
        private TextBlock[] textBlockProperty;
        private StackPanel stackPanelValue = new StackPanel() { Orientation = Orientation.Vertical };
        private TextBlock[] textBlockValue;
        private StackPanel stackPanelMinValue = new StackPanel() { Orientation = Orientation.Vertical };
        private TextBlock[] textBlockMinValue;
        private StackPanel stackPanelMaxValue = new StackPanel() { Orientation = Orientation.Vertical };
        private TextBlock[] textBlockMaxValue;

        private StackPanel stackPanelPropertyName = new StackPanel() { Orientation = Orientation.Vertical };
        private TextBlock[] textBlockPropertyName;
        private StackPanel stackPanelPropertyValue = new StackPanel() { Orientation = Orientation.Vertical };
        private TextBlock[] textBlockPropertyValue;
        public StackPanel stackPanelProperty = new StackPanel() { Orientation = Orientation.Horizontal };
        private Canvas canvasSensor = new Canvas();

        // MALT
        private StackPanel stackPanelMALT2 = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness() { Left = 20, Top = 20 } };
        private TextBlock textblockMALTData1 = new TextBlock() { Text = "MALT Light/Color Sensor (Ambient)", FontSize = 20, Margin = new Thickness() { Left = 20, Top = 10 } };
        private StackPanel stackPanelMALTPropertyName1 = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.AliceBlue), Margin = new Thickness() { Left = 20, Top = 10 } };
        private TextBlock[] textBlockMALTPropertyName1;
        private StackPanel stackPanelMALTPropertyValue1 = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.AliceBlue), Margin = new Thickness() { Left = 20, Top = 10 } };
        private TextBlock textblockMALTData2 = new TextBlock() { Text = "MALT Light/Color Sensor (Screen)", FontSize = 20, Margin = new Thickness() { Left = 20, Top = 10 } };
        private StackPanel stackPanelMALTPropertyName2 = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.AliceBlue), Margin = new Thickness() { Left = 20, Top = 10 } };
        private TextBlock[] textBlockMALTPropertyName2;
        private StackPanel stackPanelMALTPropertyValue2 = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.AliceBlue), Margin = new Thickness() { Left = 20, Top = 10, Bottom = 20 } };
        private Button hideMALTButton = new Button() { Content = "Hide", Margin = new Thickness(20) };

        public SensorDisplay(int sensorType, int index, int totalIndex, string name, int minValue, int maxValue, int scale, Color[] color)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();

            StackPanelSensor.Children.Clear();

            _sensorType = sensorType;
            _index = index;
            _totalIndex = totalIndex;
            _name = name;

            string[] vAxisLabel = new string[scale + 1];

            for (int i = 0; i <= scale; i++)
            {
                if (sensorType == Sensor.ACTIVITYSENSOR)
                {
                    if (i < 2)
                    {
                        vAxisLabel[i] = ((ActivitySensorReadingConfidence)i).ToString();
                    }
                    else
                    {
                        vAxisLabel[i] = "None";
                    }
                }
                else if (sensorType == Sensor.SIMPLEORIENTATIONSENSOR)
                {
                    vAxisLabel[i] = ((SimpleOrientation)(maxValue - i)).ToString().Replace("DegreesCounterclockwise", "°↺");
                }
                else
                {
                    vAxisLabel[i] = (maxValue - Convert.ToDouble(i) / scale * (maxValue - minValue)).ToString();
                }
            }

            _plotCanvas = new PlotCanvas(minValue, maxValue, color, canvasSensor, vAxisLabel);
            stackPanelSwitch.Children.Add(buttonSensor);

            if (sensorType == Sensor.ACCELEROMETER || sensorType == Sensor.ACCELEROMETERLINEAR || sensorType == Sensor.ACCELEROMETERGRAVITY)
            {
                Grid gridAccelerometer = new Grid();
                gridAccelerometer.Children.Add(new Ellipse() { Width = 100, Height = 100, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 });
                gridAccelerometer.Children.Add(new Ellipse() { Width = 50, Height = 50, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 });
                gridAccelerometer.Children.Add(ellipseAccelerometer);
                stackPanelTop.Children.Add(gridAccelerometer);
            }
            else if (sensorType == Sensor.COMPASS)
            {
                stackPanelTop.Children.Add(imageCompass);
            }
            else if (sensorType == Sensor.GYROMETER)
            {
                StackPanel stackPanelX = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                stackPanelX.Children.Add(imageGyrometerX);
                stackPanelX.Children.Add(new TextBlock() { Text = "X", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel stackPanelY = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                stackPanelY.Children.Add(imageGyrometerY);
                stackPanelY.Children.Add(new TextBlock() { Text = "Y", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel stackPanelZ = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                stackPanelZ.Children.Add(imageGyrometerZ);
                stackPanelZ.Children.Add(new TextBlock() { Text = "Z", HorizontalAlignment = HorizontalAlignment.Center });

                stackPanelTop.Children.Add(stackPanelX);
                stackPanelTop.Children.Add(stackPanelY);
                stackPanelTop.Children.Add(stackPanelZ);
            }
            else if (sensorType == Sensor.INCLINOMETER)
            {
                StackPanel stackPanelX = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                stackPanelX.Children.Add(imageInclinometerPitch);
                stackPanelX.Children.Add(new TextBlock() { Text = "Pitch", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel stackPanelY = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                stackPanelY.Children.Add(imageInclinometerRoll);
                stackPanelY.Children.Add(new TextBlock() { Text = "Roll", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel stackPanelZ = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                stackPanelZ.Children.Add(imageInclinometerYaw);
                stackPanelZ.Children.Add(new TextBlock() { Text = "Yaw", HorizontalAlignment = HorizontalAlignment.Center });

                stackPanelTop.Children.Add(stackPanelX);
                stackPanelTop.Children.Add(stackPanelY);
                stackPanelTop.Children.Add(stackPanelZ);
            }
            else if (sensorType == Sensor.ACTIVITYSENSOR || sensorType == Sensor.LIGHTSENSOR || sensorType == Sensor.PEDOMETER || sensorType == Sensor.PROXIMITYSENSOR)
            {
                stackPanelTop.Children.Add(textBlockSensor);
            }
            else
            {
                stackPanelTop.Height = 0;
            }

            textBlockProperty = new TextBlock[color.Length + 1];
            textBlockValue = new TextBlock[textBlockProperty.Length];
            textBlockMinValue = new TextBlock[textBlockProperty.Length];
            textBlockMaxValue = new TextBlock[textBlockProperty.Length];

            textBlockProperty[color.Length] = SetTextStyle("", HorizontalAlignment.Center);
            stackPanelDataName.Children.Add(textBlockProperty[color.Length]);

            textBlockValue[color.Length] = SetTextStyle(resourceLoader.GetString("Value"), HorizontalAlignment.Center);
            stackPanelValue.Children.Add(textBlockValue[color.Length]);

            textBlockMinValue[color.Length] = SetTextStyle(resourceLoader.GetString("Min"), HorizontalAlignment.Center);
            stackPanelMinValue.Children.Add(textBlockMinValue[color.Length]);

            textBlockMaxValue[color.Length] = SetTextStyle(resourceLoader.GetString("Max"), HorizontalAlignment.Center);
            stackPanelMaxValue.Children.Add(textBlockMaxValue[color.Length]);

            for (int i = 0; i < color.Length; i++)
            {
                textBlockProperty[i] = SetTextStyle("", HorizontalAlignment.Left);
                textBlockProperty[i].Foreground = new SolidColorBrush(color[i]);
                stackPanelDataName.Children.Add(textBlockProperty[i]);

                textBlockValue[i] = SetTextStyle("", HorizontalAlignment.Right);
                textBlockValue[i].Foreground = new SolidColorBrush(color[i]);
                stackPanelValue.Children.Add(textBlockValue[i]);

                textBlockMinValue[i] = SetTextStyle("", HorizontalAlignment.Right);
                textBlockMinValue[i].Foreground = new SolidColorBrush(color[i]);
                stackPanelMinValue.Children.Add(textBlockMinValue[i]);

                textBlockMaxValue[i] = SetTextStyle("", HorizontalAlignment.Right);
                textBlockMaxValue[i].Foreground = new SolidColorBrush(color[i]);
                stackPanelMaxValue.Children.Add(textBlockMaxValue[i]);
            }

            if (sensorType == Sensor.LIGHTSENSOR)
            {
                stackPanelBottom.Children.Add(stackPanelProperty);

                stackPanelBottomData.Children.Add(stackPanelDataName);
                stackPanelBottomData.Children.Add(stackPanelValue);
                stackPanelBottomData.Children.Add(stackPanelMinValue);
                stackPanelBottomData.Children.Add(stackPanelMaxValue);

                stackPanelBottomRightCol.Children.Add(stackPanelBottomData);
                stackPanelBottomRightCol.Children.Add(MALTButton);
                MALTButton.Click += Scenario1View.Scenario1.MALTButton;

                textBlockMALTPropertyName1 = new TextBlock[8];
                TextBlockMALTPropertyValue1 = new TextBlock[8];
                textBlockMALTPropertyName2 = new TextBlock[8];
                TextBlockMALTPropertyValue2 = new TextBlock[8];

                textBlockMALTPropertyName1[0] = new TextBlock() { Text = "Lux", Width = 120 };
                textBlockMALTPropertyName1[1] = new TextBlock() { Text = "Clear", Width = 120, Foreground = new SolidColorBrush(Colors.DarkGray) };
                textBlockMALTPropertyName1[2] = new TextBlock() { Text = "R", Width = 120, Foreground = new SolidColorBrush(Colors.Red) };
                textBlockMALTPropertyName1[3] = new TextBlock() { Text = "G", Width = 120, Foreground = new SolidColorBrush(Colors.Green) };
                textBlockMALTPropertyName1[4] = new TextBlock() { Text = "B", Width = 120, Foreground = new SolidColorBrush(Colors.Blue) };
                //textBlockMALTPropertyName1[5] = new TextBlock() { Text = "Chromaticity x", Width = 120 };
                //textBlockMALTPropertyName1[6] = new TextBlock() { Text = "Chromaticity y", Width = 120 };
                //textBlockMALTPropertyName1[7] = new TextBlock() { Text = "Chromaticity Y", Width = 120 };

                TextBlockMALTPropertyValue1[0] = new TextBlock() { Width = 120 };
                TextBlockMALTPropertyValue1[1] = new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.DarkGray) };
                TextBlockMALTPropertyValue1[2] = new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Red) };
                TextBlockMALTPropertyValue1[3] = new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Green) };
                TextBlockMALTPropertyValue1[4] = new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Blue) };
                //TextBlockMALTPropertyValue1[5] = new TextBlock() { Width = 120 };
                //TextBlockMALTPropertyValue1[6] = new TextBlock() { Width = 120 };
                //TextBlockMALTPropertyValue1[7] = new TextBlock() { Width = 120 };

                textBlockMALTPropertyName2[0] = new TextBlock() { Text = "Lux", Width = 120 };
                textBlockMALTPropertyName2[1] = new TextBlock() { Text = "Clear", Width = 120, Foreground = new SolidColorBrush(Colors.DarkGray) };
                textBlockMALTPropertyName2[2] = new TextBlock() { Text = "R", Width = 120, Foreground = new SolidColorBrush(Colors.Red) };
                textBlockMALTPropertyName2[3] = new TextBlock() { Text = "G", Width = 120, Foreground = new SolidColorBrush(Colors.Green) };
                textBlockMALTPropertyName2[4] = new TextBlock() { Text = "B", Width = 120, Foreground = new SolidColorBrush(Colors.Blue) };
                //textBlockMALTPropertyName2[5] = new TextBlock() { Text = "Chromaticity x", Width = 120 };
                //textBlockMALTPropertyName2[6] = new TextBlock() { Text = "Chromaticity y", Width = 120 };
                //textBlockMALTPropertyName2[7] = new TextBlock() { Text = "Chromaticity Y", Width = 120 };

                TextBlockMALTPropertyValue2[0] = new TextBlock() { Width = 120 };
                TextBlockMALTPropertyValue2[1] = new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.DarkGray) };
                TextBlockMALTPropertyValue2[2] = new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Red) };
                TextBlockMALTPropertyValue2[3] = new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Green) };
                TextBlockMALTPropertyValue2[4] = new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Blue) };
                //TextBlockMALTPropertyValue2[5] = new TextBlock() { Width = 120 };
                //TextBlockMALTPropertyValue2[6] = new TextBlock() { Width = 120 };
                //TextBlockMALTPropertyValue2[7] = new TextBlock() { Width = 120 };

                for (int i = 0; i < textBlockMALTPropertyName1.Length - 3; i++)
                {
                    stackPanelMALTPropertyName1.Children.Add(textBlockMALTPropertyName1[i]);
                    stackPanelMALTPropertyValue1.Children.Add(TextBlockMALTPropertyValue1[i]);
                    stackPanelMALTPropertyName2.Children.Add(textBlockMALTPropertyName2[i]);
                    stackPanelMALTPropertyValue2.Children.Add(TextBlockMALTPropertyValue2[i]);
                }

                StackPanelMALTData.Children.Add(textblockMALTData1);
                StackPanelMALTData.Children.Add(stackPanelMALTPropertyName1);
                StackPanelMALTData.Children.Add(stackPanelMALTPropertyValue1);
                StackPanelMALTData.Children.Add(textblockMALTData2);
                StackPanelMALTData.Children.Add(stackPanelMALTPropertyName2);
                StackPanelMALTData.Children.Add(stackPanelMALTPropertyValue2);
                StackPanelMALTData.Children.Add(hideMALTButton);
                hideMALTButton.Click += Scenario1View.Scenario1.HideMALTButton;

                stackPanelBottom.Children.Add(stackPanelBottomRightCol);
                stackPanelBottom.Children.Add(StackPanelMALTData);
            }
            else
            {
                stackPanelBottom.Children.Add(stackPanelProperty);
                stackPanelBottom.Children.Add(stackPanelDataName);
                stackPanelBottom.Children.Add(stackPanelValue);
                stackPanelBottom.Children.Add(stackPanelMinValue);
                stackPanelBottom.Children.Add(stackPanelMaxValue);
            }

            textBlockPropertyName = new TextBlock[_properties.Length];
            textBlockPropertyValue = new TextBlock[textBlockPropertyName.Length];

            for (int i = 0; i < _properties.Length; i++)
            {
                textBlockPropertyName[i] = SetTextStyle(_properties[i], HorizontalAlignment.Left);
                textBlockPropertyValue[i] = SetTextStyle((i == 0 ? "\r\n" : "") + "        -", HorizontalAlignment.Left);
                stackPanelPropertyName.Children.Add(textBlockPropertyName[i]);

                if (i == 0)
                {
                    textBlockPropertyName[i].Height = 60;
                    textBlockPropertyValue[i].Height = 60;
                    buttonReportInterval.Click += Scenario1View.Scenario1.ReportIntervalButton;
                    StackPanel stackPanelReportInterval = new StackPanel();
                    stackPanelReportInterval.Orientation = Orientation.Horizontal;
                    stackPanelReportInterval.Children.Add(textBlockPropertyValue[i]);
                    stackPanelReportInterval.Children.Add(TextboxReportInterval);
                    stackPanelReportInterval.Children.Add(buttonReportInterval);
                    stackPanelPropertyValue.Children.Add(stackPanelReportInterval);
                }
                else
                {
                    stackPanelPropertyValue.Children.Add(textBlockPropertyValue[i]);
                }
            }

            stackPanelProperty.Children.Add(stackPanelPropertyName);
            stackPanelProperty.Children.Add(stackPanelPropertyValue);

            StackPanelSensor.Children.Add(stackPanelTop);
            StackPanelSensor.Children.Add(canvasSensor);
            StackPanelSensor.Children.Add(stackPanelBottom);
        }

        private TextBlock SetTextStyle(string text, HorizontalAlignment horizontalAlignment)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.HorizontalAlignment = horizontalAlignment;
            textBlock.Text = text;
            textBlock.VerticalAlignment = VerticalAlignment.Center;

            return textBlock;
        }

        public double SetWidth(double width, double height)
        {
            double actualWidth = 0;
            double stackPanelMinWidth = StackPanelSensor.Margin.Left + stackPanelDataName.Width + stackPanelValue.Width + StackPanelSensor.Margin.Right;
            double stackPanelTextWidth = stackPanelMinWidth + stackPanelMinValue.Width + stackPanelMaxValue.Width; // 348

            double fontSize = 11;

            if (width > 1366)
            {
                fontSize = 20;
            }
            else if (width > 768)
            {
                fontSize = 17;
            }
            else if (width > 480)
            {
                fontSize = 14;
            }

            SetFontSize(fontSize);
            SetHeight(height * 0.2);
            canvasSensor.Width = width * 0.7;

            return actualWidth;
        }

        private void SetFontSize(double fontSize)
        {
            for (int i = 0; i < textBlockProperty.Length; i++)
            {
                textBlockProperty[i].FontSize = fontSize;
                textBlockValue[i].FontSize = fontSize;
                textBlockMinValue[i].FontSize = fontSize;
                textBlockMaxValue[i].FontSize = fontSize;
            }

            for (int i = 0; i < textBlockPropertyName.Length; i++)
            {
                textBlockPropertyName[i].FontSize = fontSize;
                textBlockPropertyValue[i].FontSize = fontSize;
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "00000000";
            textBlock.FontSize = fontSize;
            textBlock.Measure(new Size(200, 200)); // Assuming 200x200 is max size of textblock
            canvasSensor.Margin = new Thickness() { Left = textBlock.DesiredSize.Width, Top = textBlock.DesiredSize.Height, Bottom = textBlock.DesiredSize.Height * 2 };
            _plotCanvas.SetFontSize(fontSize);
        }

        public void SetHeight(double height)
        {
            _plotCanvas.SetHeight(height);
        }

        public void EnableSensor()
        {
            buttonSensor.IsEnabled = true;
            buttonSensor.Opacity = 1;
            buttonSensor.SetValue(AutomationProperties.NameProperty, "x");

            _isOn = !_isOn;
            buttonSensor.SetValue(AutomationProperties.NameProperty, "");
            StackPanelSensor.Visibility = Visibility.Visible;
            StackPanelSensor.Opacity = 1;
            PeriodicTimer.Create(_totalIndex);
            Sensor.EnableSensor(_sensorType, _index, _totalIndex);
        }

        public void UpdateProperty(string deviceId, string deviceName, uint reportInterval, uint minReportInterval, uint reportLatency,
                                   string category, string persistentUniqueId, string manufacturer, string model, string connectionType)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            textBlockPropertyValue[0].Text = string.Format("\r\n  {0}", reportInterval != 0 ? reportInterval.ToString() : "-");
            textBlockPropertyValue[1].Text = string.Format("  {0}", minReportInterval != 0 ? minReportInterval.ToString() : "-");
            textBlockPropertyValue[2].Text = "  " + category;
            textBlockPropertyValue[3].Text = "  " + persistentUniqueId;
            textBlockPropertyValue[4].Text = "  " + manufacturer;
            textBlockPropertyValue[5].Text = "  " + model;
            textBlockPropertyValue[6].Text = "  " + connectionType;
            textBlockPropertyValue[7].Text = $"{deviceId.Replace("{", "\r\n  {")}";
        }

        public void UpdateText(SensorData sensorData)
        {
            try
            {
                int index = sensorData._reading.Count - 1;
                if (sensorData._count == Sensor.currentId)
                {
                    UpdateProperty(sensorData._deviceId, sensorData._deviceName, sensorData._reportInterval, sensorData._minReportInterval, sensorData._reportLatency,
                                   sensorData._category, sensorData._persistentUniqueId, sensorData._manufacturer, sensorData._model, sensorData._connectionType);
                }

                if (StackPanelSensor.Visibility == Visibility.Visible)
                {
                    if (sensorData._sensorType == Sensor.ACCELEROMETER || sensorData._sensorType == Sensor.ACCELEROMETERLINEAR || sensorData._sensorType == Sensor.ACCELEROMETERGRAVITY)
                    {
                        double margin = 80;
                        double x = Math.Min(1, sensorData._reading[index].value[0]);
                        double y = Math.Min(1, sensorData._reading[index].value[1]);
                        double square = x * x + y * y;

                        if (square > 1)
                        {
                            x /= Math.Sqrt(square);
                            y /= Math.Sqrt(square);
                        }

                        DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
                        if (displayInformation.NativeOrientation == DisplayOrientations.Landscape)
                        {
                            switch (displayInformation.CurrentOrientation)
                            {
                                case DisplayOrientations.Landscape: ellipseAccelerometer.Margin = new Thickness() { Left = margin * x, Bottom = margin * y }; break;
                                case DisplayOrientations.Portrait: ellipseAccelerometer.Margin = new Thickness() { Left = margin * y, Bottom = -margin * x }; break;
                                case DisplayOrientations.LandscapeFlipped: ellipseAccelerometer.Margin = new Thickness() { Left = -margin * x, Bottom = -margin * y }; break;
                                case DisplayOrientations.PortraitFlipped: ellipseAccelerometer.Margin = new Thickness() { Left = -margin * y, Bottom = margin * x }; break;
                            }
                        }
                        else if (displayInformation.NativeOrientation == DisplayOrientations.Portrait)
                        {
                            switch (displayInformation.CurrentOrientation)
                            {
                                case DisplayOrientations.Landscape: ellipseAccelerometer.Margin = new Thickness() { Left = -margin * y, Bottom = margin * x }; break;
                                case DisplayOrientations.Portrait: ellipseAccelerometer.Margin = new Thickness() { Left = margin * x, Bottom = margin * y }; break;
                                case DisplayOrientations.LandscapeFlipped: ellipseAccelerometer.Margin = new Thickness() { Left = margin * y, Bottom = -margin * x }; break;
                                case DisplayOrientations.PortraitFlipped: ellipseAccelerometer.Margin = new Thickness() { Left = -margin * x, Bottom = -margin * y }; break;
                            }
                        }
                    }

                    for (int i = 0; i < sensorData._reading[index].value.Length; i++)
                    {
                        textBlockProperty[i].Text = sensorData._property[i];
                        textBlockValue[i].Text = string.Format("        {0,5:0.00}", sensorData._reading[index].value[i]);
                        textBlockMinValue[i].Text = string.Format("        {0,5:0.0}", sensorData._minValue[i]);
                        textBlockMaxValue[i].Text = string.Format("        {0,5:0.0}", sensorData._maxValue[i]);
                        if (sensorData._property[i].StartsWith("MagneticNorth"))
                        {
                            RotateTransform rotateCompass = new RotateTransform();
                            imageCompass.RenderTransform = rotateCompass;
                            rotateCompass.Angle = (-1) * Convert.ToDouble(sensorData._reading[index].value[i]);
                            rotateCompass.CenterX = imageCompass.ActualWidth / 2;
                            rotateCompass.CenterY = imageCompass.ActualHeight / 2;
                        }
                        else if (sensorData._property[i].StartsWith("AngularVelocityX"))
                        {
                            RotateTransform rotateGyrometerX = new RotateTransform() { CenterX = imageGyrometerX.ActualWidth / 2, CenterY = imageGyrometerX.ActualHeight / 2 };
                            imageGyrometerX.RenderTransform = rotateGyrometerX;
                            rotateGyrometerX.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData._reading[index].value[i])));
                        }
                        else if (sensorData._property[i].StartsWith("AngularVelocityY"))
                        {
                            RotateTransform rotateGyrometerY = new RotateTransform();
                            imageGyrometerY.RenderTransform = rotateGyrometerY;
                            rotateGyrometerY.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData._reading[index].value[i])));
                            rotateGyrometerY.CenterX = imageGyrometerY.ActualWidth / 2;
                            rotateGyrometerY.CenterY = imageGyrometerY.ActualHeight / 2;
                        }
                        else if (sensorData._property[i].StartsWith("AngularVelocityZ"))
                        {
                            RotateTransform rotateGyrometerZ = new RotateTransform();
                            imageGyrometerZ.RenderTransform = rotateGyrometerZ;
                            rotateGyrometerZ.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData._reading[index].value[i])));
                            rotateGyrometerZ.CenterX = imageGyrometerZ.ActualWidth / 2;
                            rotateGyrometerZ.CenterY = imageGyrometerZ.ActualHeight / 2;
                        }
                        else if (sensorData._property[i].StartsWith("Pitch"))
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = imageInclinometerPitch.ActualWidth / 2, CenterY = imageInclinometerPitch.ActualHeight / 2 };
                            imageInclinometerPitch.RenderTransform = rotate;
                            rotate.Angle = sensorData._reading[index].value[i];
                        }
                        else if (sensorData._property[i].StartsWith("Roll"))
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = imageInclinometerRoll.ActualWidth / 2, CenterY = imageInclinometerRoll.ActualHeight / 2 };
                            imageInclinometerRoll.RenderTransform = rotate;
                            rotate.Angle = sensorData._reading[index].value[i];
                        }
                        else if (sensorData._property[i] == "Yaw (°)")
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = imageInclinometerYaw.ActualWidth / 2, CenterY = imageInclinometerYaw.ActualHeight / 2 };
                            imageInclinometerYaw.RenderTransform = rotate;
                            rotate.Angle = -sensorData._reading[index].value[i];
                        }
                        else if (sensorData._property[i] == "Illuminance (lux)")
                        {
                            textBlockSensor.Text = "💡";
                            if (sensorData._reading[index].value[i] < 1)
                            {
                                textBlockSensor.Opacity = 0.1;
                            }
                            else
                            {
                                textBlockSensor.Opacity = Math.Min(0.1 + Math.Log(sensorData._reading[index].value[i], 2) / 10, 1);
                            }
                        }
                        else if (sensorData._property[i] == "CumulativeSteps")
                        {
                            int value = Convert.ToInt32(sensorData._reading[index].value[i]) / 100;
                            _plotCanvas.SetRange((value + 1) * 100, value * 100);
                        }
                        else if (sensorData._property[i] == "HeadingAccuracy" || sensorData._property[i] == "YawAccuracy")
                        {
                            MagnetometerAccuracy magnetometerAccuracy = (MagnetometerAccuracy)sensorData._reading[index].value[i];
                            textBlockValue[i].Text = string.Format("        {0}", magnetometerAccuracy);
                        }
                        else if (sensorData._property[i] == "IsDetected")
                        {
                            textBlockSensor.Text = (sensorData._reading[index].value[i] > 0.5 ? "📲" : "📱");
                        }
                        else if (sensorData._property[i] == "StepKind")
                        {
                            PedometerStepKind pedometerStepKind = (PedometerStepKind)sensorData._reading[index].value[i];
                            textBlockValue[i].Text = string.Format("        {0}", pedometerStepKind);
                            textBlockSensor.Text = DictionaryStepKind[pedometerStepKind];
                        }
                        else if (sensorData._sensorType == Sensor.SIMPLEORIENTATIONSENSOR)
                        {
                            SimpleOrientation simpleOrientation = (SimpleOrientation)sensorData._reading[index].value[i];
                            textBlockValue[i].Text = string.Format("        {0}", simpleOrientation).Replace("DegreesCounterclockwise", "°↺");
                            textBlockMinValue[i].Text = "";
                            textBlockMaxValue[i].Text = "";
                        }
                        else if (sensorData._sensorType == Sensor.ACTIVITYSENSOR)
                        {
                            if (sensorData._reading[index].value[i] == Sensor.ACTIVITYNONE)
                            {
                                textBlockValue[i].Text = "None";
                            }
                            else if (sensorData._reading[index].value[i] == Sensor.ACTIVITYNOTSUPPORTED)
                            {
                                textBlockValue[i].Text = "Not Supported";
                            }
                            else
                            {
                                ActivitySensorReadingConfidence activitySensorReadingConfidence = (ActivitySensorReadingConfidence)sensorData._reading[index].value[i];
                                textBlockValue[i].Text = string.Format("        {0}", activitySensorReadingConfidence);
                                textBlockSensor.Text = DictionaryActivity[(ActivityType)i];
                            }
                        }
                        else if (sensorData._sensorType == Sensor.LIGHTSENSOR)
                        {
                            if (sensorData._reading[index].value[i] == -1)
                            {
                                textBlockValue[i].Text = "N/A";
                                textBlockMinValue[i].Text = "N/A";
                                textBlockMaxValue[i].Text = "N/A";
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}