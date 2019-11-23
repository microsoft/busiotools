// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Toolkit.Uwp.UI.Controls;
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

        public bool IsOn = false;
        public int Index;
        public int SensorType;
        public PlotCanvas PlotCanvas;
        public StackPanel StackPanelProperty = new StackPanel() { Orientation = Orientation.Horizontal };
        public StackPanel StackPanelSensor = new StackPanel() { Orientation = Orientation.Vertical, Visibility = Visibility.Collapsed, Margin = new Thickness(0, 0, 18, 20) };
        public StackPanel StackPanelTop = new StackPanel() { Orientation = Orientation.Horizontal, Height = 100, HorizontalAlignment = HorizontalAlignment.Center };
        public TextBox TextboxReportInterval = new TextBox() { Height = 32, Width = 100, Margin = new Thickness(40, 20, 10, 10) };

        //MALT
        public Button MALTButton = new Button() { Content = "Include MALT", Height = 50, Width = 200, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 50, 0, 0), FontSize = 20 };
        public StackPanel StackPanelMALTData = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(50, 0, 0, 0), Background = new SolidColorBrush(Colors.AliceBlue), Visibility = Visibility.Collapsed };
        public TextBlock[] TextBlockMALTPropertyValue1;
        public TextBlock[] TextBlockMALTPropertyValue2;

        private Button buttonReportInterval = new Button() { Height = 32, Content = "Change", Margin = new Thickness(0, 20, 10, 10) };
        private Button buttonSensor = new Button();
        private Canvas canvasSensor = new Canvas();
        private Ellipse ellipseAccelerometer = new Ellipse() { Width = 20, Height = 20, Fill = new SolidColorBrush(Colors.DarkRed), Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 };
        private Expander expanderProperty = new Expander() { Header = "Properties" };
        private Image imageCompass = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Compass.png")) };
        private Image imageGyrometerX = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image imageGyrometerY = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image imageGyrometerZ = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Gyrometer.png")) };
        private Image imageInclinometerPitch = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private Image imageInclinometerRoll = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private Image imageInclinometerYaw = new Image() { Source = new BitmapImage(new Uri("ms-appx:/Images/Inclinometer.png")) };
        private int totalIndex;
        private ScrollViewer scrollViewerProperty = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
        private string[] pld = new string[] { "Panel Id", "Panel Group", "Panel Side", "Panel Width (mm)", "Panel Height (mm)", "Panel Length (mm)", "Panel PositionX", "Panel PositionY", "Panel PositionZ", "Panel RotationX", "Panel RotationY", "Panel RotationZ", "Panel Color", "Panel Shape", "Panel Visible" };
        private string[] properties = new string[] { "\r\nReport Interval", "Min Report Interval", "Category", "PersistentUniqueID", "ACPI Object Name", "Manufacturer", "Model", "ConnectionType", "IsPrimary", "Vendor Defined Sub-Type", "State", "Device ID" }; private StackPanel stackPanelBottom = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stackPanelBottomData = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stackPanelBottomRightCol = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stackPanelDataName = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(40, 0, 0, 0) };
        private StackPanel stackPanelExpander = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stackPanelMaxValue = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stackPanelMinValue = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stackPanelPropertyName = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10, 10, 0, 10) };
        private StackPanel stackPanelPropertyValue = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10) };
        private StackPanel stackPanelSwitch = new StackPanel();
        private StackPanel stackPanelValue = new StackPanel() { Orientation = Orientation.Vertical };
        private TextBlock textBlockSensor = new TextBlock() { Foreground = new SolidColorBrush(Colors.Black), FontSize = 72 };
        private TextBlock[] textBlockMaxValue;
        private TextBlock[] textBlockMinValue;
        private TextBlock[] textBlockProperty;
        private TextBlock[] textBlockPropertyName;
        private TextBlock[] textBlockPropertyValue;
        private TextBlock[] textBlockValue;

        // PLD
        private Expander expanderPLD = new Expander() { Header = "Physical Location of Device (PLD)" };
        private ScrollViewer scrollViewerPLD = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
        private StackPanel stackPanelPLD = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stackPanelPLDName = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10, 10, 0, 10) };
        private StackPanel stackPanelPLDValue = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(10) };
        private TextBlock[] textBlockPLDName;
        private TextBlock[] textBlockPLDValue;

        // MALT
        private Button hideMALTButton = new Button() { Content = "Hide", Margin = new Thickness(20) };
        private StackPanel stackPanelMALT2 = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(20, 20, 0, 0) };
        private StackPanel stackPanelMALTPropertyName1 = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.AliceBlue), Margin = new Thickness(20, 20, 0, 0) };
        private StackPanel stackPanelMALTPropertyName2 = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.AliceBlue), Margin = new Thickness(20, 20, 0, 0) };
        private StackPanel stackPanelMALTPropertyValue1 = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.AliceBlue), Margin = new Thickness(20, 20, 0, 0) };
        private StackPanel stackPanelMALTPropertyValue2 = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.AliceBlue), Margin = new Thickness(20, 20, 0, 0) };
        private TextBlock textblockMALTData1 = new TextBlock() { Text = "MALT Light/Color Sensor (Ambient)", FontSize = 20, Margin = new Thickness(20, 10, 0, 0) };
        private TextBlock textblockMALTData2 = new TextBlock() { Text = "MALT Light/Color Sensor (Screen)", FontSize = 20, Margin = new Thickness() { Left = 20, Top = 10 } };
        private TextBlock[] textBlockMALTPropertyName1;
        private TextBlock[] textBlockMALTPropertyName2;

        public SensorDisplay(int sensorType, int index, int totalIndex, int minValue, int maxValue, int scale, Color[] color)
        {
            SensorType = sensorType;
            Index = index;
            this.totalIndex = totalIndex;

            string[] vAxisLabel = new string[scale + 1];
            var resourceLoader = ResourceLoader.GetForCurrentView();          
            StackPanelSensor.Children.Clear();

            expanderProperty.Content = scrollViewerProperty;
            scrollViewerProperty.Content = StackPanelProperty;
            expanderPLD.Content = scrollViewerPLD;
            scrollViewerPLD.Content = stackPanelPLD;

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
            stackPanelSwitch.Children.Add(buttonSensor);

            if (sensorType == Sensor.ACCELEROMETER || sensorType == Sensor.ACCELEROMETERGRAVITY || sensorType == Sensor.ACCELEROMETERLINEAR)
            {
                Grid gridAccelerometer = new Grid();
                gridAccelerometer.Children.Add(new Ellipse() { Width = 100, Height = 100, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 });
                gridAccelerometer.Children.Add(new Ellipse() { Width = 50, Height = 50, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 5 });
                gridAccelerometer.Children.Add(ellipseAccelerometer);
                StackPanelTop.Children.Add(gridAccelerometer);
            }
            else if (sensorType == Sensor.COMPASS)
            {
                StackPanelTop.Children.Add(imageCompass);
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

                StackPanelTop.Children.Add(stackPanelX);
                StackPanelTop.Children.Add(stackPanelY);
                StackPanelTop.Children.Add(stackPanelZ);
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

                StackPanelTop.Children.Add(stackPanelX);
                StackPanelTop.Children.Add(stackPanelY);
                StackPanelTop.Children.Add(stackPanelZ);
            }
            else if (sensorType == Sensor.ACTIVITYSENSOR || sensorType == Sensor.LIGHTSENSOR || sensorType == Sensor.PEDOMETER || sensorType == Sensor.PROXIMITYSENSOR)
            {
                StackPanelTop.Children.Add(textBlockSensor);
            }
            else
            {
                StackPanelTop.Height = 0;
            }

            textBlockProperty = new TextBlock[color.Length + 1];
            textBlockValue = new TextBlock[textBlockProperty.Length];
            textBlockMinValue = new TextBlock[textBlockProperty.Length];
            textBlockMaxValue = new TextBlock[textBlockProperty.Length];

            textBlockProperty[color.Length] = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            stackPanelDataName.Children.Add(textBlockProperty[color.Length]);

            textBlockValue[color.Length] = new TextBlock() { Text = resourceLoader.GetString("Value"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            stackPanelValue.Children.Add(textBlockValue[color.Length]);

            textBlockMinValue[color.Length] = new TextBlock() { Text = resourceLoader.GetString("Min"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            stackPanelMinValue.Children.Add(textBlockMinValue[color.Length]);

            textBlockMaxValue[color.Length] = new TextBlock() { Text = resourceLoader.GetString("Max"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            stackPanelMaxValue.Children.Add(textBlockMaxValue[color.Length]);

            for (int i = 0; i < color.Length; i++)
            {
                textBlockProperty[i] = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
                textBlockProperty[i].Foreground = new SolidColorBrush(color[i]);
                stackPanelDataName.Children.Add(textBlockProperty[i]);

                textBlockValue[i] = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
                textBlockValue[i].Foreground = new SolidColorBrush(color[i]);
                stackPanelValue.Children.Add(textBlockValue[i]);

                textBlockMinValue[i] = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
                textBlockMinValue[i].Foreground = new SolidColorBrush(color[i]);
                stackPanelMinValue.Children.Add(textBlockMinValue[i]);

                textBlockMaxValue[i] = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
                textBlockMaxValue[i].Foreground = new SolidColorBrush(color[i]);
                stackPanelMaxValue.Children.Add(textBlockMaxValue[i]);
            }

            stackPanelExpander.Children.Add(expanderProperty);
            stackPanelExpander.Children.Add(expanderPLD);
            stackPanelBottom.Children.Add(stackPanelExpander);

            if (sensorType == Sensor.LIGHTSENSOR)
            {
                stackPanelBottomData.Children.Add(stackPanelDataName);
                stackPanelBottomData.Children.Add(stackPanelValue);
                stackPanelBottomData.Children.Add(stackPanelMinValue);
                stackPanelBottomData.Children.Add(stackPanelMaxValue);

                stackPanelBottomRightCol.Children.Add(stackPanelBottomData);
                stackPanelBottomRightCol.Children.Add(MALTButton);
                MALTButton.Click += Scenario1View.Scenario1.MALTButton;

                MALTDisplayInitialization();

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
                stackPanelBottom.Children.Add(stackPanelDataName);
                stackPanelBottom.Children.Add(stackPanelValue);
                stackPanelBottom.Children.Add(stackPanelMinValue);
                stackPanelBottom.Children.Add(stackPanelMaxValue);
            }

            textBlockPropertyName = new TextBlock[properties.Length];
            textBlockPropertyValue = new TextBlock[textBlockPropertyName.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                textBlockPropertyName[i] = new TextBlock() { Text = properties[i], HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
                textBlockPropertyValue[i] = new TextBlock() { Text = (i == 0 ? "\r\n" : "") + "        -", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
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

            StackPanelProperty.Children.Add(stackPanelPropertyName);
            StackPanelProperty.Children.Add(stackPanelPropertyValue);

            textBlockPLDName = new TextBlock[pld.Length];
            textBlockPLDValue = new TextBlock[textBlockPLDName.Length];

            for (int i = 0; i < pld.Length; i++)
            {
                textBlockPLDName[i] = new TextBlock() { Text = pld[i], HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
                textBlockPLDValue[i] = new TextBlock() { Text = (i == 0 ? "\r\n" : "") + "        -", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
                stackPanelPLDName.Children.Add(textBlockPLDName[i]);
                stackPanelPLDValue.Children.Add(textBlockPLDValue[i]);
            }

            stackPanelPLD.Children.Add(stackPanelPLDName);
            stackPanelPLD.Children.Add(stackPanelPLDValue);

            StackPanelSensor.Children.Add(StackPanelTop);
            StackPanelSensor.Children.Add(canvasSensor);
            StackPanelSensor.Children.Add(stackPanelBottom);
        }

        private void MALTDisplayInitialization()
        {
            textBlockMALTPropertyName1 = new TextBlock[]
                                            {
                                              new TextBlock() { Text = "Lux", Width = 120 },
                                              new TextBlock() { Text = "Clear", Width = 120, Foreground = new SolidColorBrush(Colors.DarkGray) },
                                              new TextBlock() { Text = "R", Width = 120, Foreground = new SolidColorBrush(Colors.Red) },
                                              new TextBlock() { Text = "G", Width = 120, Foreground = new SolidColorBrush(Colors.Green) },
                                              new TextBlock() { Text = "B", Width = 120, Foreground = new SolidColorBrush(Colors.Blue) }
                                            };
            TextBlockMALTPropertyValue1 = new TextBlock[]
                                            {
                                                new TextBlock() { Width = 120 },
                                                new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.DarkGray) },
                                                new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Red) },
                                                new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Green) },
                                                new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Blue) }
                                            };
            textBlockMALTPropertyName2 = new TextBlock[]
                                            {
                                                new TextBlock() { Text = "Lux", Width = 120 },
                                                new TextBlock() { Text = "Clear", Width = 120, Foreground = new SolidColorBrush(Colors.DarkGray) },
                                                new TextBlock() { Text = "R", Width = 120, Foreground = new SolidColorBrush(Colors.Red) },
                                                new TextBlock() { Text = "G", Width = 120, Foreground = new SolidColorBrush(Colors.Green) },
                                                new TextBlock() { Text = "B", Width = 120, Foreground = new SolidColorBrush(Colors.Blue) }
                                            };
            TextBlockMALTPropertyValue2 = new TextBlock[]
                                            {
                                                new TextBlock() { Width = 120 },
                                                new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.DarkGray) },
                                                new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Red) },
                                                new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Green) },
                                                new TextBlock() { Width = 120, Foreground = new SolidColorBrush(Colors.Blue) }
                                            };
        }

        public void SetWidth(double width, double height)
        {
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
            SetHeight(height * 0.5);
            canvasSensor.Width = width * 0.7;
            scrollViewerProperty.MaxWidth = width * 0.5;
            scrollViewerPLD.MaxWidth = width * 0.5;
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

            for (int i = 0; i < textBlockPLDName.Length; i++)
            {
                textBlockPLDName[i].FontSize = fontSize;
                textBlockPLDValue[i].FontSize = fontSize;
            }

            TextBlock textBlock = new TextBlock() { Text = "00000000" , FontSize = fontSize };
            textBlock.Measure(new Size(200, 200)); // Assuming 200x200 is max size of textblock
            canvasSensor.Margin = new Thickness(textBlock.DesiredSize.Width, textBlock.DesiredSize.Height, 0, textBlock.DesiredSize.Height * 2 );
            PlotCanvas.SetFontSize(fontSize);
        }

        public void SetHeight(double height)
        {
            PlotCanvas.SetHeight(height*0.4);
            canvasSensor.Width = height;
            scrollViewerProperty.MaxHeight = height;
            scrollViewerPLD.MaxHeight = height;
        }

        public void EnableSensor()
        {
            buttonSensor.IsEnabled = true;
            IsOn = !IsOn;
            buttonSensor.SetValue(AutomationProperties.NameProperty, "");
            StackPanelSensor.Visibility = Visibility.Visible;
            StackPanelProperty.Visibility = Visibility.Visible;
            PeriodicTimer.Create(totalIndex);
            Sensor.EnableSensor(SensorType, Index, totalIndex);
        }

        public void UpdateProperty(SensorData sensorData)
        {
            textBlockPropertyValue[0].Text = string.Format("\r\n  {0}", sensorData.ReportInterval != 0 ? sensorData.ReportInterval.ToString() : "-");
            textBlockPropertyValue[1].Text = string.Format("  {0}", sensorData.MinReportInterval != 0 ? sensorData.MinReportInterval.ToString() : "-");
            textBlockPropertyValue[2].Text = "  " + sensorData.Category;
            textBlockPropertyValue[3].Text = "  " + sensorData.PersistentUniqueId;
            textBlockPropertyValue[4].Text = "  " + sensorData.ObjectHierarchy;
            textBlockPropertyValue[5].Text = "  " + sensorData.Manufacturer;
            textBlockPropertyValue[6].Text = "  " + sensorData.Model;
            textBlockPropertyValue[7].Text = "  " + sensorData.ConnectionType;
            textBlockPropertyValue[8].Text = "  " + sensorData.IsPrimary;
            textBlockPropertyValue[9].Text = "  " + sensorData.VendorDefinedSubType;
            textBlockPropertyValue[10].Text = "  " + sensorData.State;
            textBlockPropertyValue[11].Text = $"{sensorData.DeviceId.Replace("{", "\r\n  {")}";
        }

        public void UpdatePLDProperty(SensorData sensorData)
        {
            textBlockPLDValue[0].Text = (sensorData.PanelId == null) ? "null" : sensorData.PanelId;
            textBlockPLDValue[1].Text = (sensorData.PanelGroup == null) ? "null" : Constants.PanelGroup[sensorData.PanelGroup];
            textBlockPLDValue[2].Text = (sensorData.PanelSide == null) ? "null" : Constants.PanelSide[sensorData.PanelSide];
            textBlockPLDValue[3].Text = (sensorData.PanelWidth == null) ? "null" : sensorData.PanelWidth;
            textBlockPLDValue[4].Text = (sensorData.PanelHeight == null) ? "null" : sensorData.PanelHeight;
            textBlockPLDValue[5].Text = (sensorData.PanelLength == null) ? "null" : sensorData.PanelLength;
            textBlockPLDValue[6].Text = (sensorData.PanelPositionX == null) ? "null" : sensorData.PanelPositionX;
            textBlockPLDValue[7].Text = (sensorData.PanelPositionY == null) ? "null" : sensorData.PanelPositionY;
            textBlockPLDValue[8].Text = (sensorData.PanelPositionZ == null) ? "null" : sensorData.PanelPositionZ;
            textBlockPLDValue[9].Text = (sensorData.PanelRotationX == null) ? "null" : Constants.PanelRotation[sensorData.PanelRotationX];
            textBlockPLDValue[10].Text = (sensorData.PanelRotationY == null) ? "null" : Constants.PanelRotation[sensorData.PanelRotationY];
            textBlockPLDValue[11].Text = (sensorData.PanelRotationZ == null) ? "null" : Constants.PanelRotation[sensorData.PanelRotationZ];

            try
            {
                textBlockPLDValue[12].Text = (sensorData.PanelColor == null) ? "null" : "0x" + long.Parse(sensorData.PanelColor).ToString("X");    // convert to hex
            }
            catch { }

            textBlockPLDValue[13].Text = (sensorData.PanelShape == null) ? "null" : Constants.PanelShape[sensorData.PanelShape];
            textBlockPLDValue[14].Text = (sensorData.PanelVisible == null) ? "null" : sensorData.PanelVisible;
        }

        public void UpdateText(SensorData sensorData)
        {
            try
            {
                int index = sensorData.Readings.Count - 1;
                if (sensorData.Count == Sensor.CurrentId)
                {
                    UpdateProperty(sensorData);
                    UpdatePLDProperty(sensorData);
                }

                if (StackPanelSensor.Visibility == Visibility.Visible)
                {
                    if (sensorData.SensorType == Sensor.ACCELEROMETER || sensorData.SensorType == Sensor.ACCELEROMETERLINEAR || sensorData.SensorType == Sensor.ACCELEROMETERGRAVITY)
                    {
                        double margin = 80;
                        double x = Math.Min(1, sensorData.Readings[index].Value[0]);
                        double y = Math.Min(1, sensorData.Readings[index].Value[1]);
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
                                case DisplayOrientations.Landscape: ellipseAccelerometer.Margin = new Thickness(margin * x, 0, 0, margin * y); break;
                                case DisplayOrientations.Portrait: ellipseAccelerometer.Margin = new Thickness(margin * y, 0, 0, -margin * x ); break;
                                case DisplayOrientations.LandscapeFlipped: ellipseAccelerometer.Margin = new Thickness(-margin * x, 0, 0, -margin * y); break;
                                case DisplayOrientations.PortraitFlipped: ellipseAccelerometer.Margin = new Thickness(-margin * y, 0, 0, margin * x ); break;
                            }
                        }
                        else if (displayInformation.NativeOrientation == DisplayOrientations.Portrait)
                        {
                            switch (displayInformation.CurrentOrientation)
                            {
                                case DisplayOrientations.Landscape: ellipseAccelerometer.Margin = new Thickness(-margin * y, 0, 0, margin * x); break;
                                case DisplayOrientations.Portrait: ellipseAccelerometer.Margin = new Thickness(margin * x, 0, 0, margin * y ); break;
                                case DisplayOrientations.LandscapeFlipped: ellipseAccelerometer.Margin = new Thickness(margin * y, 0, 0, -margin * x); break;
                                case DisplayOrientations.PortraitFlipped: ellipseAccelerometer.Margin = new Thickness(-margin * x, 0, 0, -margin * y); break;
                            }
                        }
                    }

                    for (int i = 0; i < sensorData.Readings[index].Value.Length; i++)
                    {
                        textBlockProperty[i].Text = sensorData.Property[i];
                        textBlockValue[i].Text = string.Format("        {0,5:0.00}", sensorData.Readings[index].Value[i]);
                        textBlockMinValue[i].Text = string.Format("        {0,5:0.0}", sensorData.MinValue[i]);
                        textBlockMaxValue[i].Text = string.Format("        {0,5:0.0}", sensorData.MaxValue[i]);

                        if (sensorData.Property[i].StartsWith("MagneticNorth"))
                        {
                            RotateTransform rotateCompass = new RotateTransform();
                            imageCompass.RenderTransform = rotateCompass;
                            rotateCompass.Angle = (-1) * Convert.ToDouble(sensorData.Readings[index].Value[i]);
                            rotateCompass.CenterX = imageCompass.ActualWidth / 2;
                            rotateCompass.CenterY = imageCompass.ActualHeight / 2;
                        }
                        else if (sensorData.Property[i].StartsWith("AngularVelocityX"))
                        {
                            RotateTransform rotateGyrometerX = new RotateTransform() { CenterX = imageGyrometerX.ActualWidth / 2, CenterY = imageGyrometerX.ActualHeight / 2 };
                            imageGyrometerX.RenderTransform = rotateGyrometerX;
                            rotateGyrometerX.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData.Readings[index].Value[i])));
                        }
                        else if (sensorData.Property[i].StartsWith("AngularVelocityY"))
                        {
                            RotateTransform rotateGyrometerY = new RotateTransform();
                            imageGyrometerY.RenderTransform = rotateGyrometerY;
                            rotateGyrometerY.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData.Readings[index].Value[i])));
                            rotateGyrometerY.CenterX = imageGyrometerY.ActualWidth / 2;
                            rotateGyrometerY.CenterY = imageGyrometerY.ActualHeight / 2;
                        }
                        else if (sensorData.Property[i].StartsWith("AngularVelocityZ"))
                        {
                            RotateTransform rotateGyrometerZ = new RotateTransform();
                            imageGyrometerZ.RenderTransform = rotateGyrometerZ;
                            rotateGyrometerZ.Angle = Math.Max(-135, Math.Min(135, Convert.ToDouble(sensorData.Readings[index].Value[i])));
                            rotateGyrometerZ.CenterX = imageGyrometerZ.ActualWidth / 2;
                            rotateGyrometerZ.CenterY = imageGyrometerZ.ActualHeight / 2;
                        }
                        else if (sensorData.Property[i].StartsWith("Pitch"))
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = imageInclinometerPitch.ActualWidth / 2, CenterY = imageInclinometerPitch.ActualHeight / 2 };
                            imageInclinometerPitch.RenderTransform = rotate;
                            rotate.Angle = sensorData.Readings[index].Value[i];
                        }
                        else if (sensorData.Property[i].StartsWith("Roll"))
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = imageInclinometerRoll.ActualWidth / 2, CenterY = imageInclinometerRoll.ActualHeight / 2 };
                            imageInclinometerRoll.RenderTransform = rotate;
                            rotate.Angle = sensorData.Readings[index].Value[i];
                        }
                        else if (sensorData.Property[i] == "Yaw (°)")
                        {
                            RotateTransform rotate = new RotateTransform() { CenterX = imageInclinometerYaw.ActualWidth / 2, CenterY = imageInclinometerYaw.ActualHeight / 2 };
                            imageInclinometerYaw.RenderTransform = rotate;
                            rotate.Angle = -sensorData.Readings[index].Value[i];
                        }
                        else if (sensorData.Property[i] == "Illuminance (lux)")
                        {
                            textBlockSensor.Text = "💡";
                            if (sensorData.Readings[index].Value[i] < 1)
                            {
                                textBlockSensor.Opacity = 0.1;
                            }
                            else
                            {
                                textBlockSensor.Opacity = Math.Min(0.1 + Math.Log(sensorData.Readings[index].Value[i], 2) / 10, 1);
                            }
                        }
                        else if (sensorData.Property[i] == "CumulativeSteps")
                        {
                            int value = Convert.ToInt32(sensorData.Readings[index].Value[i]) / 100;
                            PlotCanvas.SetRange((value + 1) * 100, value * 100);
                        }
                        else if (sensorData.Property[i] == "HeadingAccuracy" || sensorData.Property[i] == "YawAccuracy")
                        {
                            MagnetometerAccuracy magnetometerAccuracy = (MagnetometerAccuracy)sensorData.Readings[index].Value[i];
                            textBlockValue[i].Text = string.Format("        {0}", magnetometerAccuracy);
                        }
                        else if (sensorData.Property[i] == "IsDetected")
                        {
                            textBlockSensor.Text = (sensorData.Readings[index].Value[i] > 0.5 ? "📲" : "📱");
                        }
                        else if (sensorData.Property[i] == "StepKind")
                        {
                            PedometerStepKind pedometerStepKind = (PedometerStepKind)sensorData.Readings[index].Value[i];
                            textBlockValue[i].Text = string.Format("        {0}", pedometerStepKind);
                            textBlockSensor.Text = DictionaryStepKind[pedometerStepKind];
                        }
                        else if (sensorData.SensorType == Sensor.SIMPLEORIENTATIONSENSOR)
                        {
                            SimpleOrientation simpleOrientation = (SimpleOrientation)sensorData.Readings[index].Value[i];
                            textBlockValue[i].Text = string.Format("        {0}", simpleOrientation).Replace("DegreesCounterclockwise", "°↺");
                            textBlockMinValue[i].Text = "";
                            textBlockMaxValue[i].Text = "";
                        }
                        else if (sensorData.SensorType == Sensor.ACTIVITYSENSOR)
                        {
                            if (sensorData.Readings[index].Value[i] == Sensor.ACTIVITYNONE)
                            {
                                textBlockValue[i].Text = "None";
                            }
                            else if (sensorData.Readings[index].Value[i] == Sensor.ACTIVITYNOTSUPPORTED)
                            {
                                textBlockValue[i].Text = "Not Supported";
                            }
                            else
                            {
                                ActivitySensorReadingConfidence activitySensorReadingConfidence = (ActivitySensorReadingConfidence)sensorData.Readings[index].Value[i];
                                textBlockValue[i].Text = string.Format("        {0}", activitySensorReadingConfidence);
                                textBlockSensor.Text = DictionaryActivity[(ActivityType)i];
                            }
                        }
                        else if (sensorData.SensorType == Sensor.LIGHTSENSOR)
                        {
                            if (sensorData.Readings[index].Value[i] == -1)
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