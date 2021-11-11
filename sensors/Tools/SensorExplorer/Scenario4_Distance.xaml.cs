// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace SensorExplorer
{
    public sealed partial class Scenario4Distance : Page
    {
        public static Scenario4Distance Scenario4;

        private DistanceState _currentState;
        private MediaCapture _mediaCapture;
        private bool _isPreviewing;

        private IMediaEncodingProperties _previewProperties;

        private FaceDetectionEffect _faceDetectionEffect;

        private DisplayRequest _displayRequest = new DisplayRequest();

        private ProximitySensor _proximitySensor;
        private int _currentSensorNumber = 0;

        Queue<ProximitySensorReading> _proximitySensorReadingBuffer;
        ProximitySensorReading _lastProximitySensorReading;

        private const int _proximityDefaultCountdownTimeSec = 20;
        private const int _singleResultTotalWaitTimer = 3;
        private const int _proximitySensorDistanceThresholdPercent = 5; // Values from the Proximity Sensor are deemed the same if they are within 5% of each other
        private const int _proximitySensorDistanceThresholdTimeSec = 3; // Proximity Sensor must stay at the same location for 3 seconds
        private const int _proximitySensorDefaultErrorPercent = 30; // The Proximity Sensor is good enough if it's within 30% of the target distance
        private const int _proximitySensorMaxErrorPercent = 100; // Create an incredibly large max bound for error percent

        // from https://medium.com/swlh/estimating-the-object-distance-using-the-camera-obscura-formulas-and-lens-equations-python-7baaa75a26b8
        private const float _distanceScalerMultiplier = 150.0f;
        private const float _distanceScalerPixelsAdder = 226.8f;

        private int[] _proximityDistancesMm = { 400, 800, 1200 };
        private bool[] _manualDistancesSucceeded = { false, false, false };
        private Color[] _manualTestColors = { Colors.DarkMagenta, Colors.DarkOrange, Colors.DarkBlue };
        private int _currentManualDistance = 0;
        private int _singleResultWaitTimer = 0;
        private int _currentCountdown = 20;

        private int _currentCameraNumber = 0;

        private const int _minDistance = 300;
        private const int _stopMovingForwardThresholdMm = 500;
        private int _closeToMidDistanceCutoffMm = 900;
        private int _midToFarDistanceCutoffMm = 1300;
        private int _stopMovingBackwardsThresholdMm = 1700;

        Queue<int> _cameraReadingDistancesMm;
        Queue<DateTimeOffset> _cameraReadingTimeStamps;
        int _lastCameraDistanceMm;
        DateTimeOffset _lastCameraReadingTimeStamp;

        private int _maxDistancePlotMm = 2000;
        private const int _minDistancePlotMm = 0;
        private const int _plotYNumIntervals = 4;

        private PlotCanvas _plotCanvas;

        List<int> _manualProximityDistances = new List<int>();
        List<DateTimeOffset> _manualProximityTimestamps = new List<DateTimeOffset>();
        List<int> _manualTargetDistances = new List<int>();
        List<DateTimeOffset> _manualTargetTimestamps = new List<DateTimeOffset>();

        public enum DistanceState
        {
            TestSelectionScreen,
            TapeMeasureTestsInstructionScreen,
            TapeMeasureTargetDistanceScreen,
            TapeMeasureSingleTestPassScreen,
            TapeMeasureSingleTestFailedScreen,
            TapeMeasureResultsScreen,
            CameraTestsInstructionScreen,
            CameraTestsMoveBackScreen,
            CameraTestsMoveForwardScreen,
            CameraTestsResultsScreen
        }

        public Scenario4Distance()
        {
            InitializeComponent();
            Scenario4 = this;
            PeriodicTimer.CreateDistanceTimer();

            InitializeProximitySensor();

            Application.Current.Suspending += Application_Suspending;
        }

        private void InitializeProximitySensor()
        {
            if (Sensor.ProximitySensorList.Count > 1)
            {
                ChangeProximitySensorButton.Visibility = Visibility.Visible;
                RunAutomaticTestsButton.Visibility = Visibility.Visible;
                RunManualTestsButton.Visibility = Visibility.Visible;
                CurrentProximitySensorName.Text = GetSensorName();
                ProximitySensorFoundTextBox.Text = "Using proximity sensor 1 of " + Sensor.ProximitySensorList.Count;
                if (IsHumanPresenceSensor())
                {
                    ProximitySensorFoundTextBox.Text = "Using proximity sensor (human presence) 1 of " + Sensor.ProximitySensorList.Count;
                }
            }
            else if (Sensor.ProximitySensorList.Count == 1)
            {
                RunAutomaticTestsButton.Visibility = Visibility.Visible;
                RunManualTestsButton.Visibility = Visibility.Visible;
                CurrentProximitySensorName.Text = GetSensorName();
                ProximitySensorFoundTextBox.Text = "Using proximity sensor 1 of " + Sensor.ProximitySensorList.Count;
                if (IsHumanPresenceSensor())
                {
                    ProximitySensorFoundTextBox.Text = "Using proximity sensor (human presence) 1 of " + Sensor.ProximitySensorList.Count;
                }
            }

            if (Sensor.ProximitySensorList.Count >= 1)
            {
                _proximitySensor = Sensor.ProximitySensorList[0];
            }

            _proximitySensorReadingBuffer = new Queue<ProximitySensorReading>();
            _cameraReadingDistancesMm = new Queue<int>();
            _cameraReadingTimeStamps = new Queue<DateTimeOffset>();
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            PeriodicTimer.CancelDistanceTimer();

            await CleanupCameraAsync();
            if (_proximitySensor != null)
            {
                _proximitySensor.ReadingChanged -= ProximitySensorReadingChangedAsync;
            }
        }

        // This function is called every second by a timer. This function evaluates the current state and moves the device to a new state if needed.
        public async Task EvaluateStateEverySecondAsync()
        {
            switch (_currentState)
            {
                case DistanceState.TapeMeasureTargetDistanceScreen:
                    CurrentTimerTextBox.Text = "Timer: " + _currentCountdown + " seconds";
                    if (_currentCountdown == GetCurrentTimeout())
                    {
                        _manualTargetDistances.Add(0);
                        _manualTargetTimestamps.Add(DateTime.Now);
                        _manualTargetDistances.Add(_proximityDistancesMm[_currentManualDistance]);
                        _manualTargetTimestamps.Add(DateTime.Now);
                    }
                    _currentCountdown--;

                    bool evaluateResult = false;

                    if (_proximitySensorReadingBuffer.Count > 0 && DateTimeOffset.Now.Subtract(_proximitySensorReadingBuffer.Peek().Timestamp) >= TimeSpan.FromSeconds(_proximitySensorDistanceThresholdTimeSec))
                    {
                        _proximitySensorReadingBuffer.Clear();
                        evaluateResult = true;
                    }
                    else if (_proximitySensorReadingBuffer.Count > 0 && _lastProximitySensorReading.Timestamp.Subtract(_proximitySensorReadingBuffer.Peek().Timestamp) >= TimeSpan.FromSeconds(_proximitySensorDistanceThresholdTimeSec))
                    {
                        while (_proximitySensorReadingBuffer.Count > 0 && _lastProximitySensorReading.Timestamp.Subtract(_proximitySensorReadingBuffer.Peek().Timestamp) >= TimeSpan.FromSeconds(_proximitySensorDistanceThresholdTimeSec))
                        {
                            _proximitySensorReadingBuffer.Dequeue();
                        }

                        while (_proximitySensorReadingBuffer.Count > 0 &&
                            _proximitySensorReadingBuffer.Peek().DistanceInMillimeters * (100.0f + _proximitySensorDistanceThresholdPercent) / 100.0f >= _lastProximitySensorReading.DistanceInMillimeters &&
                            _lastProximitySensorReading.DistanceInMillimeters * (100.0f + _proximitySensorDistanceThresholdPercent) / 100.0f >= _proximitySensorReadingBuffer.Peek().DistanceInMillimeters)
                        {
                            _proximitySensorReadingBuffer.Dequeue();
                        }
                        evaluateResult = true;
                    }

                    if (evaluateResult && (_proximitySensorReadingBuffer.Count == 0))
                    {
                        int errorPercent = _proximitySensorDefaultErrorPercent;
                        bool parsedSuccess = Int32.TryParse(ManualErrorMarginPercentTextBox.Text, out errorPercent);
                        if (!parsedSuccess || (errorPercent > _proximitySensorMaxErrorPercent))
                        {
                            errorPercent = _proximitySensorDefaultErrorPercent;
                        }

                        if (_proximityDistancesMm[_currentManualDistance] * (100.0f + errorPercent) / 100.0f >= _lastProximitySensorReading.DistanceInMillimeters &&
                            _lastProximitySensorReading.DistanceInMillimeters * (100.0f + errorPercent) / 100.0f >= _proximityDistancesMm[_currentManualDistance])
                        {
                            SuccessImage.Visibility = Visibility.Visible;
                            _currentState = DistanceState.TapeMeasureSingleTestPassScreen;
                            _manualDistancesSucceeded[_currentManualDistance] = true;
                        }
                        else
                        {
                            FailureImage.Visibility = Visibility.Visible;
                            _currentState = DistanceState.TapeMeasureSingleTestFailedScreen;
                            _manualDistancesSucceeded[_currentManualDistance] = false;
                        }
                        _currentCountdown = GetCurrentTimeout();

                        _manualTargetDistances.Add(_proximityDistancesMm[_currentManualDistance]);
                        _manualTargetTimestamps.Add(DateTime.Now);
                        _manualTargetDistances.Add(0);
                        _manualTargetTimestamps.Add(DateTime.Now);
                    }

                    if (_currentCountdown <= 0)
                    {
                        FailureImage.Visibility = Visibility.Visible;
                        _currentState = DistanceState.TapeMeasureSingleTestFailedScreen;
                        _manualDistancesSucceeded[_currentManualDistance] = false;
                        _currentCountdown = GetCurrentTimeout();
                        _proximitySensorReadingBuffer.Clear();

                        _manualTargetDistances.Add(_proximityDistancesMm[_currentManualDistance]);
                        _manualTargetTimestamps.Add(DateTime.Now);
                        _manualTargetDistances.Add(0);
                        _manualTargetTimestamps.Add(DateTime.Now);
                    }
                    break;
                case DistanceState.TapeMeasureSingleTestPassScreen:
                case DistanceState.TapeMeasureSingleTestFailedScreen:
                    _singleResultWaitTimer++;
                    if (_singleResultWaitTimer >= _singleResultTotalWaitTimer)
                    {
                        SuccessImage.Visibility = Visibility.Collapsed;
                        FailureImage.Visibility = Visibility.Collapsed;
                        _currentManualDistance++;
                        if (_currentManualDistance >= _proximityDistancesMm.Length)
                        {
                            PassedDistances.Text = "Passed Distances: ";
                            FailedDistances.Text = "Failed Distances: ";
                            for (int i = 0; i < _manualDistancesSucceeded.Length; i++)
                            {
                                if (_manualDistancesSucceeded[i])
                                {
                                    if (PassedDistances.Text[PassedDistances.Text.Length - 1] != ' ')
                                    {
                                        PassedDistances.Text += ", ";
                                    }
                                    PassedDistances.Text += _proximityDistancesMm[i] + "mm";
                                }
                                else
                                {
                                    if (FailedDistances.Text[FailedDistances.Text.Length - 1] != ' ')
                                    {
                                        FailedDistances.Text += ", ";
                                    }
                                    FailedDistances.Text += _proximityDistancesMm[i] + "mm";
                                }
                            }

                            List<List<int>> distanceMatrix = new List<List<int>>();
                            List<List<DateTimeOffset>> timestampMatrix = new List<List<DateTimeOffset>>();

                            string[] vAxisLabel = new string[_plotYNumIntervals + 1];
                            for (int i = 0; i <= _plotYNumIntervals; i++)
                            {
                                vAxisLabel[i] = (_maxDistancePlotMm - Convert.ToDouble(i) / _plotYNumIntervals * (_maxDistancePlotMm - _minDistancePlotMm)).ToString();
                            }

                            _plotCanvas = new PlotCanvas(_minDistancePlotMm, _maxDistancePlotMm, Constants.ProximitySensorColors, ManualResultsPlot, vAxisLabel);

                            distanceMatrix.Add(_manualProximityDistances);
                            timestampMatrix.Add(_manualProximityTimestamps);
                            distanceMatrix.Add(_manualTargetDistances);
                            timestampMatrix.Add(_manualTargetTimestamps);

                            _plotCanvas.PlotGroup(distanceMatrix, timestampMatrix);

                            ManualResultsPanel.Visibility = Visibility.Visible;
                            ManualPanel.Visibility = Visibility.Collapsed;
                            ManualLongTextTextBox.Visibility = Visibility.Visible;
                            ProximityImageManual.Visibility = Visibility.Visible;
                            ManualCustomConfigOptionsTextBox.Visibility = Visibility.Visible;
                            ManualErrorMarginPercentTextBox.Visibility = Visibility.Visible;
                            TestTimeLengthInSecondsTextBox.Visibility = Visibility.Visible;
                            StartManualTestsButton.Visibility = Visibility.Visible;
                            ManualGoToDistanceTextBox.Visibility = Visibility.Collapsed;
                            CurrentTimerTextBox.Visibility = Visibility.Collapsed;
                            ManualDistanceTextBox.Visibility = Visibility.Collapsed;
                            CurrentDistanceFromSensorTextBox.Visibility = Visibility.Collapsed;
                            _currentState = DistanceState.TapeMeasureResultsScreen;
                            _currentManualDistance = 0;
                            ManualDistanceTextBox.Text = _proximityDistancesMm[_currentManualDistance] + "mm";
                            ManualDistanceTextBox.Foreground = new SolidColorBrush(_manualTestColors[_currentManualDistance]);
                            ManualGoToDistanceTextBox.Foreground = new SolidColorBrush(_manualTestColors[_currentManualDistance]);
                        }
                        else
                        {
                            ManualDistanceTextBox.Text = _proximityDistancesMm[_currentManualDistance] + "mm";
                            ManualDistanceTextBox.Foreground = new SolidColorBrush(_manualTestColors[_currentManualDistance]);
                            ManualGoToDistanceTextBox.Foreground = new SolidColorBrush(_manualTestColors[_currentManualDistance]);
                            _currentState = DistanceState.TapeMeasureTargetDistanceScreen;
                            _proximitySensorReadingBuffer.Clear();
                        }
                        _singleResultWaitTimer = 0;
                    }
                    break;
                case DistanceState.CameraTestsMoveBackScreen:
                    if (_cameraReadingTimeStamps.Count > 0)
                    {
                        if (_lastCameraDistanceMm > _stopMovingBackwardsThresholdMm)
                        {
                            MoveDirectionTextBox.Text = "Move forwards!";
                            MoveDirectionTextBox.Foreground = new SolidColorBrush(Colors.DarkMagenta);
                            _currentState = DistanceState.CameraTestsMoveForwardScreen;
                        }
                    }
                    break;
                case DistanceState.CameraTestsMoveForwardScreen:
                    if (_lastCameraDistanceMm < _stopMovingForwardThresholdMm)
                    {
                        if (_proximitySensorReadingBuffer.Count == 0)
                        {
                            CloseDistancesRatio.Text = "Proximity Sensor did not report any samples.";
                            MediumDistancesRatio.Text = "Please check your sensor on the View page";
                            FarDistancesRatio.Text = "";
                            AutoPlotHeader1.Visibility = Visibility.Collapsed;
                            AutoPlotHeader2.Visibility = Visibility.Collapsed;
                            AutoPlotHeader3.Visibility = Visibility.Collapsed;
                            AutoResultsPlot.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            int closeTotalDistanceCount = 0;
                            int closeGoodDistanceCount = 0;
                            int mediumTotalDistanceCount = 0;
                            int mediumGoodDistanceCount = 0;
                            int farTotalDistanceCount = 0;
                            int farGoodDistanceCount = 0;
                            ProximitySensorReading previousSensorReading = _proximitySensorReadingBuffer.Peek();

                            List<List<int>> distanceMatrix = new List<List<int>>();
                            List<List<DateTimeOffset>> timestampMatrix = new List<List<DateTimeOffset>>();

                            List<int> proximityDistances = new List<int>();
                            List<DateTimeOffset> proximityTimestamps = new List<DateTimeOffset>();
                            List<int> cameraDistances = new List<int>();
                            List<DateTimeOffset> cameraTimestamps = new List<DateTimeOffset>();

                            // For each camera reading, find the sensor reading with the timestamp slightly before it and the timestamp after it. Use the value closest to the real distance
                            while (_cameraReadingTimeStamps.Count > 0)
                            {
                                int curCameraDistanceMm = _cameraReadingDistancesMm.Peek();
                                DateTimeOffset curCameraTimestamp = _cameraReadingTimeStamps.Peek();
                                while ((_proximitySensorReadingBuffer.Count > 0) && (_proximitySensorReadingBuffer.Peek().Timestamp < curCameraTimestamp))
                                {
                                    previousSensorReading = _proximitySensorReadingBuffer.Peek();
                                    proximityDistances.Add((int)previousSensorReading.DistanceInMillimeters);
                                    proximityTimestamps.Add(previousSensorReading.Timestamp);
                                    _proximitySensorReadingBuffer.Dequeue();
                                }

                                ProximitySensorReading nextSensorReading = previousSensorReading;
                                if (_proximitySensorReadingBuffer.Count > 0)
                                {
                                    nextSensorReading = _proximitySensorReadingBuffer.Peek();
                                }

                                int changeBetweenCameraAndPrevSensorDistanceMm = Math.Abs(curCameraDistanceMm - (int)(previousSensorReading.DistanceInMillimeters));
                                int changeBetweenCameraAndNextSensorDistanceMm = Math.Abs(curCameraDistanceMm - (int)(nextSensorReading.DistanceInMillimeters));

                                int errorPercent = _proximitySensorDefaultErrorPercent;
                                bool parsedSuccess = Int32.TryParse(AutoErrorMarginPercentTextBox.Text, out errorPercent);
                                if (!parsedSuccess || (errorPercent > _proximitySensorMaxErrorPercent))
                                {
                                    errorPercent = _proximitySensorDefaultErrorPercent;
                                }

                                if (Math.Min(changeBetweenCameraAndNextSensorDistanceMm, changeBetweenCameraAndPrevSensorDistanceMm) * 100.0f <= curCameraDistanceMm * errorPercent)
                                {
                                    if (curCameraDistanceMm > _midToFarDistanceCutoffMm)
                                    {
                                        farGoodDistanceCount++;
                                    }
                                    else if (curCameraDistanceMm < _closeToMidDistanceCutoffMm)
                                    {
                                        closeGoodDistanceCount++;
                                    }
                                    else
                                    {
                                        mediumGoodDistanceCount++;
                                    }
                                }

                                if (curCameraDistanceMm > _midToFarDistanceCutoffMm)
                                {
                                    farTotalDistanceCount++;
                                }
                                else if (curCameraDistanceMm < _closeToMidDistanceCutoffMm)
                                {
                                    closeTotalDistanceCount++;
                                }
                                else
                                {
                                    mediumTotalDistanceCount++;
                                }
                                cameraDistances.Add(_cameraReadingDistancesMm.Peek());
                                cameraTimestamps.Add(_cameraReadingTimeStamps.Peek());
                                _cameraReadingDistancesMm.Dequeue();
                                _cameraReadingTimeStamps.Dequeue();
                            }

                            while (_proximitySensorReadingBuffer.Count > 0)
                            {
                                previousSensorReading = _proximitySensorReadingBuffer.Peek();
                                proximityDistances.Add((int)previousSensorReading.DistanceInMillimeters);
                                proximityTimestamps.Add(previousSensorReading.Timestamp);
                                _proximitySensorReadingBuffer.Dequeue();
                            }
                            CloseDistancesRatio.Text = "Close Distances (<" + _closeToMidDistanceCutoffMm + "mm) Success Ratio: " + closeGoodDistanceCount + "/" + closeTotalDistanceCount + "=" + (int)(100.0f * closeGoodDistanceCount / Math.Max(closeTotalDistanceCount, 1)) + "%";
                            MediumDistancesRatio.Text = "Medium Distances (" + _closeToMidDistanceCutoffMm + "-" + _midToFarDistanceCutoffMm + "mm) Success Ratio: " + mediumGoodDistanceCount + "/" + mediumTotalDistanceCount + "=" + (int)(100.0f * mediumGoodDistanceCount / Math.Max(mediumTotalDistanceCount, 1)) + "%";
                            FarDistancesRatio.Text = "Far Distances(>" + _midToFarDistanceCutoffMm + "mm) Success Ratio: " + farGoodDistanceCount + "/" + farTotalDistanceCount + "=" + (int)(100.0f * farGoodDistanceCount / Math.Max(farTotalDistanceCount, 1)) + "%";
                            AutoPlotHeader1.Visibility = Visibility.Visible;
                            AutoPlotHeader2.Visibility = Visibility.Visible;
                            AutoPlotHeader3.Visibility = Visibility.Visible;
                            AutoResultsPlot.Visibility = Visibility.Visible;

                            string[] vAxisLabel = new string[_plotYNumIntervals + 1];
                            for (int i = 0; i <= _plotYNumIntervals; i++)
                            {
                                vAxisLabel[i] = (_maxDistancePlotMm - Convert.ToDouble(i) / _plotYNumIntervals * (_maxDistancePlotMm - _minDistancePlotMm)).ToString();
                            }

                            _plotCanvas = new PlotCanvas(_minDistancePlotMm, _maxDistancePlotMm, Constants.ProximitySensorColors, AutoResultsPlot, vAxisLabel);

                            distanceMatrix.Add(proximityDistances);
                            timestampMatrix.Add(proximityTimestamps);
                            distanceMatrix.Add(cameraDistances);
                            timestampMatrix.Add(cameraTimestamps);

                            _plotCanvas.PlotGroup(distanceMatrix, timestampMatrix);
                        }
                        PreviewControl.Visibility = Visibility.Collapsed;
                        FacesCanvas.Visibility = Visibility.Collapsed;
                        AutoPanel.Visibility = Visibility.Collapsed;
                        AutomaticLongTextTextBox.Visibility = Visibility.Visible;
                        AutoCustomConfigOptionsTextBox.Visibility = Visibility.Visible;
                        AutoErrorMarginPercentTextBox.Visibility = Visibility.Visible;
                        MoveDirectionTextBox.Visibility = Visibility.Collapsed;
                        StartAutomaticTestsButton.Visibility = Visibility.Visible;
                        AutoResultsPanel.Visibility = Visibility.Visible;
                        _currentState = DistanceState.CameraTestsResultsScreen;


                        await CleanupCameraAsync();
                    }
                    break;
                default: // Do nothing in the other states
                    break;
            }
        }

        private async Task StartPreviewAsync()
        {
            try
            {
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                if (devices.Count == 0)
                {
                    StartAutomaticTestsButton.Visibility = Visibility.Collapsed;
                    return;
                }
                StartAutomaticTestsButton.Visibility = Visibility.Visible;
                _currentCameraNumber %= devices.Count;

                if (devices.Count > 1)
                {
                    CameraToggleTextBox.Text = "Using camera " + (_currentCameraNumber + 1) + " of " + devices.Count;
                    CameraToggleTextBox.Visibility = Visibility.Visible;
                    ChangeCameraButton.Visibility = Visibility.Visible;
                }

                _mediaCapture = new MediaCapture();

                var cameraDevice = devices[_currentCameraNumber];
                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };
                await _mediaCapture.InitializeAsync(settings);

                _displayRequest.RequestActive();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException)
            {
                StartAutomaticTestsButton.Visibility = Visibility.Collapsed;
                // This will be thrown if the user denied access to the camera and/or the microphone in privacy settings
                DistanceFromCameraTextBox.Text = "The app was denied access to the camera and/or the microphone";
                Debug.WriteLine("The app was denied access to the camera and/or the microphone");
                return;
            }

            try
            {
                PreviewControl.Source = _mediaCapture;

                await _mediaCapture.StartPreviewAsync();
                _previewProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
                _isPreviewing = true;

                await CreateFaceDetectionEffectAsync();
            }
            catch
            {
                StartAutomaticTestsButton.Visibility = Visibility.Collapsed;
                DistanceFromCameraTextBox.Text = "Another app is using the current camera. Please use a different camera or close the application and try again.";
            }

        }

        private async Task CleanupCameraAsync()
        {
            try
            {
                if (_mediaCapture != null)
                {
                    if (_isPreviewing)
                    {
                        await _mediaCapture.StopPreviewAsync();
                    }

                    if (_faceDetectionEffect != null)
                    {
                        await CleanUpFaceDetectionEffectAsync();
                    }

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        PreviewControl.Source = null;
                        if (_displayRequest != null)
                        {
                            _displayRequest.RequestRelease();
                        }

                        _mediaCapture.Dispose();
                        _mediaCapture = null;
                    });
                }
                FacesCanvas.Children.Clear();
            }
            catch
            {
                StartAutomaticTestsButton.Visibility = Visibility.Collapsed;
                DistanceFromCameraTextBox.Text = "Another app is using the current camera. Please use a different camera or close the application and try again.";
            }
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanupCameraAsync();
                deferral.Complete();
            }
        }

        private async void ProximitySensorReadingChangedAsync(object sender, ProximitySensorReadingChangedEventArgs e)
        {
            _lastProximitySensorReading = e.Reading;
            _proximitySensorReadingBuffer.Enqueue(e.Reading);
            if ((_currentState == DistanceState.TapeMeasureTargetDistanceScreen) && !e.Reading.IsDetected)
            {
                _proximitySensorReadingBuffer.Clear();
            }
            else if (e.Reading.IsDetected)
            {
                if ((_currentState == DistanceState.TapeMeasureTargetDistanceScreen) || (_currentState == DistanceState.TapeMeasureSingleTestPassScreen) || (_currentState == DistanceState.TapeMeasureSingleTestFailedScreen))
                {
                    _manualProximityDistances.Add((int)e.Reading.DistanceInMillimeters);
                    _manualProximityTimestamps.Add(e.Reading.Timestamp);
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    DistanceFromSensorTextBox.Text = "Distance from Presence Sensor: " + e.Reading.DistanceInMillimeters + " mm";
                    CurrentDistanceFromSensorTextBox.Text = "Distance from Presence Sensor: " + e.Reading.DistanceInMillimeters + " mm";
                    Debug.WriteLine("Sensor: " + e.Reading.Timestamp);
                });
            }
        }

        int GetCurrentTimeout()
        {
            int errorPercent = _proximityDefaultCountdownTimeSec;
            bool parsedSuccess = Int32.TryParse(TestTimeLengthInSecondsTextBox.Text, out errorPercent);
            if (!parsedSuccess)
            {
                errorPercent = _proximityDefaultCountdownTimeSec;
            }
            return errorPercent;
        }

        bool IsHumanPresenceSensor()
        {
            try
            {
                return (Sensor.ProximitySensorDeviceInfo[_currentSensorNumber].Properties[Constants.Properties["DEVPKEY_Sensor_ProximityType"]].ToString() == "1");
            }
            catch
            {
                return false;
            }
        }

        string GetSensorName()
        {
            try
            {
                return Sensor.ProximitySensorDeviceInfo[_currentSensorNumber].Properties[Constants.Properties["Sensor_Name"]].ToString();
            }
            catch
            {
                return "No sensor name set";
            }
        }

        #region Button callbacks

        private void ConfirmInstructions_Click(object sender, RoutedEventArgs e)
        {
            StartPanel1.Visibility = Visibility.Collapsed;
            StartPanel2.Visibility = Visibility.Visible;
        }

        private async void RunAutomaticTestsButton_Click(object sender, RoutedEventArgs e)
        {
            _currentState = DistanceState.CameraTestsInstructionScreen;
            StartPanel2.Visibility = Visibility.Collapsed;
            AutoPanel.Visibility = Visibility.Visible;
            PreviewControl.Visibility = Visibility.Visible;
            FacesCanvas.Visibility = Visibility.Visible;
            EscapeAutomaticTestsButton.Visibility = Visibility.Collapsed;
            AutoMaxDistanceTextBox.Visibility = Visibility.Visible;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await StartPreviewAsync();
            });

            _proximitySensorReadingBuffer.Clear();
            _proximitySensor.ReadingChanged += ProximitySensorReadingChangedAsync;
        }

        private void RunManualTestsButton_Click(object sender, RoutedEventArgs e)
        {
            _currentState = DistanceState.TapeMeasureTestsInstructionScreen;
            StartPanel2.Visibility = Visibility.Collapsed;
            ManualPanel.Visibility = Visibility.Visible;
        }

        private void ChangeProximitySensor_Click(object sender, RoutedEventArgs e)
        {
            _currentSensorNumber = (_currentSensorNumber + 1) % Sensor.ProximitySensorList.Count;

            CurrentProximitySensorName.Text = GetSensorName();

            ProximitySensorFoundTextBox.Text = "Using proximity sensor " + (_currentSensorNumber + 1) + " of " + Sensor.ProximitySensorList.Count;
            if (IsHumanPresenceSensor())
            {
                ProximitySensorFoundTextBox.Text = "Using proximity sensor (human presence) " + (_currentSensorNumber + 1) + " of " + Sensor.ProximitySensorList.Count;
            }
 
            _proximitySensor = Sensor.ProximitySensorList[_currentSensorNumber];
        }

        private void StartManualTestsButton_Click(object sender, RoutedEventArgs e)
        {
            _currentState = DistanceState.TapeMeasureTargetDistanceScreen;
            ManualLongTextTextBox.Visibility = Visibility.Collapsed;
            ProximityImageManual.Visibility = Visibility.Collapsed;
            ManualCustomConfigOptionsTextBox.Visibility = Visibility.Collapsed;
            ManualErrorMarginPercentTextBox.Visibility = Visibility.Collapsed;
            TestTimeLengthInSecondsTextBox.Visibility = Visibility.Collapsed;
            StartManualTestsButton.Visibility = Visibility.Collapsed;
            ManualGoToDistanceTextBox.Visibility = Visibility.Visible;
            CurrentTimerTextBox.Visibility = Visibility.Visible;
            _currentCountdown = GetCurrentTimeout();
            CurrentTimerTextBox.Text = "Timer: " + _currentCountdown + " seconds";
            ManualDistanceTextBox.Visibility = Visibility.Visible;
            CurrentDistanceFromSensorTextBox.Visibility = Visibility.Visible;

            _manualProximityDistances.Clear();
            _manualProximityTimestamps.Clear();
            _manualTargetDistances.Clear();
            _manualTargetTimestamps.Clear();

            _proximitySensorReadingBuffer.Clear();
            _proximitySensor.ReadingChanged += ProximitySensorReadingChangedAsync;
        }

        private void CompleteManualTestsButton_Click(object sender, RoutedEventArgs e)
        {
            _currentState = DistanceState.TestSelectionScreen;
            ManualResultsPanel.Visibility = Visibility.Collapsed;
            StartPanel1.Visibility = Visibility.Visible;

            _proximitySensor.ReadingChanged -= ProximitySensorReadingChangedAsync;
        }

        private void StartAutomaticTestsButton_Click(object sender, RoutedEventArgs e)
        {
            _proximitySensorReadingBuffer.Clear();
            _cameraReadingDistancesMm.Clear();
            _cameraReadingTimeStamps.Clear();
            _currentState = DistanceState.CameraTestsMoveBackScreen;
            AutomaticLongTextTextBox.Visibility = Visibility.Collapsed;
            AutoCustomConfigOptionsTextBox.Visibility = Visibility.Collapsed;
            AutoErrorMarginPercentTextBox.Visibility = Visibility.Collapsed;
            AutoMaxDistanceTextBox.Visibility = Visibility.Collapsed;
            StartAutomaticTestsButton.Visibility = Visibility.Collapsed;
            MoveDirectionTextBox.Text = "Move backwards!";
            MoveDirectionTextBox.Foreground = new SolidColorBrush(Colors.DarkBlue);
            MoveDirectionTextBox.Visibility = Visibility.Visible;
            EscapeAutomaticTestsButton.Visibility = Visibility.Visible;
            ChangeCameraButton.Visibility = Visibility.Collapsed;
            CameraToggleTextBox.Visibility = Visibility.Collapsed;
        }

        private void EscapeAutomaticTestsButton_Click(object sender, RoutedEventArgs e)
        {
            _proximitySensorReadingBuffer.Clear();
            _cameraReadingDistancesMm.Clear();
            _cameraReadingTimeStamps.Clear();
            _currentState = DistanceState.CameraTestsInstructionScreen;
            AutomaticLongTextTextBox.Visibility = Visibility.Visible;
            AutoCustomConfigOptionsTextBox.Visibility = Visibility.Visible;
            AutoErrorMarginPercentTextBox.Visibility = Visibility.Visible;
            AutoMaxDistanceTextBox.Visibility = Visibility.Visible;
            StartAutomaticTestsButton.Visibility = Visibility.Visible;
            MoveDirectionTextBox.Visibility = Visibility.Collapsed;
            ChangeCameraButton.Visibility = Visibility.Visible;
            CameraToggleTextBox.Visibility = Visibility.Visible;

            StartPanel2.Visibility = Visibility.Collapsed;
            AutoPanel.Visibility = Visibility.Visible;
            PreviewControl.Visibility = Visibility.Visible;
            FacesCanvas.Visibility = Visibility.Visible;
            EscapeAutomaticTestsButton.Visibility = Visibility.Collapsed;

        }

        private void CompleteAutomaticTestsButton_Click(object sender, RoutedEventArgs e)
        {
            _currentState = DistanceState.TestSelectionScreen;
            AutoResultsPanel.Visibility = Visibility.Collapsed;
            StartPanel1.Visibility = Visibility.Visible;

            _proximitySensor.ReadingChanged -= ProximitySensorReadingChangedAsync;
        }

        private async void ChangeCamera_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await CleanupCameraAsync();

                _currentCameraNumber++;
                await StartPreviewAsync();
            });
        }

        private void AutoErrorMarginPercent_TextChanged(object sender, TextChangedEventArgs e)
        {
            int errorPercent;
            bool parsedSuccess = Int32.TryParse(AutoErrorMarginPercentTextBox.Text, out errorPercent);
            if ((!parsedSuccess) || (errorPercent > 100) || (errorPercent < 0))
            {
                AutoErrorMarginPercentTextBox.Text = "";
            }
        }

        private void AutoMaxDistance_TextChanged(object sender, TextChangedEventArgs e)
        {
            int maxDistance;
            bool parsedSuccess = Int32.TryParse(AutoMaxDistanceTextBox.Text, out maxDistance);
            if ((!parsedSuccess) || (maxDistance > 2000) || (maxDistance < 0))
            {
                AutoMaxDistanceTextBox.Text = "";
            } else
            {
                if (maxDistance >= _minDistance)
                {
                    _stopMovingBackwardsThresholdMm = maxDistance - _minDistance;
                    _maxDistancePlotMm = maxDistance;
                    _closeToMidDistanceCutoffMm = maxDistance / 2;
                    _midToFarDistanceCutoffMm = maxDistance - _minDistance;
                }
            }
        }

        private void ManualErrorMarginPercent_TextChanged(object sender, TextChangedEventArgs e)
        {
            int errorPercent;
            bool parsedSuccess = Int32.TryParse(ManualErrorMarginPercentTextBox.Text, out errorPercent);
            if ((!parsedSuccess) || (errorPercent > 100) || (errorPercent < 0))
            {
                ManualErrorMarginPercentTextBox.Text = "";
            }
        }

        private void TestTimeLengthInSeconds_TextChanged(object sender, TextChangedEventArgs e)
        {
            int timeoutTimeInSeconds;
            bool parsedSuccess = Int32.TryParse(TestTimeLengthInSecondsTextBox.Text, out timeoutTimeInSeconds);
            if ((!parsedSuccess) || (timeoutTimeInSeconds < 0))
            {
                TestTimeLengthInSecondsTextBox.Text = "";
            }
        }

        #endregion


        #region Face detection helpers

        /// <summary>
        /// Adds face detection to the preview stream, registers for its events, enables it, and gets the FaceDetectionEffect instance
        /// </summary>
        /// <returns></returns>
        private async Task CreateFaceDetectionEffectAsync()
        {
            // Create the definition, which will contain some initialization settings
            var definition = new FaceDetectionEffectDefinition();

            // To ensure preview smoothness, do not delay incoming samples
            definition.SynchronousDetectionEnabled = false;

            // In this scenario, choose detection speed over accuracy
            definition.DetectionMode = FaceDetectionMode.HighPerformance;

            // Add the effect to the preview stream
            _faceDetectionEffect = (FaceDetectionEffect)await _mediaCapture.AddVideoEffectAsync(definition, MediaStreamType.VideoPreview);

            // Register for face detection events
            _faceDetectionEffect.FaceDetected += FaceDetectionEffect_FaceDetected;

            // Choose the shortest interval between detection events
            _faceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);

            // Start detecting faces
            _faceDetectionEffect.Enabled = true;
        }

        /// <summary>
        ///  Disables and removes the face detection effect, and unregisters the event handler for face detection
        /// </summary>
        /// <returns></returns>
        private async Task CleanUpFaceDetectionEffectAsync()
        {
            // Disable detection
            _faceDetectionEffect.Enabled = false;

            // Unregister the event handler
            _faceDetectionEffect.FaceDetected -= FaceDetectionEffect_FaceDetected;

            if (_mediaCapture != null)
            {
                // Remove the effect (see ClearEffectsAsync method to remove all effects from a stream)
                await _mediaCapture.RemoveEffectAsync(_faceDetectionEffect);
            }

            // Clear the member variable that held the effect instance
            _faceDetectionEffect = null;
        }

        private async void FaceDetectionEffect_FaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            //Debug.WriteLine(DateTime.Now);
            // Ask the UI thread to render the face bounding boxes
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HighlightDetectedFaces(args.ResultFrame.DetectedFaces));
        }

        /// <summary>
        /// Iterates over all detected faces, creating and adding Rectangles to the FacesCanvas as face bounding boxes
        /// </summary>
        /// <param name="faces">The list of detected faces from the FaceDetected event of the effect</param>
        private void HighlightDetectedFaces(IReadOnlyList<DetectedFace> faces)
        {
            // Remove any existing rectangles from previous events
            FacesCanvas.Children.Clear();

            double maxHeight = 0;

            // For each detected face
            for (int i = 0; i < faces.Count; i++)
            {
                var previewStream = _previewProperties as VideoEncodingProperties;

                // Face coordinate units are preview resolution pixels, which can be a different scale from our display resolution, so a conversion may be necessary
                Rectangle faceBoundingBox = ConvertPreviewToUiRectangle(faces[i].FaceBox);

                if (faces[i].FaceBox.Height > maxHeight)
                {
                    maxHeight = faces[i].FaceBox.Height;
                }

                // Set bounding box stroke properties
                faceBoundingBox.StrokeThickness = 2;

                // Highlight the first face in the set
                faceBoundingBox.Stroke = (i == 0 ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.DeepSkyBlue));

                // Add grid to canvas containing all face UI objects
                FacesCanvas.Children.Add(faceBoundingBox);
            }

            if (maxHeight != 0)
            {
                var previewStream = _previewProperties as VideoEncodingProperties;
                if (previewStream != null)
                {
                    DateTimeOffset timeNow = DateTimeOffset.Now;
                    Debug.WriteLine("Camera: " + timeNow);
                    int cameraDistanceMm = (int)(_distanceScalerMultiplier * (previewStream.Height + _distanceScalerPixelsAdder) / maxHeight);
                    DistanceFromCameraTextBox.Text = "Distance from Camera: " + (cameraDistanceMm).ToString() + " mm";

                    if ((_currentState == DistanceState.CameraTestsMoveForwardScreen) || (_currentState == DistanceState.CameraTestsMoveBackScreen))
                    {
                        _cameraReadingDistancesMm.Enqueue(cameraDistanceMm);
                        _cameraReadingTimeStamps.Enqueue(timeNow);
                        _lastCameraDistanceMm = cameraDistanceMm;
                        _lastCameraReadingTimeStamp = timeNow;
                    }
                }
            }

            var previewArea = GetPreviewStreamRectInControl(_previewProperties as VideoEncodingProperties, PreviewControl);
            FacesCanvas.Width = previewArea.Width;
            FacesCanvas.Height = previewArea.Height;

            Canvas.SetLeft(FacesCanvas, previewArea.X);
            Canvas.SetTop(FacesCanvas, previewArea.Y);
        }

        /// <summary>
        /// Takes face information defined in preview coordinates and returns one in UI coordinates, taking
        /// into account the position and size of the preview control.
        /// </summary>
        /// <param name="faceBoxInPreviewCoordinates">Face coordinates as retried from the FaceBox property of a DetectedFace, in preview coordinates.</param>
        /// <returns>Rectangle in UI (CaptureElement) coordinates, to be used in a Canvas control.</returns>
        private Rectangle ConvertPreviewToUiRectangle(BitmapBounds faceBoxInPreviewCoordinates)
        {
            var result = new Rectangle();
            var previewStream = _previewProperties as VideoEncodingProperties;

            // If there is no available information about the preview, return an empty rectangle, as re-scaling to the screen coordinates will be impossible
            if (previewStream == null) return result;

            // Similarly, if any of the dimensions is zero (which would only happen in an error case) return an empty rectangle
            if (previewStream.Width == 0 || previewStream.Height == 0) return result;

            double streamWidth = previewStream.Width;
            double streamHeight = previewStream.Height;

            // Get the rectangle that is occupied by the actual video feed
            var previewInUI = GetPreviewStreamRectInControl(previewStream, PreviewControl);

            // Scale the width and height from preview stream coordinates to window coordinates
            result.Width = (faceBoxInPreviewCoordinates.Width / streamWidth) * previewInUI.Width;
            result.Height = (faceBoxInPreviewCoordinates.Height / streamHeight) * previewInUI.Height;

            // Scale the X and Y coordinates from preview stream coordinates to window coordinates
            var x = (faceBoxInPreviewCoordinates.X / streamWidth) * previewInUI.Width;
            var y = (faceBoxInPreviewCoordinates.Y / streamHeight) * previewInUI.Height;
            Canvas.SetLeft(result, x);
            Canvas.SetTop(result, y);

            return result;
        }

        /// <summary>
        /// Calculates the size and location of the rectangle that contains the preview stream within the preview control, when the scaling mode is Uniform
        /// </summary>
        /// <param name="previewResolution">The resolution at which the preview is running</param>
        /// <param name="previewControl">The control that is displaying the preview using Uniform as the scaling mode</param>
        /// <returns></returns>
        public Rect GetPreviewStreamRectInControl(VideoEncodingProperties previewResolution, CaptureElement previewControl)
        {
            var result = new Rect();

            // In case this function is called before everything is initialized correctly, return an empty result
            if (previewControl == null || previewControl.ActualHeight < 1 || previewControl.ActualWidth < 1 ||
                previewResolution == null || previewResolution.Height == 0 || previewResolution.Width == 0)
            {
                return result;
            }

            var streamWidth = previewResolution.Width;
            var streamHeight = previewResolution.Height;

            // Start by assuming the preview display area in the control spans the entire width and height both (this is corrected in the next if for the necessary dimension)
            result.Width = previewControl.ActualWidth;
            result.Height = previewControl.ActualHeight;

            // If UI is "wider" than preview, letterboxing will be on the sides
            if ((previewControl.ActualWidth / previewControl.ActualHeight > streamWidth / (double)streamHeight))
            {
                var scale = previewControl.ActualHeight / streamHeight;
                var scaledWidth = streamWidth * scale;

                result.X = (previewControl.ActualWidth - scaledWidth) / 2.0;
                result.Width = scaledWidth;
            }
            else // Preview stream is "wider" than UI, so letterboxing will be on the top+bottom
            {
                var scale = previewControl.ActualWidth / streamWidth;
                var scaledHeight = streamHeight * scale;

                result.Y = (previewControl.ActualHeight - scaledHeight) / 2.0;
                result.Height = scaledHeight;
            }

            return result;
        }

        #endregion
    }
}