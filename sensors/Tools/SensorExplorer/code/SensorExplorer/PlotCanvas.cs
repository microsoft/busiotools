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
        private bool _replotGrid = true;
        private double _fontSize = 11;
        private double _height = 100;
        private int _frame = 10;
        private int _maxValue = 0;
        private int _minValue = 0;
        private string[] _vAxisLabel;

        private Canvas _canvas;
        private Color[] _color;
        private List<int> _plotIndex = new List<int>();

        public PlotCanvas(int minValue, int maxValue, Color[] color, Canvas canvas, string[] vAxisLabel)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _color = color;
            _canvas = canvas;
            _vAxisLabel = vAxisLabel;

            for (int i = 0; i < _color.Length; i++)
            {
                if (_color[i] != Colors.Black)
                {
                    _plotIndex.Add(i);
                }
            }
        }

        public void SetFrame(int frame)
        {
            _frame = frame;
            _replotGrid = true;
        }

        public void SetHeight(double height)
        {
            _height = height;
            _replotGrid = true;
        }

        public void SetFontSize(double fontSize)
        {
            _fontSize = fontSize;
            _replotGrid = true;
        }

        public void SetRange(int maxValue, int minValue)
        {
            for (int i = 0; i < _vAxisLabel.Length; i++)
            {
                _vAxisLabel[i] = (maxValue - Convert.ToDouble(i) / (_vAxisLabel.Length - 1) * (maxValue - minValue)).ToString();
            }

            _maxValue = maxValue;
            _minValue = minValue;
            _replotGrid = true;
        }

        public void Plot(SensorData sensorData)
        {
            try
            {
                if (_replotGrid)
                {
                    _canvas.Children.Clear();
                    _canvas.Height = _height;

                    PlotGrid(_maxValue, _minValue, Colors.Black);

                    _replotGrid = false;
                }
                else
                {
                    int index = _canvas.Children.Count;

                    for (int i = 0; i < _plotIndex.Count; i++)
                    {
                        index--;
                        _canvas.Children.RemoveAt(index);
                    }
                }

                PathFigure[] pathFigure = new PathFigure[_plotIndex.Count];

                for (int i = 0; i < pathFigure.Length; i++)
                {
                    pathFigure[i] = new PathFigure();
                }

                DateTime now = DateTime.UtcNow;

                for (int i = sensorData._reading.Count - 1; i >= 0; i--)
                {
                    if (sensorData._reading[i].timestamp < now)
                    {
                        if (i >= 0)
                        {
                            for (int j = 0; j < _plotIndex.Count; j++)
                            {
                                int index = _plotIndex[j];
                                AddLineSegmentToPathFigure(ref pathFigure[index], 0, sensorData._reading[i].value[index], (sensorData._reading[i].timestamp - now).TotalSeconds, sensorData._reading[i].value[index]);
                            }
                        }

                        for (; i > 0 && sensorData._reading[i].timestamp > now.AddSeconds(-_frame); i--)
                        {
                            for (int j = 0; j < _plotIndex.Count; j++)
                            {
                                int index = _plotIndex[j];
                                AddLineSegmentToPathFigure(ref pathFigure[index], (sensorData._reading[i].timestamp - now).TotalSeconds, sensorData._reading[i - 1].value[index], (sensorData._reading[i - 1].timestamp - now).TotalSeconds, sensorData._reading[i - 1].value[index]);
                            }
                        }

                        break;
                    }
                }

                Path[] path = AddPathFigureToPath(pathFigure);

                for (int i = 0; i < path.Length; i++)
                {
                    _canvas.Children.Add(path[i]);
                }
            }
            catch { }
        }

        private void AddLineSegmentToPathFigure(ref PathFigure pathFigure, double currTimestamp, double currValue, double prevTimestamp, double prevValue)
        {
            try
            {
                LineSegment lineSegment;

                double currX = Math.Max(Math.Min(_canvas.Width * -currTimestamp / _frame, _canvas.Width), 0);
                double currY = Math.Max(Math.Min(_canvas.Height * (_maxValue - currValue) / (_maxValue - _minValue), _canvas.Height), 0);

                lineSegment = new LineSegment();
                lineSegment.Point = new Point(currX, currY);
                pathFigure.Segments.Add(lineSegment);

                double prevX = Math.Max(Math.Min(_canvas.Width * -prevTimestamp / _frame, _canvas.Width), 0);
                double prevY = Math.Max(Math.Min(_canvas.Height * (_maxValue - prevValue) / (_maxValue - _minValue), _canvas.Height), 0);

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

                    path[i] = new Path();
                    path[i].Data = pathGeometry;
                    path[i].Stroke = new SolidColorBrush(_color[i]);
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
                for (int i = 0; i <= _frame; i++)
                {
                    Line vLine = new Line();

                    vLine.X1 = i * _canvas.Width / _frame;
                    vLine.Y1 = 0;
                    vLine.X2 = vLine.X1;
                    vLine.Y2 = _canvas.Height;

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
                    _canvas.Children.Add(vLine);

                    if (_frame < 15 || i % 5 == 0)
                    {
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = (-i).ToString();
                        textBlock.FontSize = _fontSize;
                        textBlock.Measure(new Size(200, 200)); // Assuming 200x200 is max size of textblock
                        textBlock.Width = textBlock.DesiredSize.Width;
                        textBlock.Height = textBlock.DesiredSize.Height;
                        textBlock.Margin = new Thickness() { Top = vLine.Y2 + 5, Left = vLine.X2 - textBlock.Width / 2 };
                        _canvas.Children.Add(textBlock);
                    }
                }

                for (int i = 0; i < _vAxisLabel.Length; i++)
                {
                    Line hLine = new Line();

                    hLine.X1 = 0;
                    hLine.Y1 = i * _canvas.Height / (_vAxisLabel.Length - 1);
                    hLine.X2 = _canvas.Width;
                    hLine.Y2 = hLine.Y1;

                    hLine.Stroke = new SolidColorBrush(color);

                    if (i < _vAxisLabel.Length - 1)
                    {
                        hLine.StrokeThickness = 1;
                    }
                    else
                    {
                        hLine.StrokeThickness = 2;
                    }

                    // Add the path to the Canvas
                    _canvas.Children.Add(hLine);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = _vAxisLabel[i];
                    textBlock.FontSize = _fontSize;
                    textBlock.Measure(new Size(200, 200)); // Assuming 200x200 is max size of textblock
                    textBlock.Width = textBlock.DesiredSize.Width;
                    textBlock.Height = textBlock.DesiredSize.Height;
                    textBlock.Margin = new Thickness() { Top = hLine.Y1 - textBlock.Height / 2, Left = -textBlock.Width - 5 };
                    _canvas.Children.Add(textBlock);
                }
            }
            catch { }
        }
    }
}