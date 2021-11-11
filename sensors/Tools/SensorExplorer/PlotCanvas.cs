// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace SensorExplorer
{
    class PlotCanvas
    {
        private bool replotGrid = true;
        private Canvas canvas;
        private Color[] color;
        private double fontSize = 11;
        private double height = 100;
        private int frame = 10;
        private int maxValue = 0;
        private int minValue = 0;
        private List<int> plotIndex = new List<int>();
        private string[] vAxisLabel;
        private bool isOneShotGraph = false;
        public PlotCanvas(int minValue, int maxValue, Color[] color, Canvas canvas, string[] vAxisLabel)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.color = color;
            this.canvas = canvas;
            this.vAxisLabel = vAxisLabel;

            for (int i = 0; i < color.Length; i++)
            {
                if (color[i] != Colors.Black)
                {
                    plotIndex.Add(i);
                }
            }
        }

        public void HideCanvas()
        {
            canvas.Visibility = Visibility.Collapsed;
        }

        public void ShowCanvas()
        {
            canvas.Visibility = Visibility.Visible;
        }

        public void SetHeight(double height)
        {
            this.height = height;
            replotGrid = true;
        }

        public void SetFontSize(double fontSize)
        {
            this.fontSize = fontSize;
            replotGrid = true;
        }

        public void SetRange(int maxValue, int minValue)
        {
            for (int i = 0; i < vAxisLabel.Length; i++)
            {
                vAxisLabel[i] = (maxValue - Convert.ToDouble(i) / (vAxisLabel.Length - 1) * (maxValue - minValue)).ToString();
            }

            this.maxValue = maxValue;
            this.minValue = minValue;
            replotGrid = true;
        }

        public void Plot(SensorData sensorData)
        {
            try
            {
                if (replotGrid)
                {
                    canvas.Children.Clear();
                    canvas.Height = height;

                    PlotGrid(maxValue, minValue, Colors.Black);

                    replotGrid = false;
                }
                else
                {
                    int index = canvas.Children.Count;

                    for (int i = 0; i < plotIndex.Count; i++)
                    {
                        index--;
                        canvas.Children.RemoveAt(index);
                    }
                }

                PathFigure[] pathFigure = new PathFigure[plotIndex.Count];

                for (int i = 0; i < pathFigure.Length; i++)
                {
                    pathFigure[i] = new PathFigure();
                }

                DateTime now = DateTime.UtcNow;

                for (int i = sensorData.Readings.Count - 1; i >= 0; i--)
                {
                    if (sensorData.Readings[i].Timestamp < now)
                    {
                        if (i >= 0)
                        {
                            for (int j = 0; j < plotIndex.Count; j++)
                            {
                                int index = plotIndex[j];
                                AddLineSegmentToPathFigure(ref pathFigure[index], 0, sensorData.Readings[i].Value[index], (sensorData.Readings[i].Timestamp - now).TotalSeconds, sensorData.Readings[i].Value[index]);
                            }
                        }

                        for (; i > 0 && sensorData.Readings[i].Timestamp > now.AddSeconds(-frame); i--)
                        {
                            for (int j = 0; j < plotIndex.Count; j++)
                            {
                                int index = plotIndex[j];
                                AddLineSegmentToPathFigure(ref pathFigure[index], (sensorData.Readings[i].Timestamp - now).TotalSeconds, sensorData.Readings[i - 1].Value[index], (sensorData.Readings[i - 1].Timestamp - now).TotalSeconds, sensorData.Readings[i - 1].Value[index]);
                            }
                        }

                        break;
                    }
                }

                Path[] path = AddPathFigureToPath(pathFigure);

                for (int i = 0; i < path.Length; i++)
                {
                    canvas.Children.Add(path[i]);
                }
            }
            catch { }
        }

        private void AddLineSegmentToPathFigure(ref PathFigure pathFigure, double currTimestamp, double currValue, double prevTimestamp, double prevValue)
        {
            try
            {
                LineSegment lineSegment;

                double currX = Math.Max(Math.Min(canvas.Width * -currTimestamp / frame, canvas.Width), 0);
                double currY = Math.Max(Math.Min(canvas.Height * (maxValue - currValue) / (maxValue - minValue), canvas.Height), 0);

                lineSegment = new LineSegment();
                lineSegment.Point = new Point(currX, currY);
                pathFigure.Segments.Add(lineSegment);

                double prevX = Math.Max(Math.Min(canvas.Width * -prevTimestamp / frame, canvas.Width), 0);
                double prevY = Math.Max(Math.Min(canvas.Height * (maxValue - prevValue) / (maxValue - minValue), canvas.Height), 0);

                lineSegment = new LineSegment();
                lineSegment.Point = new Point(prevX, prevY);
                pathFigure.Segments.Add(lineSegment);
            }
            catch { }
        }

        private Path[] AddPathFigureToPath(PathFigure[] pathFigure)
        {
            Path[] path = new Path[pathFigure.Length];

            try
            {
                for (int i = 0; i < pathFigure.Length; i++)
                {
                    if (pathFigure[i].Segments.Count > 0)
                    {
                        pathFigure[i].StartPoint = ((LineSegment)pathFigure[i].Segments[0]).Point;
                    }

                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures.Add(pathFigure[i]);

                    path[i] = new Path() { Data = pathGeometry, Stroke = new SolidColorBrush(color[i]), StrokeThickness = 2 };
                }
            }
            catch { }

            return path;
        }

        private void PlotGrid(int maxValue, int minValue, Color color)
        {
            try
            {
                for (int i = 0; i <= frame; i++)
                {
                    Line vLine = new Line();

                    vLine.X1 = i * canvas.Width / frame;
                    vLine.Y1 = 0;
                    vLine.X2 = vLine.X1;
                    vLine.Y2 = canvas.Height;

                    vLine.Stroke = new SolidColorBrush(color);

                    if (i == 0)
                    {
                        vLine.StrokeThickness = 2;
                    }
                    else
                    {
                        vLine.StrokeThickness = 1;
                    }

                    // Add the path to the Canvas
                    canvas.Children.Add(vLine);

                    if (frame < 15 || i % 5 == 0 || i == frame)
                    {
                        TextBlock textBlock = new TextBlock() { Text = (-i).ToString(), FontSize = fontSize };
                        if (isOneShotGraph)
                        {
                            if (i == 0)
                            {
                                textBlock.Text = "Start";
                            }
                            else if (i == frame)
                            {
                                textBlock.Text = "End";
                            }
                            else
                            {
                                textBlock.Text = i.ToString();
                            }
                        }
                        textBlock.Measure(new Size(200, 200)); // Assuming 200x200 is max size of textblock
                        textBlock.Width = textBlock.DesiredSize.Width;
                        textBlock.Height = textBlock.DesiredSize.Height;
                        textBlock.Margin = new Thickness(vLine.X2 - textBlock.Width / 2, vLine.Y2 + 5, 0, 0);
                        canvas.Children.Add(textBlock);
                    }
                }

                for (int i = 0; i < vAxisLabel.Length; i++)
                {
                    Line hLine = new Line();

                    hLine.X1 = 0;
                    hLine.Y1 = i * canvas.Height / (vAxisLabel.Length - 1);
                    hLine.X2 = canvas.Width;
                    hLine.Y2 = hLine.Y1;

                    hLine.Stroke = new SolidColorBrush(color);

                    if (i < vAxisLabel.Length - 1)
                    {
                        hLine.StrokeThickness = 1;
                    }
                    else
                    {
                        hLine.StrokeThickness = 2;
                    }

                    // Add the path to the Canvas
                    canvas.Children.Add(hLine);

                    TextBlock textBlock = new TextBlock() { Text = vAxisLabel[i], FontSize = fontSize };
                    textBlock.Measure(new Size(200, 200)); // Assuming 200x200 is max size of textblock
                    textBlock.Width = textBlock.DesiredSize.Width;
                    textBlock.Height = textBlock.DesiredSize.Height;
                    textBlock.Margin = new Thickness(-textBlock.Width - 5, hLine.Y1 - textBlock.Height / 2, 0, 0);
                    canvas.Children.Add(textBlock);
                }
            }
            catch { }
        }

        public void PlotGroup(List<List<int>> distances, List<List<DateTimeOffset>> timestamps)
        {
            DateTimeOffset startTime = timestamps[0][0];
            DateTimeOffset endTime = timestamps[0][timestamps[0].Count - 1];
            DateTimeOffset timeNow = DateTimeOffset.Now;
            isOneShotGraph = true;
            for (int i = 1; i < timestamps.Count; i++)
            {
                if (timestamps[i][0] < startTime)
                {
                    startTime = timestamps[i][0];
                }
                if (timestamps[i][timestamps[i].Count - 1] > endTime)
                {
                    endTime = timestamps[i][timestamps[i].Count - 1];
                }
            }
            try
            {
                canvas.Children.Clear();
                canvas.Height = height;

                frame = (int)timeNow.Subtract(startTime).TotalSeconds;

                PlotGrid(maxValue, minValue, Colors.Black);

                PathFigure[] pathFigure = new PathFigure[plotIndex.Count];

                for (int i = 0; i < pathFigure.Length; i++)
                {
                    pathFigure[i] = new PathFigure();
                }

                for (int i = 0; i < timestamps.Count; i++)
                {
                    for (int j = 1; j < distances[i].Count; j++)
                    {
                        if (timestamps[i][j] < timeNow)
                        {
                            AddLineSegmentToPathFigure(ref pathFigure[i], startTime.Subtract(timestamps[i][j - 1]).TotalSeconds, distances[i][j - 1], startTime.Subtract(timestamps[i][j]).TotalSeconds, distances[i][j]);
                        }
                    }
                }

                Path[] path = AddPathFigureToPath(pathFigure);

                for (int i = 0; i < path.Length; i++)
                {
                    canvas.Children.Add(path[i]);
                }
            }
            catch { }

            replotGrid = true;
        }
    }
}