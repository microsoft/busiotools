// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

using OpenCvSharp;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using System.Linq;
using Windows.Media.MediaProperties;
using System.Threading;
using Windows.Devices.Sensors;
using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace SensorExplorer
{
    public sealed partial class Scenario4Distance : Page
    {
        [ComImport]
        [Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* value, out uint capacity);
        }

        public static Scenario4Distance Scenario4;

        private int m_numberOfGoodChessboardSamples = 0;
        private double[,] m_cameraMatrix;
        private double[] m_distCoeffs;
        private bool m_isLastChessboardSampleGood = false;
        private List<List<Point3f>> m_objectPoints = new List<List<Point3f>>();
        private List<List<Point2f>> m_imagePoints = new List<List<Point2f>>();
        private int m_distanceMm = 0;
        private MediaCapture m_mediaCapture = null;
        private MediaFrameReader m_reader = null;

        private const int m_imageNumRows = 720;
        private const int m_imageNumCols = 1280;

        private ProximitySensor m_sensor;
        private DeviceWatcher m_watcher;
        private int m_numberOfGoodProximitySamples = 0;

        MainPage rootPage = MainPage.Current;

        public Scenario4Distance()
        {
            InitializeComponent();
            Scenario4 = this;

            InitMediaCapture();

            m_watcher = DeviceInformation.CreateWatcher(ProximitySensor.GetDeviceSelector());
            m_watcher.Added += OnProximitySensorAdded;
            m_watcher.Start();
        }

        public async void InitMediaCapture()
        {
            var allGroups = await MediaFrameSourceGroup.FindAllAsync();
            var sourceGroups = allGroups.Select(g => new
            {
                Group = g,
                SourceInfo = g.SourceInfos.FirstOrDefault(i => i.SourceKind == MediaFrameSourceKind.Color)
            }).Where(g => g.SourceInfo != null).ToList();

            if (sourceGroups.Count == 0)
            {
                return;
            }
            var selectedSource = sourceGroups.FirstOrDefault();

            m_mediaCapture = null;
            m_mediaCapture = new MediaCapture();

            var settings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = selectedSource.Group,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                StreamingCaptureMode = StreamingCaptureMode.Video,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu
            };

            // Set the MediaCapture to a variable in App.xaml.cs to handle suspension.
            (App.Current as App).MediaCapture = m_mediaCapture;

            await m_mediaCapture.InitializeAsync(settings);

            MediaFrameSource frameSource = m_mediaCapture.FrameSources[selectedSource.SourceInfo.Id];
            BitmapSize size = new BitmapSize()
            {
                Height = m_imageNumRows,
                Width = m_imageNumCols
            };
            m_reader = await m_mediaCapture.CreateFrameReaderAsync(frameSource, MediaEncodingSubtypes.Bgra8, size);
            m_reader.FrameArrived += ColorFrameReader_FrameArrivedAsync;
            await m_reader.StartAsync();
        }

        private async void ColorFrameReader_FrameArrivedAsync(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            var frame = sender.TryAcquireLatestFrame();

            if (frame != null)
            {
                var inputBitmap = frame.VideoMediaFrame?.SoftwareBitmap;

                if (inputBitmap != null)
                {
                    SoftwareBitmap originalBitmap = SoftwareBitmap.Convert(inputBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                    await CheckerboardDetectorAsync(originalBitmap);
                }
            }
        }

        public async System.Threading.Tasks.Task CheckerboardDetectorAsync(SoftwareBitmap input)
        {
            Mat mInput = SoftwareBitmapToMat(input);
            Mat gray = mInput.CvtColor(ColorConversionCodes.BGRA2GRAY);
            List<Point2f> corners = new List<Point2f>();
            bool foundCorners = Cv2.FindChessboardCorners(gray, new OpenCvSharp.Size(9, 7), OutputArray.Create(corners));
            TermCriteria criteria = TermCriteria.Both(30, 0.0001);

            if (foundCorners)
            {
                Cv2.CornerSubPix(gray, corners, new OpenCvSharp.Size(11, 11), new OpenCvSharp.Size(-1, -1), criteria);

                bool foundBoard = false;
                Cv2.DrawChessboardCorners(mInput, new OpenCvSharp.Size(9, 7), corners, foundBoard);

                m_numberOfGoodChessboardSamples++;

                List<Point3f> objLocation = new List<Point3f>();
                for (int i = 0; i < 7; ++i)
                {
                    for (int j = 0; j < 9; ++j)
                    {
                        objLocation.Add(new Point3f(j * 20, i * 20, 0));
                    }
                }

                if (m_numberOfGoodChessboardSamples <= 20)
                {
                    m_objectPoints.Add(objLocation);
                    m_imagePoints.Add(corners);
                }

                if (m_numberOfGoodChessboardSamples == 20)
                {
                    double[,] cameraMatrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
                    double[] distCoeffs = new double[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    Vec3d[] rvecs = { };
                    Vec3d[] tvecs = { };

                    Cv2.CalibrateCamera(m_objectPoints, m_imagePoints, new OpenCvSharp.Size(1280, 720), cameraMatrix, distCoeffs, out rvecs, out tvecs);

                    m_cameraMatrix = cameraMatrix;
                    m_distCoeffs = distCoeffs;
                }

                if (m_numberOfGoodChessboardSamples >= 20)
                {
                    double[] rvecs2 = { };
                    double[] tvecs2 = { };

                    Cv2.SolvePnP(objLocation, corners, m_cameraMatrix, m_distCoeffs, ref rvecs2, ref tvecs2);
                    m_distanceMm = (int)Math.Round(Math.Sqrt(tvecs2[0] * tvecs2[0] + tvecs2[1] * tvecs2[1] + tvecs2[2] * tvecs2[2]));
                }
            }

            m_isLastChessboardSampleGood = foundCorners;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.IsDetectChessboard.Text = "Object is " + (m_isLastChessboardSampleGood ? "detected" : "not detected") + " with the camera";
                this.NumSamplesChessboard.Text = "Number of detected chessboards: " + m_numberOfGoodChessboardSamples;
                if (m_numberOfGoodChessboardSamples < 20)
                {
                    this.DistanceFromChessboard.Text = "Calibrating ... Wait for 20 complete samples";
                }
                else
                {
                    this.DistanceFromChessboard.Text = "Distance from camera: " + m_distanceMm + " mm";
                }
            });
        }

        public static unsafe Mat SoftwareBitmapToMat(SoftwareBitmap softwareBitmap)
        {
            BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Write);
            var reference = buffer.CreateReference();
            ((IMemoryBufferByteAccess)reference).GetBuffer(out var dataInBytes, out var capacity);

            Mat outputMat = new Mat(softwareBitmap.PixelHeight, softwareBitmap.PixelWidth, MatType.CV_8UC4, (IntPtr)dataInBytes);
            return outputMat;
        }

        public static unsafe void MatToSoftwareBitmap(Mat input, SoftwareBitmap output)
        {
            BitmapBuffer buffer = output.LockBuffer(BitmapBufferAccessMode.ReadWrite);
            var reference = buffer.CreateReference();
            ((IMemoryBufferByteAccess)reference).GetBuffer(out var dataInBytes, out var capacity);
            BitmapPlaneDescription layout = buffer.GetPlaneDescription(0);

            for (int i = 0; i < layout.Height; i++)
            {
                for (int j = 0; j < layout.Width; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        dataInBytes[layout.StartIndex + layout.Stride * i + 4 * j + k] =
                        input.DataPointer[layout.StartIndex + layout.Stride * i + 4 * j + k];
                    }
                }
            }
        }

        private async void OnProximitySensorAdded(DeviceWatcher sender, DeviceInformation device)
        {
            if (null == m_sensor)
            {
                ProximitySensor foundSensor = ProximitySensor.FromId(device.Id);
                if (null != foundSensor)
                {
                    if (null == foundSensor.MaxDistanceInMillimeters)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            rootPage.NotifyUser("Proximity sensor does not report detection ranges, using it anyway", NotifyType.StatusMessage);
                        });
                    }

                    m_sensor = foundSensor;
                    m_sensor.ReadingChanged += new TypedEventHandler<ProximitySensor, ProximitySensorReadingChangedEventArgs>(ReadingChanged);
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        rootPage.NotifyUser("Could not get a proximity sensor from the device id", NotifyType.ErrorMessage);
                    });
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (null != m_sensor)
            {
                m_sensor.ReadingChanged -= new TypedEventHandler<ProximitySensor, ProximitySensorReadingChangedEventArgs>(ReadingChanged);
            }
            base.OnNavigatingFrom(e);
        }

        async private void ReadingChanged(ProximitySensor sender, ProximitySensorReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ProximitySensorReading reading = e.Reading;
                if (null != reading)
                {
                    m_numberOfGoodProximitySamples++;

                    IsDetectedObject.Text = "Object is " + (reading.IsDetected ? "detected" : "not detected") + " with the proximity sensor";
                    NumSamplesProximity.Text = "Number of good detections with proximity sensor: " + m_numberOfGoodProximitySamples;

                    // Show the detection distance, if available
                    if (null != reading.DistanceInMillimeters)
                    {
                        DistanceWithProximitySensor.Text = "Distance with the proximity sensor: " + reading.DistanceInMillimeters.ToString() + " mm";
                    }
                }
            });
        }
    }
}