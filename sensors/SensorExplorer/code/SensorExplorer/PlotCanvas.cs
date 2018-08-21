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
        private double fontSize = 11;
        private double height = 100;
        private int frame = 10;
        private int maxValue = 0;
        private int minValue = 0;
        private string[] vAxisLabel;
        private Canvas canvas;
        private Color[] color;
        private List<int> plotIndex = new List<int>();

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

                for (int i = sensorData.ReadingList.Count - 1; i >= 0; i--)
                {
                    if (sensorData.ReadingList[i].timestamp < now)
                    {
                        if (i >= 0)
                        {
                            for (int j = 0; j < plotIndex.Count; j++)
                            {
                                int index = plotIndex[j];
                                AddLineSegmentToPathFigure(ref pathFigure[index], 0, sensorData.ReadingList[i].value[index], (sensorData.ReadingList[i].timestamp - now).TotalSeconds, sensorData.ReadingList[i].value[index]);
                            }
                        }

                        for (; i > 0 && sensorData.ReadingList[i].timestamp > now.AddSeconds(-frame); i--)
                        {
                            for (int j = 0; j < plotIndex.Count; j++)
                            {
                                int index = plotIndex[j];
                                AddLineSegmentToPathFigure(ref pathFigure[index], (sensorData.ReadingList[i].timestamp - now).TotalSeconds, sensorData.ReadingList[i - 1].value[index], (sensorData.ReadingList[i - 1].timestamp - now).TotalSeconds, sensorData.ReadingList[i - 1].value[index]);
                            }
                        }

                        //???break;
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
                        pathFigure[i].StartPoint = ((LineSegment) pathFigure[i].Segments[0]).Point;
                    }

                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures.Add(pathFigure[i]);

                    path[i] = new Path();
                    path[i].Data = pathGeometry;
                    path[i].Stroke = new SolidColorBrush(color[i]);
                    path[i].StrokeThickness = 2;
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

                    canvas.Children.Add(vLine);

                    if (frame < 15 || i % 5 == 0)
                    {
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = (-i).ToString();
                        textBlock.FontSize = fontSize;
                        textBlock.Measure(new Size(200, 200)); // Assuming 200x200 is max size of textblock
                        textBlock.Width = textBlock.DesiredSize.Width;
                        textBlock.Height = textBlock.DesiredSize.Height;
                        textBlock.Margin = new Thickness() { Top = vLine.Y2 + 5, Left = vLine.X2 - textBlock.Width / 2 };
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

                    canvas.Children.Add(hLine);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = vAxisLabel[i];
                    textBlock.FontSize = fontSize;
                    textBlock.Measure(new Size(200, 200)); // Assuming 200x200 is max size of textblock
                    textBlock.Width = textBlock.DesiredSize.Width;
                    textBlock.Height = textBlock.DesiredSize.Height;
                    textBlock.Margin = new Thickness() { Top = hLine.Y1 - textBlock.Height / 2, Left = -textBlock.Width - 5 };
                    canvas.Children.Add(textBlock);
                }
            }
            catch { }
        }
    }
}