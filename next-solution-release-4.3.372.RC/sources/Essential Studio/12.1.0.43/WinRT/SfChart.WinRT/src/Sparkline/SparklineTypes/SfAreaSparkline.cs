#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using WindowsLinesegment = System.Windows.Media.LineSegment;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Data;
using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class SfAreaSparkline : MarkerBase
    {
        #region fields

        Path segmentPath;

        #endregion

        #region ctor

        public SfAreaSparkline()
        {

        }

        #endregion

        #region methods

        protected override void RenderSegments()
        {
            bool isAdded = true;
            base.RenderSegments();
            PathFigure figure;
            PathGeometry segmentGeometry;
            WindowsLinesegment lineSegment;
            Point pointToScreen;
            double xTolerance = Math.Abs((deltaX * 5) / availableWidth);
            double yTolerance = Math.Abs((deltaY * 5) / availableHeight);
            double prevXValue = 1, prevYValue = 0, yVal = 0, xVal = 0;
            int index = 0;
            segmentGeometry = new PathGeometry();
            figure = new PathFigure();
            figure.StartPoint = TransformToVisible(xValues[0], 0);
            int segCount = EmptyPointValue == EmptyPointValues.None ? EmptyPointIndexes.Count : 1;
            for (int j = 0; j < segCount; j++)
            {
                if (xValues.Count > EmptyPointIndexes[j] + 1)
                {
                    isAdded = false;
                    if (j != 0)
                    {
                        segmentGeometry = new PathGeometry();
                        figure = new PathFigure();

                        figure.StartPoint = TransformToVisible(xValues[(int)EmptyPointIndexes[j] + 1], 0);
                    }
                    if (SegmentPresenter.Children.Count > j)
                        segmentPath = SegmentPresenter.Children[j] as Path;
                    else
                    {
                        segmentPath = new Path();
                        SetBinding(segmentPath);
                        SegmentPresenter.Children.Add(segmentPath);
                    }
                    segmentPath.Clip = null;
                    for (double i = EmptyPointIndexes[j]; i < yValues.Count; i++)
                    {
                        if (yValues.Count > index)
                        {
                            yVal = yValues[index];
                            xVal = xValues[index];

                            if (!double.IsNaN(yVal))
                            {
                                if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                                {
                                    prevXValue = xVal;
                                    prevYValue = yVal;
                                    lineSegment = new WindowsLinesegment();
                                    pointToScreen = TransformToVisible(xValues[index], yValues[index]);
                                    lineSegment.Point = pointToScreen;
                                    figure.Segments.Add(lineSegment);
                                    if (MarkerVisibility == Visibility.Visible)
                                    {
                                        AddMarker(pointToScreen, xVal, yVal, index);
                                    }
                                }
                            }
                            else
                            {
                                isAdded = true;
                                AddSegment(figure, segmentGeometry, ref segmentPath, TransformToVisible(xValues[index != 0 ? index - 1 : index], 0));
                                index++;
                                break;
                            }
                            index++;
                        }
                    }
                }
                if (!isAdded)
                {
                    AddSegment(figure, segmentGeometry, ref segmentPath, TransformToVisible(xValues[index - 1], 0));
                    this.segmentPath.Data = segmentGeometry;
                }
            }
        }

        private void AddSegment(PathFigure figure, PathGeometry segmentGeometry, ref Path segmantPath, Point screenPoint)
        {
            WindowsLinesegment lineSegment = new WindowsLinesegment();
            lineSegment.Point = screenPoint;
            figure.Segments.Add(lineSegment);
            figure.IsClosed = true;
            segmentGeometry.Figures.Add(figure);
            segmantPath.Data = segmentGeometry;
        }

        internal override void SetBinding(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Interior");
            element.SetBinding(Shape.FillProperty, binding);
            base.SetBinding(element);
        }
        #endregion
    }
}
