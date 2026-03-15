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
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public class VerticalLineAnnotation : StraightLineAnnotation
    {
        public override void UpdateAnnotation()
        {
            if (shape != null)
            {
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        if (XAxis != null && YAxis != null)
                        {
                            if (Chart.AnnotationManager != null && ShowAxisLabel 
                                && !Chart.ChartAnnotationCanvas.Children.Contains(AxisMarkerObject.MarkerCanvas))
                                Chart.AnnotationManager.AddOrRemoveAnnotations(AxisMarkerObject, false);
                           
                            if (XAxis.Orientation == Orientation.Horizontal)
                            {
                                y1 = (Y1 == null) ? YAxis.VisibleRange.Start : ConvertData(Y1, YAxis);
                                y2 = (Y2 == null) ? YAxis.VisibleRange.End : ConvertData(Y2, YAxis);
                                X2 = X1;
                                x1 = ConvertData(X1, XAxis);
                                x2 = ConvertData(X2, XAxis);
                                if (ShowAxisLabel)
                                    SetAxisMarkerValue(X1, X2, YAxis.VisibleRange.Start, YAxis.VisibleRange.End, AxisMode.Horizontal);
                                this.DraggingMode = AxisMode.Horizontal;
                            }
                            else
                            {
                                x1 = (X1 == null) ? XAxis.VisibleRange.Start : ConvertData(X1, XAxis);
                                x2 = (X2 == null) ? XAxis.VisibleRange.End : ConvertData(X2, XAxis);
                                Y2 = Y1;
                                y1 = ConvertData(Y1, YAxis);
                                y2 = ConvertData(Y2, YAxis);
                                if (ShowAxisLabel)
                                    SetAxisMarkerValue(XAxis.VisibleRange.Start, XAxis.VisibleRange.End, Y1, Y2, AxisMode.Vertical);
                                this.DraggingMode = AxisMode.Vertical;
                            }
                            if (ShowAxisLabel)
                                AxisMarkerObject.UpdateAnnotation();
                            if (ShowLine)
                            {
                                Point point = (XAxis.Orientation == Orientation.Horizontal) ? new Point(this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                                        this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1) + YAxis.PlotOffset) : new Point(this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                                        this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1) + XAxis.PlotOffset);
                                Point point2 = (XAxis.Orientation == Orientation.Horizontal) ? new Point(Chart.ValueToPointRelativeToAnnotation(XAxis, x2),
                                                         this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2) - YAxis.PlotOffset) : new Point(this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2),
                                                         Chart.ValueToPointRelativeToAnnotation(XAxis, x2) - XAxis.PlotOffset);
                                DrawLine(point, point2, shape);
                            }
                        }
                        break;
                    case CoordinateUnit.Pixel:
                        if (ShowLine && this.Chart!=null)
                        {
                           this.DraggingMode = AxisMode.Horizontal;
                           if (X1 == null)
                               X1 = 0;
                           X2 = X1;
                           Y1 = (Y1 == null) ? 0 : Y1;
                           if(this.Chart.DesiredSize.Height!=0)
                            Y2 = (Y2 == null) ? this.Chart.DesiredSize.Height : Y2;
                           Point elementPoint1 = new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1));
                           Point elementPoint2 = new Point(Convert.ToDouble(X2), Convert.ToDouble(Y2));
                           DrawLine(elementPoint1, elementPoint2, shape);
                        }
                        break;
                }
            }
        }
        
        internal Point GetAxisLabelPosition(Size desiredSize, Point originalPosition, Size textSize)
        {
            Point point = originalPosition;
            ChartAxis axis = (XAxis.Orientation == Orientation.Horizontal) ? XAxis : YAxis;
            ChartAxis axis1 = (XAxis.Orientation != Orientation.Horizontal) ? XAxis : YAxis;
            point.X += (desiredSize.Width / 2);
            point.X -= (textSize.Width / 2);
            point.Y= axis.OpposedPosition? point.Y : 0;
            var chartAxes = Chart.Axes.Where(axes => (axes.Orientation == axis.Orientation))
                    .Where(position => (position.OpposedPosition));
            double bottom =(chartAxes.Count() > 0)? chartAxes.ElementAt(0).RenderedRect.Bottom: 0;
            if (axis.OpposedPosition)
            {
                point.Y -= (textSize.Height);
                if (Chart.Axes.IndexOf(axis) != 0)
                    point.Y -= (bottom - axis.RenderedRect.Bottom);
                point.Y -= axis1.PlotOffset;
            }
            else
                point.Y += (axis.RenderedRect.Top - bottom);
            return point;
        }
    }
}
