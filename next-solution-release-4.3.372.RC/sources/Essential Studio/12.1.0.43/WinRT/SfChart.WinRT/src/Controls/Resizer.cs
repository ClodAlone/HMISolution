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
#if NETFX_CORE
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public sealed class Resizer : Control
    {
        #region Constructor
        public Resizer()
        {
            this.DefaultStyleKey = typeof(Resizer);
        }
        #endregion

        #region Properties
        private ChartAxis XAxis { get { return AnnotationResizer.XAxis; } }

        private ChartAxis YAxis { get { return AnnotationResizer.YAxis; } }

        private double ActualX1 
        { 
            get 
            {
                if (isAxis)
                    return chart.ValueToPointRelativeToAnnotation(XAxis, AnnotationResizer.ConvertData(AnnotationResizer.X1, XAxis));
                else
                    return Convert.ToDouble(AnnotationResizer.X1);
            }
            set { AnnotationResizer.X1 = value; }
        }

        private double ActualX2
        {
            get
            {
                if (isAxis)
                    return chart.ValueToPointRelativeToAnnotation(XAxis, AnnotationResizer.ConvertData(AnnotationResizer.X2, XAxis));
                else
                    return Convert.ToDouble(AnnotationResizer.X2);
            }
            set { AnnotationResizer.X2 = value; }
        }

        private double ActualY1
        {
            get
            {
                if (isAxis)
                    return chart.ValueToPointRelativeToAnnotation(YAxis, AnnotationResizer.ConvertData(AnnotationResizer.Y1, YAxis));
                else
                    return Convert.ToDouble(AnnotationResizer.Y1);
            }
            set { AnnotationResizer.Y1 = value; }
        }

        private double ActualY2
        {
            get
            {
                if (isAxis)
                    return chart.ValueToPointRelativeToAnnotation(YAxis, AnnotationResizer.ConvertData(AnnotationResizer.Y2, YAxis));
                else
                    return Convert.ToDouble(AnnotationResizer.Y2);
            }
            set { AnnotationResizer.Y2 =  value; }
        }
        
        internal AnnotationResizer AnnotationResizer{ get; set; }

        #endregion

        #region Fields
        
        Thumb resizeTopLeft, resizeMiddleLeft, resizeBottomLeft, resizeTopMiddle, resizeBottomMiddle, resizeBottomRight, resizeTopRight, resizeMiddleRight;

        SfChart chart;
        
        bool isSwapY = false, isSwapX = false,isAxis, isRotated=false;

        double x1, y1, x2, y2;
        
#endregion

        #region Methods
#if NETFX_CORE
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
                isAxis = AnnotationResizer.CoordinateUnit == CoordinateUnit.Axis;
                isRotated = isAxis && XAxis.Orientation != Orientation.Horizontal;
                resizeTopLeft = this.GetTemplateChild("resizeTopLeft") as Thumb;
                resizeMiddleLeft = this.GetTemplateChild("resizeMiddleRight") as Thumb;
                resizeBottomLeft = this.GetTemplateChild("resizeBottomLeft") as Thumb;
                resizeTopMiddle = this.GetTemplateChild("resizeTopMiddle") as Thumb;
                resizeBottomMiddle = this.GetTemplateChild("resizeBottomMiddle") as Thumb;
                resizeTopRight = this.GetTemplateChild("resizeTopRight") as Thumb;
                resizeMiddleRight = this.GetTemplateChild("resizeMiddleLeft") as Thumb;
                resizeBottomRight = this.GetTemplateChild("resizeBottomRight") as Thumb;
                resizeBottomLeft.DragDelta += resizeBottomLeft_DragDelta;
                resizeBottomMiddle.DragDelta += resizeBottomMiddle_DragDelta;
                resizeBottomRight.DragDelta += resizeBottomRight_DragDelta;
                resizeMiddleLeft.DragDelta += resizeMiddleLeft_DragDelta;
                resizeMiddleRight.DragDelta += resizeMiddleRight_DragDelta;
                resizeTopLeft.DragDelta += resizeTopLeft_DragDelta;
                resizeTopMiddle.DragDelta += resizeTopMiddle_DragDelta;
                resizeTopRight.DragDelta += resizeTopRight_DragDelta;
#if WPF
                resizeBottomLeft.DragCompleted += OnDragCompleted;
                resizeTopRight.DragCompleted += OnDragCompleted;
                resizeTopMiddle.DragCompleted += OnDragCompleted;
                resizeTopLeft.DragCompleted += OnDragCompleted;
                resizeMiddleRight.DragCompleted += OnDragCompleted;
                resizeMiddleLeft.DragCompleted += OnDragCompleted;
                resizeBottomMiddle.DragCompleted += OnDragCompleted;
                resizeBottomRight.DragCompleted += OnDragCompleted;
#endif
                chart = AnnotationResizer.Chart;
                CheckCoordinateValue();
                MapActualValueToPixels();
                ChangeView();
        }
#if WPF
        void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            AnnotationResizer.IsResizing = false;
        }
#endif
        private void CheckCoordinateValue()
        {
            if (ActualX1 > ActualX2)
               isSwapX = true;
            if (ActualY1 > ActualY2)
               isSwapY = true;
        }
        
        internal void ChangeView()
        {
            Resizer obj = AnnotationResizer.ResizerControl;
            if (obj != null)
            {
                    if (AnnotationResizer.ResizingMode == AxisMode.Horizontal)
                    {
                        obj.resizeBottomLeft.Visibility = Visibility.Collapsed;
                        obj.resizeBottomRight.Visibility = Visibility.Collapsed;
                        obj.resizeBottomMiddle.Visibility = Visibility.Collapsed;
                        obj.resizeMiddleLeft.Visibility = Visibility.Visible;
                        obj.resizeMiddleRight.Visibility = Visibility.Visible;
                        obj.resizeTopLeft.Visibility = Visibility.Collapsed;
                        obj.resizeTopMiddle.Visibility = Visibility.Collapsed;
                        obj.resizeTopRight.Visibility = Visibility.Collapsed;
                    }
                    else if (AnnotationResizer.ResizingMode == AxisMode.Vertical)
                    {
                        obj.resizeBottomLeft.Visibility = Visibility.Collapsed;
                        obj.resizeBottomRight.Visibility = Visibility.Collapsed;
                        obj.resizeBottomMiddle.Visibility = Visibility.Visible;
                        obj.resizeMiddleLeft.Visibility = Visibility.Collapsed;
                        obj.resizeMiddleRight.Visibility = Visibility.Collapsed;
                        obj.resizeTopLeft.Visibility = Visibility.Collapsed;
                        obj.resizeTopMiddle.Visibility = Visibility.Visible;
                        obj.resizeTopRight.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        obj.resizeBottomLeft.Visibility = Visibility.Visible;
                        obj.resizeBottomRight.Visibility = Visibility.Visible;
                        obj.resizeBottomMiddle.Visibility = Visibility.Visible;
                        obj.resizeMiddleLeft.Visibility = Visibility.Visible;
                        obj.resizeMiddleRight.Visibility = Visibility.Visible;
                        obj.resizeTopLeft.Visibility = Visibility.Visible;
                        obj.resizeTopMiddle.Visibility = Visibility.Visible;
                        obj.resizeTopRight.Visibility = Visibility.Visible;
                    }
              }
        }

        internal void MapActualValueToPixels()
        {
            if (ActualX1 < ActualX2)
            {
                x1 = ActualX1;
                x2 = ActualX2;
            }
            else
            {
                x1 = ActualX2;
                x2 = ActualX1;
            }
            if (ActualY1 < ActualY2)
            {
                y1 = ActualY1;
                y2 = ActualY2;
            }
            else
            {
                y1 = ActualY2;
                y2 = ActualY1;
            }
        }

        void Move(double horizontalChange, double verticalChange, bool isLeft, bool isTop)
        {
            if (isRotated)
            {
                double temp = horizontalChange;
                horizontalChange = verticalChange;
                verticalChange = temp;
            }
            x1 = isLeft ? (x1 + horizontalChange) < x2 ? (x1 + horizontalChange) : x1 : x1;
            x2 = !isLeft ? (x2 + horizontalChange) > x1 ? (x2 + horizontalChange) : x2 : x2;
            y1 = isTop?(y1 + verticalChange) < y2 ? (y1 + verticalChange) : y1: y1;
            y2 = !isTop ? (y2 + verticalChange) > y1 ? (y2 + verticalChange) : y2 : y2;
            MapPixelToActualValue();
        }
        
        void resizeTopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, e.VerticalChange,isRotated, !isRotated);
        }

        void MapPixelToActualValue()
        {
            if (isSwapX)
            {
                ActualX1 = isAxis ? chart.PointToAnnotationValue(XAxis, isRotated ? new Point(0, x2) : new Point(x2, 0)) : x2;
                ActualX2 = isAxis ? chart.PointToAnnotationValue(XAxis, isRotated ? new Point(0, x1) : new Point(x1, 0)) : x1;
            }
            else
            {
                ActualX1 = isAxis ? chart.PointToAnnotationValue(XAxis, isRotated ? new Point(0, x1) : new Point(x1, 0)) : x1;
                ActualX2 = isAxis ? chart.PointToAnnotationValue(XAxis, isRotated ? new Point(0, x2) : new Point(x2, 0)) : x2;
            }
            if (isSwapY)
            {
                ActualY2 = isAxis ? chart.PointToAnnotationValue(YAxis, isRotated ? new Point(y1, 0) : new Point(0, y1)) : y1;
                ActualY1 = isAxis ? chart.PointToAnnotationValue(YAxis, isRotated ? new Point(y2, 0) : new Point(0, y2)) : y2;
            }
            else
            {
                ActualY2 = isAxis ? chart.PointToAnnotationValue(YAxis, isRotated ? new Point(y2, 0) : new Point(0, y2)) : y2;
                ActualY1 = isAxis ? chart.PointToAnnotationValue(YAxis, isRotated ? new Point(y1, 0) : new Point(0, y1)) : y1;
            }
        }

        void resizeTopMiddle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(0, e.VerticalChange, true, true);
        }

        void resizeTopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, e.VerticalChange, true, true);           
        }

        void resizeMiddleRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, 0, false, false);
        }

        void resizeMiddleLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, 0, true, true);
        }

        void resizeBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, e.VerticalChange, false, false);
        }

        void resizeBottomMiddle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(0, e.VerticalChange,false,false);
        }

        void resizeBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, e.VerticalChange, !isRotated, isRotated);
        }
        #endregion
    }
}