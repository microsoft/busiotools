using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
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
        public static Dictionary<ActivityType, String> DictionaryActivity = new Dictionary<ActivityType, String>
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

        public static Dictionary<PedometerStepKind, String> DictionaryStepKind = new Dictionary<PedometerStepKind, String>
        {
            { PedometerStepKind.Unknown, "❓" },
            { PedometerStepKind.Walking, "🚶" },
            { PedometerStepKind.Running, "🏃" },
        };

        public StackPanel StackPanelSwitch = new StackPanel();
        public Button ButtonSensor = new Button();
        public StackPanel StackPanelSensor = new StackPanel();
        public bool _manualToggle = true;
        public bool _isOn = false;
        public bool _isSelected = false;
        public PlotCanvas _plotCanvas;

        private StackPanel StackPanelTop = new StackPanel();
        private Ellipse EllipseAccelerometer = new Ellipse() { Width = 20, Height = 20, Fill = new SolidColorBrush(Colors.DarkRed), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 };
        private Image ImageCompass = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Compass.png")) };
        private Image ImageGyrometerX = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image ImageGyrometerY = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image ImageGyrometerZ = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image ImageInclinometerPitch = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private Image ImageInclinometerRoll = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private Image ImageInclinometerYaw = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private TextBlock TextBlockSensor = new TextBlock() { Foreground = new SolidColorBrush(Colors.Black), FontSize = 72 };

        private StackPanel StackPanelBottom = new StackPanel();
        private StackPanel StackPanelDataName = new StackPanel();
        private TextBlock[] TextBlockProperty;
        private StackPanel StackPanelValue = new StackPanel();
        private TextBlock[] TextBlockValue;
        private StackPanel StackPanelMinValue = new StackPanel();
        private TextBlock[] TextBlockMinValue;
        private StackPanel StackPanelMaxValue = new StackPanel();
        private TextBlock[] TextBlockMaxValue;

        private StackPanel StackPanelPropertyName = new StackPanel();
        private TextBlock[] TextBlockPropertyName;
        private StackPanel StackPanelPropertyValue = new StackPanel();
        private TextBlock[] TextBlockPropertyValue;

        private StackPanel StackPanelProperty = new StackPanel();
        private Canvas CanvasSensor = new Canvas();

        public int _sensorType;
        public int _index;
        private int _totalIndex;
        private string _name;
        private string[] _properties = new string[] { "\r\nReport Interval", "Min Report Interval", "Category", "PersistentUniqueID", "Manufacturer", "Model", "ConnectionType", "Device ID" };

        private DisplayInformation _displayInformation;
        private Windows.Foundation.Collections.IPropertySet _localState = ApplicationData.Current.LocalSettings.Values;

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

            _plotCanvas = new PlotCanvas(minValue, maxValue, color, CanvasSensor, vAxisLabel);

            StackPanelSwitch.Children.Add(ButtonSensor);

            StackPanelSensor.Margin = new Thickness() { Right = 18, Bottom = 20, Top = 0 };
            StackPanelSensor.Orientation = Orientation.Vertical;
            StackPanelSensor.Visibility = Visibility.Collapsed;

            StackPanelTop.Orientation = Orientation.Horizontal;
            StackPanelTop.Height = 100;
            StackPanelTop.HorizontalAlignment = HorizontalAlignment.Center;

            if (sensorType == Sensor.ACCELEROMETER || sensorType == Sensor.ACCELEROMETERLINEAR || sensorType == Sensor.ACCELEROMETERGRAVITY)
            {
                Grid GridAccelerometer = new Grid();
                GridAccelerometer.Children.Add(new Ellipse() { Width = 100, Height = 100, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 });
                GridAccelerometer.Children.Add(new Ellipse() { Width = 50, Height = 50, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 });
                GridAccelerometer.Children.Add(EllipseAccelerometer);
                StackPanelTop.Children.Add(GridAccelerometer);
            }
            else if (sensorType == Sensor.COMPASS)
            {
                StackPanelTop.Children.Add(ImageCompass);
            }
            else if (sensorType == Sensor.GYROMETER)
            {
                StackPanel StackPanelX = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelX.Children.Add(ImageGyrometerX);
                StackPanelX.Children.Add(new TextBlock() { Text = "X", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel StackPanelY = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelY.Children.Add(ImageGyrometerY);
                StackPanelY.Children.Add(new TextBlock() { Text = "Y", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel StackPanelZ = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelZ.Children.Add(ImageGyrometerZ);
                StackPanelZ.Children.Add(new TextBlock() { Text = "Z", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanelTop.Children.Add(StackPanelX);
                StackPanelTop.Children.Add(StackPanelY);
                StackPanelTop.Children.Add(StackPanelZ);
            }
            else if (sensorType == Sensor.INCLINOMETER)
            {
                StackPanel StackPanelX = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelX.Children.Add(ImageInclinometerPitch);
                StackPanelX.Children.Add(new TextBlock() { Text = "Pitch", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel StackPanelY = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelY.Children.Add(ImageInclinometerRoll);
                StackPanelY.Children.Add(new TextBlock() { Text = "Roll", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel StackPanelZ = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelZ.Children.Add(ImageInclinometerYaw);
                StackPanelZ.Children.Add(new TextBlock() { Text = "Yaw", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanelTop.Children.Add(StackPanelX);
                StackPanelTop.Children.Add(StackPanelY);
                StackPanelTop.Children.Add(StackPanelZ);
            }
            else if (sensorType == Sensor.ACTIVITYSENSOR || sensorType == Sensor.LIGHTSENSOR || sensorType == Sensor.PEDOMETER || sensorType == Sensor.PROXIMITYSENSOR)
            {
                StackPanelTop.Children.Add(TextBlockSensor);
            }
            else
            {
                StackPanelTop.Height = 0;
            }

            StackPanelBottom.Orientation = Orientation.Horizontal;

            StackPanelDataName.Orientation = Orientation.Vertical;
            StackPanelValue.Orientation = Orientation.Vertical;
            StackPanelMinValue.Orientation = Orientation.Vertical;
            StackPanelMaxValue.Orientation = Orientation.Vertical;

            TextBlockProperty = new TextBlock[color.Length + 1];
            TextBlockValue = new TextBlock[TextBlockProperty.Length];
            TextBlockMinValue = new TextBlock[TextBlockProperty.Length];
            TextBlockMaxValue = new TextBlock[TextBlockProperty.Length];

            TextBlockProperty[color.Length] = SetTextStyle("", HorizontalAlignment.Center);
            StackPanelDataName.Children.Add(TextBlockProperty[color.Length]);

            TextBlockValue[color.Length] = SetTextStyle(resourceLoader.GetString("Value"), HorizontalAlignment.Center);
            StackPanelValue.Children.Add(TextBlockValue[color.Length]);

            TextBlockMinValue[color.Length] = SetTextStyle(resourceLoader.GetString("Min"), HorizontalAlignment.Center);
            StackPanelMinValue.Children.Add(TextBlockMinValue[color.Length]);

            TextBlockMaxValue[color.Length] = SetTextStyle(resourceLoader.GetString("Max"), HorizontalAlignment.Center);
            StackPanelMaxValue.Children.Add(TextBlockMaxValue[color.Length]);

            for (int i = 0; i < color.Length; i++)
            {
                TextBlockProperty[i] = SetTextStyle("", HorizontalAlignment.Left);
                TextBlockProperty[i].Foreground = new SolidColorBrush(color[i]);
                StackPanelDataName.Children.Add(TextBlockProperty[i]);

                TextBlockValue[i] = SetTextStyle("", HorizontalAlignment.Right);
                TextBlockValue[i].Foreground = new SolidColorBrush(color[i]);
                StackPanelValue.Children.Add(TextBlockValue[i]);

                TextBlockMinValue[i] = SetTextStyle("", HorizontalAlignment.Right);
                TextBlockMinValue[i].Foreground = new SolidColorBrush(color[i]);
                StackPanelMinValue.Children.Add(TextBlockMinValue[i]);

                TextBlockMaxValue[i] = SetTextStyle("", HorizontalAlignment.Right);
                TextBlockMaxValue[i].Foreground = new SolidColorBrush(color[i]);
                StackPanelMaxValue.Children.Add(TextBlockMaxValue[i]);
            }

            StackPanelDataName.Margin = new Thickness(40, 0, 0, 0);

            StackPanelBottom.Children.Add(StackPanelProperty);
            StackPanelBottom.Children.Add(StackPanelDataName);
            StackPanelBottom.Children.Add(StackPanelValue);
            StackPanelBottom.Children.Add(StackPanelMinValue);
            StackPanelBottom.Children.Add(StackPanelMaxValue);

            StackPanelProperty.Orientation = Orientation.Horizontal;

            StackPanelPropertyName.Orientation = Orientation.Vertical;
            StackPanelPropertyValue.Orientation = Orientation.Vertical;

            TextBlockPropertyName = new TextBlock[_properties.Length];
            TextBlockPropertyValue = new TextBlock[TextBlockPropertyName.Length];

            for (int i = 0; i < _properties.Length; i++)
            {
                TextBlockPropertyName[i] = SetTextStyle(_properties[i], HorizontalAlignment.Left);
                StackPanelPropertyName.Children.Add(TextBlockPropertyName[i]);

                TextBlockPropertyValue[i] = SetTextStyle((i == 0 ? "\r\n" : "") + "        -", HorizontalAlignment.Left);
                StackPanelPropertyValue.Children.Add(TextBlockPropertyValue[i]);
            }

            StackPanelProperty.Children.Add(StackPanelPropertyName);
            StackPanelProperty.Children.Add(StackPanelPropertyValue);

            StackPanelSensor.Children.Add(StackPanelTop);
            StackPanelSensor.Children.Add(CanvasSensor);
            StackPanelSensor.Children.Add(StackPanelBottom);
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
            double stackPanelMinWidth = StackPanelSensor.Margin.Left + StackPanelDataName.Width + StackPanelValue.Width + StackPanelSensor.Margin.Right;
            double stackPanelTextWidth = stackPanelMinWidth + StackPanelMinValue.Width + StackPanelMaxValue.Width; // 348

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
            CanvasSensor.Width = width * 0.7;

            return actualWidth;
        }

        private void SetFontSize(double fontSize)
        {
            for (int i = 0; i < TextBlockProperty.Length; i++)
            {
                TextBlockProperty[i].FontSize = fontSize;
                TextBlockValue[i].FontSize = fontSize;
                TextBlockMinValue[i].FontSize = fontSize;
                TextBlockMaxValue[i].FontSize = fontSize;
            }

            for (int i = 0; i < TextBlockPropertyName.Length; i++)
            {
                TextBlockPropertyName[i].FontSize = fontSize;
                TextBlockPropertyValue[i].FontSize = fontSize;
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "00000000";
            textBlock.FontSize = fontSize;
            textBlock.Measure(new Size(200, 200)); // Assuming 200x200 is max size of textblock
            CanvasSensor.Margin = new Thickness() { Left = textBlock.DesiredSize.Width, Top = textBlock.DesiredSize.Height, Bottom = textBlock.DesiredSize.Height * 2 };
            _plotCanvas.SetFontSize(fontSize);
        }

        public void SetHeight(double height)
        {
            _plotCanvas.SetHeight(height);
        }

        public void EnableSensor()
        {
            ButtonSensor.IsEnabled = true;
            ButtonSensor.Opacity = 1;
            ButtonSensor.SetValue(AutomationProperties.NameProperty, "x");

            _isOn = !_isOn;
            ButtonSensor.SetValue(AutomationProperties.NameProperty, "");
            StackPanelSensor.Visibility = Visibility.Visible;
            StackPanelSensor.Opacity = 1;
            PeriodicTimer.Create(_totalIndex);
            Sensor.EnableSensor(_sensorType, _index, _totalIndex);
        }

        public void UpdateProperty(string deviceId, string deviceName, uint reportInterval, uint minReportInterval, uint reportLatency,
                                   string category, string persistentUniqueId, string manufacturer, string model, string connectionType)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            TextBlockPropertyValue[0].Text = String.Format("\r\n  {0}", reportInterval != 0 ? reportInterval.ToString() : "-");
            TextBlockPropertyValue[1].Text = String.Format("  {0}", minReportInterval != 0 ? minReportInterval.ToString() : "-");
            TextBlockPropertyValue[2].Text = "  " + category;
            TextBlockPropertyValue[3].Text = "  " + persistentUniqueId;
            TextBlockPropertyValue[4].Text = "  " + manufacturer;
            TextBlockPropertyValue[5].Text = "  " + model;
            TextBlockPropertyValue[6].Text = "  " + connectionType;
            TextBlockPropertyValue[7].Text = $"{deviceId.Replace("{", "\r\n  {")}";
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
                                case DisplayOrientations.Landscape: EllipseAccelerometer.Margin = new Thickness() { Left = margin * x, Bottom = margin * y }; break;
                                case DisplayOrientations.Portrait: EllipseAccelerometer.Margin = new Thickness() { Left = margin * y, Bottom = -margin * x }; break;
                                case DisplayOrientations.LandscapeFlipped: EllipseAccelerometer.Margin = new Thickness() { Left = -margin * x, Bottom = -margin * y }; break;
                                case DisplayOrientations.PortraitFlipped: EllipseAccelerometer.Margin = new Thickness() { Left = -margin * y, Bottom = margin * x }; break;
                            }
                        }
                        else if (displayInformation.NativeOrientation == DisplayOrientations.Portrait)
                        {
                            switch (displayInformation.CurrentOrientation)
                            {
                                case DisplayOrientations.Landscape: EllipseAccelerometer.Margin = new Thickness() { Left = -margin * y, Bottom = margin * x }; break;
                                case DisplayOrientations.Portrait: EllipseAccelerometer.Margin = new Thickness() { Left = margin * x, Bottom = margin * y }; break;
                                case DisplayOrientations.LandscapeFlipped: EllipseAccelerometer.Margin = new Thickness() { Left = margin * y, Bottom = -margin * x }; break;
                                case DisplayOrientations.PortraitFlipped: EllipseAccelerometer.Margin = new Thickness() { Left = -margin * x, Bottom = -margin * y }; break;
                            }
                        }
                    }

                    for (int i = 0; i < sensorData._reading[index].value.Length; i++)
                    {
                        TextBlockProperty[i].Text = sensorData._property[i];
                        TextBlockValue[i].Text = String.Format("        {0,5:0.00}", sensorData._reading[index].value[i]);
                        TextBlockMinValue[i].Text = String.Format("        {0,5:0.0}", sensorData._minValue[i]);
                        TextBlockMaxValue[i].Text = String.Format("        {0,5:0.0}", sensorData._maxValue[i]);
                        if (sensorData._property[i].StartsWith("MagneticNorth"))
                        {
                            RotateTransform rotateCompass = new RotateTransform();
                            ImageCompass.RenderTransform = rotateCompass;

                            rotateCompass.Angle = (-1) * Convert.ToDouble(sensorData._reading[index].value[i]);
                            rotateCompass.CenterX = ImageCompass.ActualWidth / 2;
                            rotateCompass.CenterY = ImageCompass.ActualHeight / 2;
                        }
                        else if (sensorData._property[i].StartsWith("AngularVelocityX"))
                        {
                            RotateTransform rotateGyrometerX = new RotateTransform() { CenterX = ImageGyrometerX.ActualWidth / 2, CenterY = ImageGyrometerX.ActualHeight / 2 };
                            ImageGyrometerX.RenderTransform = rotateGyrometerX;

                            rotateGyrometerX.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData._reading[index].value[i])));
                        }
                        else if (sensorData._property[i].StartsWith("AngularVelocityY"))
                        {
                            RotateTransform rotateGyrometerY = new RotateTransform();
                            ImageGyrometerY.RenderTransform = rotateGyrometerY;

                            rotateGyrometerY.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData._reading[index].value[i])));
                            rotateGyrometerY.CenterX = ImageGyrometerY.ActualWidth / 2;
                            rotateGyrometerY.CenterY = ImageGyrometerY.ActualHeight / 2;
                        }
                        else if (sensorData._property[i].StartsWith("AngularVelocityZ"))
                        {
                            RotateTransform rotateGyrometerZ = new RotateTransform();
                            ImageGyrometerZ.RenderTransform = rotateGyrometerZ;

                            rotateGyrometerZ.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData._reading[index].value[i])));
                            rotateGyrometerZ.CenterX = ImageGyrometerZ.ActualWidth / 2;
                            rotateGyrometerZ.CenterY = ImageGyrometerZ.ActualHeight / 2;
                        }
                        else if (sensorData._property[i].StartsWith("Pitch"))
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = ImageInclinometerPitch.ActualWidth / 2, CenterY = ImageInclinometerPitch.ActualHeight / 2 };
                            ImageInclinometerPitch.RenderTransform = rotate;

                            rotate.Angle = sensorData._reading[index].value[i];
                        }
                        else if (sensorData._property[i].StartsWith("Roll"))
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = ImageInclinometerRoll.ActualWidth / 2, CenterY = ImageInclinometerRoll.ActualHeight / 2 };
                            ImageInclinometerRoll.RenderTransform = rotate;

                            rotate.Angle = sensorData._reading[index].value[i];
                        }
                        else if (sensorData._property[i] == "Yaw (°)")
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = ImageInclinometerYaw.ActualWidth / 2, CenterY = ImageInclinometerYaw.ActualHeight / 2 };
                            ImageInclinometerYaw.RenderTransform = rotate;

                            rotate.Angle = -sensorData._reading[index].value[i];
                        }
                        else if (sensorData._property[i] == "Illuminance (lux)")
                        {
                            TextBlockSensor.Text = "💡";
                            if (sensorData._reading[index].value[i] < 1)
                            {
                                TextBlockSensor.Opacity = 0.1;
                            }
                            else
                            {
                                TextBlockSensor.Opacity = Math.Min(0.1 + Math.Log(sensorData._reading[index].value[i], 2) / 10, 1);
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
                            TextBlockValue[i].Text = String.Format("        {0}", magnetometerAccuracy);
                        }
                        else if (sensorData._property[i] == "IsDetected")
                        {
                            TextBlockSensor.Text = (sensorData._reading[index].value[i] > 0.5 ? "📲" : "📱");
                        }
                        else if (sensorData._property[i] == "StepKind")
                        {
                            PedometerStepKind pedometerStepKind = (PedometerStepKind)sensorData._reading[index].value[i];
                            TextBlockValue[i].Text = String.Format("        {0}", pedometerStepKind);

                            TextBlockSensor.Text = DictionaryStepKind[pedometerStepKind];
                        }
                        else if (sensorData._sensorType == Sensor.SIMPLEORIENTATIONSENSOR)
                        {
                            SimpleOrientation simpleOrientation = (SimpleOrientation)sensorData._reading[index].value[i];
                            TextBlockValue[i].Text = String.Format("        {0}", simpleOrientation).Replace("DegreesCounterclockwise", "°↺");
                            TextBlockMinValue[i].Text = "";
                            TextBlockMaxValue[i].Text = "";
                        }
                        else if (sensorData._sensorType == Sensor.ACTIVITYSENSOR)
                        {
                            if (sensorData._reading[index].value[i] == Sensor.ACTIVITYNONE)
                            {
                                TextBlockValue[i].Text = "None";
                            }
                            else if (sensorData._reading[index].value[i] == Sensor.ACTIVITYNOTSUPPORTED)
                            {
                                TextBlockValue[i].Text = "Not Supported";
                            }
                            else
                            {
                                ActivitySensorReadingConfidence activitySensorReadingConfidence = (ActivitySensorReadingConfidence)sensorData._reading[index].value[i];
                                TextBlockValue[i].Text = String.Format("        {0}", activitySensorReadingConfidence);
                                TextBlockSensor.Text = DictionaryActivity[(ActivityType)i];
                            }
                        }
                        else if (sensorData._sensorType == Sensor.LIGHTSENSOR)
                        {
                            if (sensorData._reading[index].value[i] == -1)
                            {
                                TextBlockValue[i].Text = "N/A";
                                TextBlockMinValue[i].Text = "N/A";
                                TextBlockMaxValue[i].Text = "N/A";
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}