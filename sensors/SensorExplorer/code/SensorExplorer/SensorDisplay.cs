using System;
using System.Collections.Generic;
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

        public StackPanel StackPanelSwitch = new StackPanel();
        public Button ButtonSensor = new Button();
        public StackPanel StackPanelSensor = new StackPanel();
        public bool ManualToggle = true;
        public bool IsOn = false;
        public bool IsSelected = false;
        public PlotCanvas PlotCanvas;
        public int SensorType;
        public int Index;

        private StackPanel stackPanelTop = new StackPanel();
        private Ellipse ellipseAccelerometer = new Ellipse() { Width = 20, Height = 20, Fill = new SolidColorBrush(Colors.DarkRed), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 };
        private Image imageCompass = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Compass.png")) };
        private Image imageGyrometerX = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image imageGyrometerY = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image imageGyrometerZ = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image imageInclinometerPitch = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private Image imageInclinometerRoll = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private Image imageInclinometerYaw = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private TextBlock textBlockSensor = new TextBlock() { Foreground = new SolidColorBrush(Colors.Black), FontSize = 72 };
        private StackPanel stackPanelBottom = new StackPanel();
        private StackPanel stackPanelDataName = new StackPanel();
        private TextBlock[] textBlockProperty;
        private StackPanel stackPanelValue = new StackPanel();
        private TextBlock[] textBlockValue;
        private StackPanel stackPanelMinValue = new StackPanel();
        private TextBlock[] textBlockMinValue;
        private StackPanel stackPanelMaxValue = new StackPanel();
        private TextBlock[] textBlockMaxValue;
        private StackPanel stackPanelPropertyName = new StackPanel();
        private TextBlock[] textBlockPropertyName;
        private StackPanel stackPanelPropertyValue = new StackPanel();
        private TextBlock[] textBlockPropertyValue;
        private StackPanel stackPanelProperty = new StackPanel();
        private Canvas canvasSensor = new Canvas();
        private int totalIndex;
        private string name;
        private string[] properties = new string[] { "\r\nReport Interval", "Min Report Interval", "Category", "PersistentUniqueID", "Manufacturer", "Model", "ConnectionType", "Device ID" };

        public SensorDisplay(int sensorType, int index, int totalIndex, string name, int minValue, int maxValue, int scale, Color[] color)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();

            StackPanelSensor.Children.Clear();

            SensorType = sensorType;
            Index = index;
            this.totalIndex = totalIndex;
            this.name = name;

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

            PlotCanvas = new PlotCanvas(minValue, maxValue, color, canvasSensor, vAxisLabel);

            StackPanelSwitch.Children.Add(ButtonSensor);

            StackPanelSensor.Margin = new Thickness() { Right = 18, Bottom = 20, Top = 0 };
            StackPanelSensor.Orientation = Orientation.Vertical;
            StackPanelSensor.Visibility = Visibility.Collapsed;

            stackPanelTop.Orientation = Orientation.Horizontal;
            stackPanelTop.Height = 100;
            stackPanelTop.HorizontalAlignment = HorizontalAlignment.Center;

            if (sensorType == Sensor.ACCELEROMETER || sensorType == Sensor.ACCELEROMETERLINEAR || sensorType == Sensor.ACCELEROMETERGRAVITY)
            {
                Grid GridAccelerometer = new Grid();
                GridAccelerometer.Children.Add(new Ellipse() { Width = 100, Height = 100, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 });
                GridAccelerometer.Children.Add(new Ellipse() { Width = 50, Height = 50, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 });
                GridAccelerometer.Children.Add(ellipseAccelerometer);
                stackPanelTop.Children.Add(GridAccelerometer);
            }
            else if (sensorType == Sensor.COMPASS)
            {
                stackPanelTop.Children.Add(imageCompass);
            }
            else if (sensorType == Sensor.GYROMETER)
            {
                StackPanel StackPanelX = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelX.Children.Add(imageGyrometerX);
                StackPanelX.Children.Add(new TextBlock() { Text = "X", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel StackPanelY = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelY.Children.Add(imageGyrometerY);
                StackPanelY.Children.Add(new TextBlock() { Text = "Y", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel StackPanelZ = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelZ.Children.Add(imageGyrometerZ);
                StackPanelZ.Children.Add(new TextBlock() { Text = "Z", HorizontalAlignment = HorizontalAlignment.Center });

                stackPanelTop.Children.Add(StackPanelX);
                stackPanelTop.Children.Add(StackPanelY);
                stackPanelTop.Children.Add(StackPanelZ);
            }
            else if (sensorType == Sensor.INCLINOMETER)
            {
                StackPanel StackPanelX = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelX.Children.Add(imageInclinometerPitch);
                StackPanelX.Children.Add(new TextBlock() { Text = "Pitch", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel StackPanelY = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelY.Children.Add(imageInclinometerRoll);
                StackPanelY.Children.Add(new TextBlock() { Text = "Roll", HorizontalAlignment = HorizontalAlignment.Center });

                StackPanel StackPanelZ = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 60 };
                StackPanelZ.Children.Add(imageInclinometerYaw);
                StackPanelZ.Children.Add(new TextBlock() { Text = "Yaw", HorizontalAlignment = HorizontalAlignment.Center });

                stackPanelTop.Children.Add(StackPanelX);
                stackPanelTop.Children.Add(StackPanelY);
                stackPanelTop.Children.Add(StackPanelZ);
            }
            else if (sensorType == Sensor.ACTIVITYSENSOR || sensorType == Sensor.LIGHTSENSOR || sensorType == Sensor.PEDOMETER || sensorType == Sensor.PROXIMITYSENSOR)
            {
                stackPanelTop.Children.Add(textBlockSensor);
            }
            else
            {
                stackPanelTop.Height = 0;
            }

            stackPanelBottom.Orientation = Orientation.Horizontal;
            stackPanelDataName.Orientation = Orientation.Vertical;
            stackPanelValue.Orientation = Orientation.Vertical;
            stackPanelMinValue.Orientation = Orientation.Vertical;
            stackPanelMaxValue.Orientation = Orientation.Vertical;

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

            stackPanelDataName.Margin = new Thickness(40, 0, 0, 0);

            stackPanelBottom.Children.Add(stackPanelProperty);
            stackPanelBottom.Children.Add(stackPanelDataName);
            stackPanelBottom.Children.Add(stackPanelValue);
            stackPanelBottom.Children.Add(stackPanelMinValue);
            stackPanelBottom.Children.Add(stackPanelMaxValue);

            stackPanelProperty.Orientation = Orientation.Horizontal;
            stackPanelPropertyName.Orientation = Orientation.Vertical;
            stackPanelPropertyValue.Orientation = Orientation.Vertical;

            textBlockPropertyName = new TextBlock[properties.Length];
            textBlockPropertyValue = new TextBlock[textBlockPropertyName.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                textBlockPropertyName[i] = SetTextStyle(properties[i], HorizontalAlignment.Left);
                stackPanelPropertyName.Children.Add(textBlockPropertyName[i]);

                textBlockPropertyValue[i] = SetTextStyle((i == 0 ? "\r\n" : "") + "        -", HorizontalAlignment.Left);
                stackPanelPropertyValue.Children.Add(textBlockPropertyValue[i]);
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
            double stackPanelTextWidth = stackPanelMinWidth + stackPanelMinValue.Width + stackPanelMaxValue.Width;
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
            PlotCanvas.SetFontSize(fontSize);
        }

        public void SetHeight(double height)
        {
            PlotCanvas.SetHeight(height);
        }

        public void EnableSensor()
        {
            ButtonSensor.IsEnabled = true;
            ButtonSensor.Opacity = 1;
            ButtonSensor.SetValue(AutomationProperties.NameProperty, "x");

            IsOn = !IsOn;
            ButtonSensor.SetValue(AutomationProperties.NameProperty, "");
            StackPanelSensor.Visibility = Visibility.Visible;
            StackPanelSensor.Opacity = 1;
            PeriodicTimer.Create(totalIndex);
            Sensor.EnableSensor(SensorType, Index, totalIndex);
        }

        public void UpdateProperty(string deviceId, string deviceName, uint reportInterval, uint minReportInterval, uint reportLatency,
                                   string category, string persistentUniqueId, string manufacturer, string model, string connectionType)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            textBlockPropertyValue[0].Text = String.Format("\r\n  {0}", reportInterval != 0 ? reportInterval.ToString() : "-");
            textBlockPropertyValue[1].Text = String.Format("  {0}", minReportInterval != 0 ? minReportInterval.ToString() : "-");
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
                int index = sensorData.ReadingList.Count - 1;
                if (sensorData.Count == Sensor.currentId)
                {
                    UpdateProperty(sensorData.DeviceId, sensorData.DeviceName, sensorData.ReportInterval, sensorData.MinReportInterval, sensorData.ReportLatency,
                                   sensorData.Category, sensorData.PersistentUniqueId, sensorData.Manufacturer, sensorData.Model, sensorData.ConnectionType);
                }

                if (StackPanelSensor.Visibility == Visibility.Visible)
                {
                    if (sensorData.SensorType == Sensor.ACCELEROMETER || sensorData.SensorType == Sensor.ACCELEROMETERLINEAR || sensorData.SensorType == Sensor.ACCELEROMETERGRAVITY)
                    {
                        double margin = 80;
                        double x = Math.Min(1, sensorData.ReadingList[index].value[0]);
                        double y = Math.Min(1, sensorData.ReadingList[index].value[1]);
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

                    for (int i = 0; i < sensorData.ReadingList[index].value.Length; i++)
                    {
                        textBlockProperty[i].Text = sensorData.Property[i];
                        textBlockValue[i].Text = String.Format("        {0,5:0.00}", sensorData.ReadingList[index].value[i]);
                        textBlockMinValue[i].Text = String.Format("        {0,5:0.0}", sensorData.MinValue[i]);
                        textBlockMaxValue[i].Text = String.Format("        {0,5:0.0}", sensorData.MaxValue[i]);
                        if (sensorData.Property[i].StartsWith("MagneticNorth"))
                        {
                            RotateTransform rotateCompass = new RotateTransform();
                            imageCompass.RenderTransform = rotateCompass;

                            rotateCompass.Angle = (-1) * Convert.ToDouble(sensorData.ReadingList[index].value[i]);
                            rotateCompass.CenterX = imageCompass.ActualWidth / 2;
                            rotateCompass.CenterY = imageCompass.ActualHeight / 2;
                        }
                        else if (sensorData.Property[i].StartsWith("AngularVelocityX"))
                        {
                            RotateTransform rotateGyrometerX = new RotateTransform() { CenterX = imageGyrometerX.ActualWidth / 2, CenterY = imageGyrometerX.ActualHeight / 2 };
                            imageGyrometerX.RenderTransform = rotateGyrometerX;

                            rotateGyrometerX.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData.ReadingList[index].value[i])));
                        }
                        else if (sensorData.Property[i].StartsWith("AngularVelocityY"))
                        {
                            RotateTransform rotateGyrometerY = new RotateTransform();
                            imageGyrometerY.RenderTransform = rotateGyrometerY;

                            rotateGyrometerY.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData.ReadingList[index].value[i])));
                            rotateGyrometerY.CenterX = imageGyrometerY.ActualWidth / 2;
                            rotateGyrometerY.CenterY = imageGyrometerY.ActualHeight / 2;
                        }
                        else if (sensorData.Property[i].StartsWith("AngularVelocityZ"))
                        {
                            RotateTransform rotateGyrometerZ = new RotateTransform();
                            imageGyrometerZ.RenderTransform = rotateGyrometerZ;

                            rotateGyrometerZ.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData.ReadingList[index].value[i])));
                            rotateGyrometerZ.CenterX = imageGyrometerZ.ActualWidth / 2;
                            rotateGyrometerZ.CenterY = imageGyrometerZ.ActualHeight / 2;
                        }
                        else if (sensorData.Property[i].StartsWith("Pitch"))
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = imageInclinometerPitch.ActualWidth / 2, CenterY = imageInclinometerPitch.ActualHeight / 2 };
                            imageInclinometerPitch.RenderTransform = rotate;

                            rotate.Angle = sensorData.ReadingList[index].value[i];
                        }
                        else if (sensorData.Property[i].StartsWith("Roll"))
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = imageInclinometerRoll.ActualWidth / 2, CenterY = imageInclinometerRoll.ActualHeight / 2 };
                            imageInclinometerRoll.RenderTransform = rotate;

                            rotate.Angle = sensorData.ReadingList[index].value[i];
                        }
                        else if (sensorData.Property[i] == "Yaw (°)")
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = imageInclinometerYaw.ActualWidth / 2, CenterY = imageInclinometerYaw.ActualHeight / 2 };
                            imageInclinometerYaw.RenderTransform = rotate;

                            rotate.Angle = -sensorData.ReadingList[index].value[i];
                        }
                        else if (sensorData.Property[i] == "Illuminance (lux)")
                        {
                            textBlockSensor.Text = "💡";
                            if (sensorData.ReadingList[index].value[i] < 1)
                            {
                                textBlockSensor.Opacity = 0.1;
                            }
                            else
                            {
                                textBlockSensor.Opacity = Math.Min(0.1 + Math.Log(sensorData.ReadingList[index].value[i], 2) / 10, 1);
                            }
                        }
                        else if (sensorData.Property[i] == "CumulativeSteps")
                        {
                            int value = Convert.ToInt32(sensorData.ReadingList[index].value[i]) / 100;
                            PlotCanvas.SetRange((value + 1) * 100, value * 100);
                        }
                        else if (sensorData.Property[i] == "HeadingAccuracy" || sensorData.Property[i] == "YawAccuracy")
                        {
                            MagnetometerAccuracy magnetometerAccuracy = (MagnetometerAccuracy)sensorData.ReadingList[index].value[i];
                            textBlockValue[i].Text = String.Format("        {0}", magnetometerAccuracy);
                        }
                        else if (sensorData.Property[i] == "IsDetected")
                        {
                            textBlockSensor.Text = (sensorData.ReadingList[index].value[i] > 0.5 ? "📲" : "📱");
                        }
                        else if (sensorData.Property[i] == "StepKind")
                        {
                            PedometerStepKind pedometerStepKind = (PedometerStepKind)sensorData.ReadingList[index].value[i];
                            textBlockValue[i].Text = String.Format("        {0}", pedometerStepKind);

                            textBlockSensor.Text = DictionaryStepKind[pedometerStepKind];
                        }
                        else if (sensorData.SensorType == Sensor.SIMPLEORIENTATIONSENSOR)
                        {
                            SimpleOrientation simpleOrientation = (SimpleOrientation)sensorData.ReadingList[index].value[i];
                            textBlockValue[i].Text = String.Format("        {0}", simpleOrientation).Replace("DegreesCounterclockwise", "°↺");
                            textBlockMinValue[i].Text = "";
                            textBlockMaxValue[i].Text = "";
                        }
                        else if (sensorData.SensorType == Sensor.ACTIVITYSENSOR)
                        {
                            if (sensorData.ReadingList[index].value[i] == Sensor.ACTIVITYNONE)
                            {
                                textBlockValue[i].Text = "None";
                            }
                            else if (sensorData.ReadingList[index].value[i] == Sensor.ACTIVITYNOTSUPPORTED)
                            {
                                textBlockValue[i].Text = "Not Supported";
                            }
                            else
                            {
                                ActivitySensorReadingConfidence activitySensorReadingConfidence = (ActivitySensorReadingConfidence)sensorData.ReadingList[index].value[i];
                                textBlockValue[i].Text = String.Format("        {0}", activitySensorReadingConfidence);
                                textBlockSensor.Text = DictionaryActivity[(ActivityType)i];
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}