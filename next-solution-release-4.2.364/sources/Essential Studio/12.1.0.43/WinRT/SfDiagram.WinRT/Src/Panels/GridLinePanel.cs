#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Diagram.Panels
{
    internal class GridLinePanel : Panel
    {
        List<Path> Remove = new List<Path>();
        internal Path DrawLine(Point pt1, Point pt2, double thickness, Style style)
        {
            Path _path = new Path();
            _path.StrokeThickness = thickness;
            _path.Style = style;
            LineGeometry geometry = new LineGeometry() { StartPoint = pt1, EndPoint = pt2 };
            _path.Data = geometry;
            return _path;
        }

        internal void ChangePoints(Point point1, Point point2, Path path, double thickness, Style style)
        {
            LineGeometry geo = path.Data as LineGeometry;
            geo.StartPoint = point1;
            geo.EndPoint = point2;
            path.StrokeThickness = thickness;
            path.Style = style;
        }
        internal void DrawHLine(Point point1, Point point2)
        {
            Path _path = new Path();
            _path.StrokeThickness = 1;
            _path.Stroke = new SolidColorBrush(Colors.Gray);
            _path.StrokeDashArray = new DoubleCollection() { 6, 6 };
            LineGeometry geometry = new LineGeometry() { StartPoint = point1, EndPoint = point2 };
            _path.Data = geometry;
            this.Children.Add(_path);
            Remove.Add(_path);
        }

        internal void DrawVLine(Point point1, Point point2)
        {
            Path _path = new Path();
            _path.StrokeThickness = 1;
            _path.Stroke = new SolidColorBrush(Colors.Gray);
            _path.StrokeDashArray = new DoubleCollection() { 6, 6 };
            LineGeometry geometry = new LineGeometry() { StartPoint = point1, EndPoint = point2 };
            _path.Data = geometry;
            this.Children.Add(_path);
            Remove.Add(_path);
        }

        internal void Clear()
        {
            if (Remove.Count > 0)
            {
                foreach (var p in Remove)
                {
                    this.Children.Remove(p);
                }
                Remove.Clear();
            }
        }
    }
}
