using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Sensors;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace SensorExplorer
{
    public sealed partial class Scenario1View : Page
    {
        public static Scenario1View Scenario1;

        private MainPage rootPage = MainPage.Current;
        private List<SensorDisplay> sensorDisplay = new List<SensorDisplay>();
        private List<SensorData> sensorData = new List<SensorData>();
        private ApplicationDataContainer localState = ApplicationData.Current.LocalSettings;
        private Popup settingsPopup; // This is the container that will hold our custom content

        public Scenario1View()
        {
            this.InitializeComponent();
            Scenario1 = this;

            SizeChanged += MainPageSizeChanged;

            EnumerateSensors();

            PeriodicTimer.SensorData = sensorData;
            PeriodicTimer.SensorDisplay = sensorDisplay;

            Sensor.SensorData = sensorData;
            Sensor.SensorDisplay = sensorDisplay;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            try
            {
                Sensor.DisableSensor(Sensor.SensorDisplay[Sensor.currentId].SensorType, Sensor.SensorDisplay[Sensor.currentId].Index);
            }
            catch { }

            rootPage.loggingChannelView.Dispose();
            rootPage.NotifyUser("", NotifyType.StatusMessage);
        }

        private async void EnumerateSensors()
        {
            try
            {
                await Sensor.GetDefault();
                int totalIndex = -1;

                for (int index = 0; index < Sensor.AccelerometerStandardList.Count; index++)
                {
                    totalIndex++;
                    SensorData AccelerometerData = new SensorData(Sensor.ACCELEROMETER, totalIndex, "Accelerometer (Standard)", Constants.AccelerometerPropertyTitles);
                    SensorDisplay AccelerometerDisplay = new SensorDisplay(Sensor.ACCELEROMETER, index, totalIndex, "Accelerometer (Standard)", -2, 2, 2, Constants.AccelerometerColors);
                    sensorDisplay.Add(AccelerometerDisplay);
                    sensorData.Add(AccelerometerData);
                    AddPivotItem(Sensor.ACCELEROMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.AccelerometerLinearList.Count; index++)
                {
                    totalIndex++;
                    SensorData AccelerometerLinearData = new SensorData(Sensor.ACCELEROMETERLINEAR, totalIndex, "Accelerometer (Linear)", Constants.AccelerometerPropertyTitles);
                    SensorDisplay AccelerometerLinearDisplay = new SensorDisplay(Sensor.ACCELEROMETERLINEAR, index, totalIndex, "Accelerometer (Linear)", -2, 2, 2, Constants.AccelerometerColors);
                    sensorDisplay.Add(AccelerometerLinearDisplay);
                    sensorData.Add(AccelerometerLinearData);
                    AddPivotItem(Sensor.ACCELEROMETERLINEAR, index, totalIndex);
                }
                for (int index = 0; index < Sensor.AccelerometerGravityList.Count; index++)
                {
                    totalIndex++;
                    SensorData AccelerometerGravityData = new SensorData(Sensor.ACCELEROMETERGRAVITY, totalIndex, "Accelerometer (Gravity)", Constants.AccelerometerPropertyTitles);
                    SensorDisplay AccelerometerGravityDisplay = new SensorDisplay(Sensor.ACCELEROMETERGRAVITY, index, totalIndex, "Accelerometer (Gravity)", -2, 2, 2, Constants.AccelerometerColors);
                    sensorDisplay.Add(AccelerometerGravityDisplay);
                    sensorData.Add(AccelerometerGravityData);
                    AddPivotItem(Sensor.ACCELEROMETERGRAVITY, index, totalIndex);
                }
                for (int index = 0; index < Sensor.ActivitySensorList.Count; index++)
                {
                    totalIndex++;
                    SensorData ActivitySensorData = new SensorData(Sensor.ACTIVITYSENSOR, totalIndex, "ActivitySensor", Constants.ActivitySensorPropertyTitles);
                    SensorDisplay ActivitySensorDisplay = new SensorDisplay(Sensor.ACTIVITYSENSOR, index, totalIndex, "ActivitySensor", 2, 0, 2, Constants.ActivitySensorColors);
                    sensorDisplay.Add(ActivitySensorDisplay);
                    sensorData.Add(ActivitySensorData);
                    AddPivotItem(Sensor.ACTIVITYSENSOR, index, totalIndex);
                }
                if (Sensor.Altimeter != null)
                {
                    totalIndex++;
                    SensorData AltimeterData = new SensorData(Sensor.ALTIMETER, totalIndex, "Altimeter", Constants.AltimeterPropertyTitles);
                    SensorDisplay AltimeterDisplay = new SensorDisplay(Sensor.ALTIMETER, 0, totalIndex, "Altimeter", -10, 10, 2, Constants.AltimeterColors);
                    sensorDisplay.Add(AltimeterDisplay);
                    sensorData.Add(AltimeterData);
                    AddPivotItem(Sensor.ALTIMETER, 0, totalIndex);
                }
                for (int index = 0; index < Sensor.BarometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData BarometerData = new SensorData(Sensor.BAROMETER, totalIndex, "Barometer", Constants.BarometerPropertyTitles);
                    SensorDisplay BarometerDisplay = new SensorDisplay(Sensor.BAROMETER, index, totalIndex, "Barometer", 950, 1050, 2, Constants.BarometerColors);
                    sensorDisplay.Add(BarometerDisplay);
                    sensorData.Add(BarometerData);
                    AddPivotItem(Sensor.BAROMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.CompassList.Count; index++)
                {
                    totalIndex++;
                    SensorData CompassData = new SensorData(Sensor.COMPASS, totalIndex, "Compass", Constants.CompassPropertyTitles);
                    SensorDisplay CompassDisplay = new SensorDisplay(Sensor.COMPASS, index, totalIndex, "Compass", 0, 360, 2, Constants.CompassColors);
                    sensorDisplay.Add(CompassDisplay);
                    sensorData.Add(CompassData);
                    AddPivotItem(Sensor.COMPASS, index, totalIndex);
                }
                for (int index = 0; index < Sensor.GyrometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData GyrometerData = new SensorData(Sensor.GYROMETER, totalIndex, "Gyrometer", Constants.GyrometerPropertyTitles);
                    SensorDisplay GyrometerDisplay = new SensorDisplay(Sensor.GYROMETER, index, totalIndex, "Gyrometer", -200, 200, 2, Constants.GyrometerColors);
                    sensorDisplay.Add(GyrometerDisplay);
                    sensorData.Add(GyrometerData);
                    AddPivotItem(Sensor.GYROMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.InclinometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData InclinometerData = new SensorData(Sensor.INCLINOMETER, totalIndex, "Inclinometer", Constants.InclinometerPropertyTitles);
                    SensorDisplay InclinometerDisplay = new SensorDisplay(Sensor.INCLINOMETER, index, totalIndex, "Inclinometer", -180, 360, 3, Constants.InclinometerColors);
                    sensorDisplay.Add(InclinometerDisplay);
                    sensorData.Add(InclinometerData);
                    AddPivotItem(Sensor.INCLINOMETER, index, totalIndex);
                }
                if(Sensor.LightSensor != null)
                {
                    totalIndex++;
                    SensorData LightSensorData = new SensorData(Sensor.LIGHTSENSOR, totalIndex, "LightSensor", Constants.LightSensorPropertyTitles);
                    SensorDisplay LightSensorDisplay = new SensorDisplay(Sensor.LIGHTSENSOR, 0, totalIndex, "LightSensor", 0, 1000, 2, Constants.LightSensorColors);
                    sensorDisplay.Add(LightSensorDisplay);
                    sensorData.Add(LightSensorData);
                    AddPivotItem(Sensor.LIGHTSENSOR, 0, totalIndex);
                }
                for (int index = 0; index < Sensor.MagnetometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData MagnetometerData = new SensorData(Sensor.MAGNETOMETER, totalIndex, "Magnetometer", Constants.MagnetometerPropertyTitles);
                    SensorDisplay MagnetometerDisplay = new SensorDisplay(Sensor.MAGNETOMETER, index, totalIndex, "Magnetometer", -500, 500, 2, Constants.MagnetometerColors);
                    sensorDisplay.Add(MagnetometerDisplay);
                    sensorData.Add(MagnetometerData);
                    AddPivotItem(Sensor.MAGNETOMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.OrientationAbsoluteList.Count; index++)
                {
                    totalIndex++;
                    SensorData OrientationAbsoluteData = new SensorData(Sensor.ORIENTATIONSENSOR, totalIndex, "Orientation (Absolute)", Constants.OrientationSensorPropertyTitles);
                    SensorDisplay OrientationAbsoluteDisplay = new SensorDisplay(Sensor.ORIENTATIONSENSOR, index, totalIndex, "Orientation (Absolute)", -1, 1, 2, Constants.OrientationSensorColors);
                    sensorDisplay.Add(OrientationAbsoluteDisplay);
                    sensorData.Add(OrientationAbsoluteData);
                    AddPivotItem(Sensor.ORIENTATIONSENSOR, index, totalIndex);
                }
                for (int index = 0; index < Sensor.OrientationRelativeList.Count; index++)
                {
                    totalIndex++;
                    SensorData OrientationRelativeData = new SensorData(Sensor.ORIENTATIONRELATIVE, totalIndex, "Orientation (Relative)", Constants.OrientationSensorPropertyTitles);
                    SensorDisplay OrientationRelativeDisplay = new SensorDisplay(Sensor.ORIENTATIONRELATIVE, index, totalIndex, "Orientation (Relative)", -1, 1, 2, Constants.OrientationSensorColors);
                    sensorDisplay.Add(OrientationRelativeDisplay);
                    sensorData.Add(OrientationRelativeData);
                    AddPivotItem(Sensor.ORIENTATIONRELATIVE, index, totalIndex);
                }
                for (int index = 0; index < Sensor.OrientationGeomagneticList.Count; index++)
                {
                    totalIndex++;
                    SensorData OrientationGeomagneticData = new SensorData(Sensor.ORIENTATIONGEOMAGNETIC, totalIndex, "Orientation (Geomagnetic)", Constants.OrientationSensorPropertyTitles);
                    SensorDisplay OrientationGeomagneticDisplay = new SensorDisplay(Sensor.ORIENTATIONGEOMAGNETIC, index, totalIndex, "Orientation (Geomagnetic)", -1, 1, 2, Constants.OrientationSensorColors);
                    sensorDisplay.Add(OrientationGeomagneticDisplay);
                    sensorData.Add(OrientationGeomagneticData);
                    AddPivotItem(Sensor.ORIENTATIONGEOMAGNETIC, index, totalIndex);
                }
                for (int index = 0; index < Sensor.PedometerList.Count; index++)
                {
                    totalIndex++;
                    SensorData PedometerData = new SensorData(Sensor.PEDOMETER, totalIndex, "Pedometer", Constants.PedometerPropertyTitles);
                    SensorDisplay PedometerDisplay = new SensorDisplay(Sensor.PEDOMETER, index, totalIndex, "Pedometer", 0, 50, 2, Constants.PedometerColors);
                    sensorDisplay.Add(PedometerDisplay);
                    sensorData.Add(PedometerData);
                    AddPivotItem(Sensor.PEDOMETER, index, totalIndex);
                }
                for (int index = 0; index < Sensor.ProximitySensorList.Count; index++)
                {
                    totalIndex++;
                    SensorData ProximitySensorData = new SensorData(Sensor.PROXIMITYSENSOR, totalIndex, "ProximitySensor", Constants.ProximitySensorPropertyTitles);
                    SensorDisplay ProximitySensorDisplay = new SensorDisplay(Sensor.PROXIMITYSENSOR, index, totalIndex, "ProximitySensor", 0, 1, 1, Constants.ProximitySensorColors);
                    sensorDisplay.Add(ProximitySensorDisplay);
                    sensorData.Add(ProximitySensorData);
                    AddPivotItem(Sensor.PROXIMITYSENSOR, index, totalIndex);
                }
                if (Sensor.SimpleOrientationSensor != null)
                {
                    totalIndex++;
                    SensorData simpleOrientationSensorData = new SensorData(Sensor.SIMPLEORIENTATIONSENSOR, totalIndex, "SimpleOrientationSensor", Constants.SimpleOrientationSensorPropertyTitles);
                    SensorDisplay simpleOrientationSensorDisplay = new SensorDisplay(Sensor.SIMPLEORIENTATIONSENSOR, 0, totalIndex, "SimpleOrientationSensor", 0, 5, 5, Constants.SimpleOrientationSensorColors);
                    sensorDisplay.Add(simpleOrientationSensorDisplay);
                    sensorData.Add(simpleOrientationSensorData);
                    AddPivotItem(Sensor.PROXIMITYSENSOR, 0, totalIndex);
                }

                AddSummaryPage();

                var resourceLoader = ResourceLoader.GetForCurrentView();
                rootPage.NotifyUser(resourceLoader.GetString("NumberOfSensors") + ": " + (PivotSensor.Items.Count - 1), NotifyType.StatusMessage);
                ProgressRingSensor.IsActive = false;

                if (PivotSensor.Items.Count > 0)
                {
                    PivotSensor.SelectionChanged += PivotSensorSelectionChanged;
                    PivotSensor.SelectedIndex = 0;
                    PivotSensorSelectionChanged(null, null);
                }
                else
                {
                    TextBlockNoSensor.Text = resourceLoader.GetString("CannotFindSensor");
                    return;
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }

            ProgressRingSensor.IsActive = false;
        }

        private void AddSummaryPage()
        {
            PivotItem PivotItemSensor = new PivotItem();
            ScrollViewer scrollViewerSensor = new ScrollViewer();
            scrollViewerSensor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewerSensor.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            StackPanel stackpanel = new StackPanel();
            stackpanel.Orientation = Orientation.Horizontal;
            StackPanel stackpanelProperty = new StackPanel();
            StackPanel stackpanelValue = new StackPanel();
            TextBlock[] TextBlockProperties = new TextBlock[18];
            TextBlock[] TextBlockValues = new TextBlock[18];
            for(int i = 0; i < 18; i++)
            {
                TextBlockProperties[i] = new TextBlock();
                TextBlockProperties[i].FontSize = 20;
                stackpanelProperty.Children.Add(TextBlockProperties[i]);

                TextBlockValues[i] = new TextBlock();
                TextBlockValues[i].FontSize = 20;
                TextBlockValues[i].HorizontalAlignment = HorizontalAlignment.Center;
                stackpanelValue.Children.Add(TextBlockValues[i]);
            }

            stackpanel.Children.Add(stackpanelProperty);
            stackpanel.Children.Add(stackpanelValue);

            TextBlockProperties[0].Text = "Sensor Category";
            TextBlockProperties[0].FontWeight = FontWeights.Bold;
            TextBlockProperties[1].Text = "Accelerometer (Standard)";
            TextBlockProperties[2].Text = "Accelerometer (Gravity)";
            TextBlockProperties[3].Text = "Accelerometer (Relative)";
            TextBlockProperties[4].Text = "Activity Sensor";
            TextBlockProperties[5].Text = "Altimeter";
            TextBlockProperties[6].Text = "Barometer";
            TextBlockProperties[7].Text = "Compass";
            TextBlockProperties[8].Text = "Gyrometer";
            TextBlockProperties[9].Text = "Inclinometer";
            TextBlockProperties[10].Text = "Light Sensor";
            TextBlockProperties[11].Text = "Magnetometer";
            TextBlockProperties[12].Text = "Orientation Sensor (Absolute)";
            TextBlockProperties[13].Text = "Orientation Sensor (Geomagnetic)";
            TextBlockProperties[14].Text = "Orientation Sensor (Relative)";
            TextBlockProperties[15].Text = "Pedometer";
            TextBlockProperties[16].Text = "Proximity Sensor";
            TextBlockProperties[17].Text = "Simple Orientation Sensor";

            TextBlockValues[0].Text = "Number of Sensor(s) Available";
            TextBlockValues[0].FontWeight = FontWeights.Bold;
            TextBlockValues[1].Text = Sensor.AccelerometerStandardList.Count.ToString();
            TextBlockValues[2].Text = Sensor.AccelerometerGravityList.Count.ToString();
            TextBlockValues[3].Text = Sensor.AccelerometerLinearList.Count.ToString();
            TextBlockValues[4].Text = Sensor.ActivitySensorList.Count.ToString();
            TextBlockValues[5].Text = (Sensor.Altimeter == null) ? ("0"): ("1");
            TextBlockValues[6].Text = Sensor.BarometerList.Count.ToString();
            TextBlockValues[7].Text = Sensor.CompassList.Count.ToString();
            TextBlockValues[8].Text = Sensor.GyrometerList.Count.ToString();
            TextBlockValues[9].Text = Sensor.InclinometerList.Count.ToString();
            TextBlockValues[10].Text = (Sensor.LightSensor == null) ? ("0") : ("1");
            TextBlockValues[11].Text = Sensor.MagnetometerList.Count.ToString();
            TextBlockValues[12].Text = Sensor.OrientationAbsoluteList.Count.ToString();
            TextBlockValues[13].Text = Sensor.OrientationGeomagneticList .Count.ToString();
            TextBlockValues[14].Text = Sensor.OrientationRelativeList.Count.ToString();
            TextBlockValues[15].Text = Sensor.PedometerList.Count.ToString();
            TextBlockValues[16].Text = Sensor.ProximitySensorList.Count.ToString();
            TextBlockValues[17].Text = (Sensor.SimpleOrientationSensor == null) ? ("0") : ("1");

            PivotItemSensor.Header = "Summary";
            scrollViewerSensor.Content = stackpanel;
            PivotItemSensor.Content = scrollViewerSensor;
            PivotSensor.Items.Add(PivotItemSensor);
        }

        private void AddPivotItem(int sensorType, int index, int totalIndex)
        {
            PivotItem PivotItemSensor = new PivotItem();
            ScrollViewer scrollViewerSensor = new ScrollViewer();
            scrollViewerSensor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewerSensor.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            PivotItemSensor.Header = sensorData[totalIndex].Name + " " + (index + 1);
            scrollViewerSensor.Content = sensorDisplay[totalIndex].StackPanelSensor;
            PivotItemSensor.Content = scrollViewerSensor;
            PivotSensor.Items.Add(PivotItemSensor);
        }

        void MainPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width - 50;
            double actualWidth = 0;
            for (int i = 0; i < sensorDisplay.Count; i++)
            {
                actualWidth = sensorDisplay[i].SetWidth(e.NewSize.Width, e.NewSize.Height);
            }
        }

        private void PivotSensorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < PivotSensor.Items.Count; i++)
            {
                if (i != PivotSensor.SelectedIndex)
                {
                    (((PivotSensor.Items[i] as PivotItem).Content as ScrollViewer).Content as StackPanel).Visibility = Visibility.Collapsed;
                }
                else
                {
                    (((PivotSensor.Items[i] as PivotItem).Content as ScrollViewer).Content as StackPanel).Visibility = Visibility.Visible;
                    if (i != PivotSensor.Items.Count - 1)
                    {
                        if (Sensor.currentId != -1) // disable previous sensor
                        {
                            Sensor.DisableSensor(Sensor.SensorDisplay[Sensor.currentId].SensorType, Sensor.SensorDisplay[Sensor.currentId].Index);
                        }
                        Sensor.currentId = i;   // sensor being displayed                   
                        sensorDisplay[i].EnableSensor();
                        if ((PivotSensor.Items[i] as PivotItem).Header.ToString().Contains("LightSensor"))
                        {
                            saveFileButton.IsEnabled = true;
                        }
                        else
                        {
                            saveFileButton.IsEnabled = false;
                        }
                    }
                }
            }
        }

        public void LogDataLightSensor(LightSensorReading reading)
        {
            LoggingFields loggingFields = new LoggingFields();
            loggingFields.AddString("Timestamp", reading.Timestamp.ToString());
            loggingFields.AddDouble("IlluminanceInLux", reading.IlluminanceInLux);
            rootPage.loggingChannelView.LogEvent("LightSensorData", loggingFields);
        }

        private async void FeedbackButton(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"feedback-hub:?
                                referrer=SensorExplorer&
                                tabid=2&
                                contextid=73&
                                appid=Microsoft.SensorExplorer1_8wekyb3d8bbwe!App&
                                newFeedback=true");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            if (!success)
            {
                rootPage.NotifyUser("Cannot launch Feedback Hub", NotifyType.ErrorMessage);
            }
        }

        public async void SaveFileButtonClick(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("ETL", new List<string>() { ".etl" });
            savePicker.SuggestedFileName = "SensorExplorerLog";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                StorageFile logFileGenerated = await rootPage.loggingSessionView.CloseAndSaveToFileAsync();
                await logFileGenerated.CopyAndReplaceAsync(file);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    rootPage.NotifyUser("File " + file.Name + " was saved.", NotifyType.StatusMessage);
                }
                else if (status == FileUpdateStatus.CompleteAndRenamed)
                {
                    rootPage.NotifyUser("File " + file.Name + " was renamed and saved.", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("File " + file.Name + " couldn't be saved.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("Operation cancelled.", NotifyType.ErrorMessage);
            }

            // start a new loging session
            rootPage.loggingSessionView = new FileLoggingSession("SensorExplorerLogViewNew");
            rootPage.loggingSessionView.AddLoggingChannel(rootPage.loggingChannelView);
        }

        private void ReportIntervalButton(object sender, RoutedEventArgs e)
        {
            try
            {
                SensorDisplay selected = sensorDisplay[Sensor.currentId];
                if(selected.SensorType == Sensor.ACCELEROMETER)
                {
                    Sensor.AccelerometerStandardList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.ACCELEROMETERLINEAR)
                {
                    Sensor.AccelerometerLinearList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.ACCELEROMETERGRAVITY)
                {
                    Sensor.AccelerometerGravityList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                // ActivitySensor doesn't have ReportInterval
                else if (selected.SensorType == Sensor.ALTIMETER)
                {
                    Sensor.Altimeter.ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.BAROMETER)
                {
                    Sensor.BarometerList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.COMPASS)
                {
                    Sensor.CompassList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.GYROMETER)
                {
                    Sensor.GyrometerList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.INCLINOMETER)
                {
                    Sensor.InclinometerList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.LIGHTSENSOR)
                {
                    Sensor.LightSensor.ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.MAGNETOMETER)
                {
                    Sensor.MagnetometerList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.ORIENTATIONSENSOR)
                {
                    Sensor.OrientationAbsoluteList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.ORIENTATIONGEOMAGNETIC)
                {
                    Sensor.OrientationGeomagneticList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.ORIENTATIONRELATIVE)
                {
                    Sensor.OrientationRelativeList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                else if (selected.SensorType == Sensor.PEDOMETER)
                {
                    Sensor.PedometerList[selected.Index].ReportInterval = uint.Parse(textboxReportInterval.Text);
                    sensorData[Sensor.currentId].UpdateReportInterval(uint.Parse(textboxReportInterval.Text));
                }
                //ProximitySensor doesn't have ReportInterval
                //SimpleOrientationSensor doesn't have ReportInterval               
            }
            catch { }
        }
    }
}