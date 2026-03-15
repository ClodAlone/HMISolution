#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// The ChartSeries class represents an series of the <see
    /// cref="ChartArea">ChartArea</see>.
    /// </summary>
    /// <remarks>
    /// A ChartArea contains any number of series.  Each series has different chart types and data points.
    /// </remarks>
    /// <seealso cref="ChartArea">ChartArea class specification</seealso>
    /// <seealso cref="ChartAxis">ChartAxis class specification</seealso>
    [ContentProperty("Data")]
    public class ChartSeries : Control,IDisposable
    {
        internal bool IsDataModified = false;
        internal bool IsRefreshAnimation = true;
        internal static readonly DependencyProperty IsXAxisInversedProperty =
DependencyProperty.Register("IsXAxisInversed", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnIsXAxisInversedChanged)));

        internal bool IsXAxisInversed
        {
            get
            {
                return (bool)GetValue(IsXAxisInversedProperty);
            }

            set
            {
                SetValue(IsXAxisInversedProperty, value);
            }
        }

        private static void OnIsXAxisInversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.seriesGrid != null && series.XAxis != null)
            {
                if (series.XAxis.Orientation == Orientation.Horizontal)
                {
                    series.ScaleX = series.ScaleX * -1;
                }
                else
                {
                    series.ScaleY = series.ScaleY * -1;
                }

                series.seriesGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                series.seriesGrid.RenderTransform = new CompositeTransform() { ScaleX = series.ScaleX, ScaleY = series.ScaleY };
                series.XAxis.UpdateAdornmentOnZoom(series);
            }
        }

        internal static readonly DependencyProperty IsYAxisInversedProperty =
DependencyProperty.Register("IsYAxisInversed", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnIsYAxisInversedChanged)));

        internal bool IsYAxisInversed
        {
            get
            {
                return (bool)GetValue(IsYAxisInversedProperty);
            }

            set
            {
                SetValue(IsYAxisInversedProperty, value);
            }
        }

        private static void OnIsYAxisInversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.seriesGrid != null && series.XAxis != null)
            {
                if (series.YAxis.Orientation == Orientation.Horizontal)
                {
                    series.ScaleX = series.ScaleX * -1;
                }
                else
                {
                    series.ScaleY = series.ScaleY * -1;
                }

                series.seriesGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                series.seriesGrid.RenderTransform = new CompositeTransform() { ScaleX = series.ScaleX, ScaleY = series.ScaleY };
                series.YAxis.UpdateAdornmentOnZoom(series);
            }
        }

        /// <summary>
        /// Identifies the ChartType dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartTypeProperty =
  DependencyProperty.Register("ChartType", typeof(ChartType), typeof(ChartSeries), new PropertyMetadata(ChartSeries.KnownType(ChartTypes.Column, null), new PropertyChangedCallback(OnChartTypeChanged)));

        /// <summary>
        /// Gets or sets the value of the chart Type.  This is a dependency property.
        /// </summary>
        /// <remarks>Intended to be used with custom chart types.</remarks>
        public ChartType ChartType
        {
            get
            {
                return (ChartType)GetValue(ChartTypeProperty);
            }

            set
            {
                SetValue(ChartTypeProperty, value);
            }
        }

        private static void OnChartTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if(series!=null)
            {
                series.CalculateSegments();
            }
        }

        /// <summary>
        /// Identifies the IsVisibleOnLegend dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisbileOnLegendProperty =
  DependencyProperty.Register("IsVisbileOnLegend", typeof(bool), typeof(ChartSeries), new PropertyMetadata(true, new PropertyChangedCallback(OnVisibleValueChanged)));

        /// <summary>
        /// Gets or sets the value of the IsVisbileOnLegend.  This is a dependency property.
        /// </summary>
        /// <remarks>sets the visbility of the legend.</remarks>
        public bool IsVisbileOnLegend
        {
            get
            {
                return (bool)GetValue(IsVisbileOnLegendProperty);
            }

            set
            {
                SetValue(IsVisbileOnLegendProperty, value);
            }
        }
        private static void OnVisibleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.Area != null && series.Area.Legends!= null)
            {
                series.Area.Legends.GenerateItems();
            }
        }
        internal Dictionary<object, object> OldAnimatedValues
        {
            get;
            set;
        }

        #region revamp
        ChartDataModel m_DataModel = new ChartDataModel();
        /// <summary>
        /// Get or Set DataModel
        /// </summary>
        public ChartDataModel DataModel
        {
            get { return m_DataModel; }
            set
            {
                m_DataModel = value;
            }
        }

        double scaleX = 1d, scaleY = 1d;

        internal double ScaleX
        {
            get { return scaleX; }
            set { scaleX = value; }
        }

        internal double ScaleY
        {
            get { return scaleY; }
            set { scaleY = value; }
        }

        double renderOriginX = 0.5d, renderOriginY = 0.5d;

        internal double RenderOriginX
        {
            get { return renderOriginX; }
            set { renderOriginX = value; }
        }

        internal double RenderOriginY
        {
            get { return renderOriginY; }
            set { renderOriginY = value; }
        }

        #endregion

        #region Interactive Cursor

        internal Line HorizontalCursor;
        internal Line VerticalCursor;
        private bool HCFlag = false;
        private bool VCFlag = false;
        internal Visibility ICVisibility=Visibility.Collapsed;
        private bool InteractiveCursorLoaded = false;

        //internal void SetValuesForCursor()
        //{
        //    if (this.Data.Count == 0)
        //    {
        //        if (ICVisibility == this.InteractiveCursor.CursorVisibility)
        //        {
        //            Visibility temp = this.InteractiveCursor.CursorVisibility;
        //            this.InteractiveCursor.CursorVisibility = Visibility.Collapsed;
        //            this.ICVisibility = temp;
        //        }
        //        return;
        //    }

        //    if (this.Area.GetSeriesAxesType(this.Type) == ChartAxesType.PolarAxes || this.Area.GetSeriesAxesType(this.Type) == ChartAxesType.RadarAxes ||
        //        this.Area.GetSeriesAxesType(this.Type) == ChartAxesType.None || this.Type == ChartTypes.Funnel || this.Type == ChartTypes.Pyramid)
        //    {
        //        Visibility temp = InteractiveCursor.CursorVisibility;
        //        this.InteractiveCursor.CursorVisibility = Visibility.Collapsed;
        //        this.ICVisibility = temp;
        //        return;
        //    }

        //    InteractiveCursor.XPosition = InteractiveCursor.XPosition > this.Data.Count ? this.Data.Count : InteractiveCursor.XPosition;
        //    InteractiveCursor.YPosition = InteractiveCursor.YPosition > this.Data.Count ? this.Data.Count : InteractiveCursor.YPosition;
        //    InteractiveCursor.XPosition = InteractiveCursor.XPosition < 1 ? this.Data.Count : InteractiveCursor.XPosition;
        //    InteractiveCursor.YPosition = InteractiveCursor.YPosition < 1 ? this.Data.Count : InteractiveCursor.YPosition;
            
        //    if (ICVisibility == Visibility.Visible)
        //    {
        //        if (this.Visibility == Visibility.Collapsed)
        //        {
        //            this.InteractiveCursor.CursorVisibility = Visibility.Collapsed;
        //            ICVisibility = Visibility.Visible;
        //            return;
        //        }
        //        else
        //            this.InteractiveCursor.CursorVisibility = ICVisibility;
        //    }

        //    if (this.IsRotated(this.Type) == true)
        //    {
        //        if (VerticalCursor != null && this.InteractiveCursor != null)
        //        {
        //            VerticalCursor.X1 = Area.ValueToPoint(Area.SecondaryAxis, this.Data[(int)this.InteractiveCursor.YPosition - 1].Y);
        //            VerticalCursor.X2 = Area.ValueToPoint(Area.SecondaryAxis, this.Data[(int)this.InteractiveCursor.YPosition - 1].Y);
        //        }

        //        if (HorizontalCursor != null && this.InteractiveCursor != null)
        //        {
        //            HorizontalCursor.Y1 = Area.ValueToPoint(Area.PrimaryAxis, this.Data[(int)this.InteractiveCursor.XPosition - 1].X);
        //            HorizontalCursor.Y2 = Area.ValueToPoint(Area.PrimaryAxis, this.Data[(int)this.InteractiveCursor.XPosition - 1].X);
        //        }
        //    }
        //    else
        //    {
        //        if (HorizontalCursor != null && this.InteractiveCursor != null)
        //        {
        //            HorizontalCursor.Y1 = Area.ValueToPoint(Area.SecondaryAxis, this.Data[(int)this.InteractiveCursor.YPosition - 1].Y);
        //            HorizontalCursor.Y2 = Area.ValueToPoint(Area.SecondaryAxis, this.Data[(int)this.InteractiveCursor.YPosition - 1].Y);
        //        }

        //        if (VerticalCursor != null && this.InteractiveCursor != null)
        //        {
        //            VerticalCursor.X1 = Area.ValueToPoint(Area.PrimaryAxis, this.Data[(int)this.InteractiveCursor.XPosition - 1].X);
        //            VerticalCursor.X2 = Area.ValueToPoint(Area.PrimaryAxis, this.Data[(int)this.InteractiveCursor.XPosition - 1].X);
        //        }
        //    }
        //}

        void DrawCursorLines()
        {
            if (this.Area.GetSeriesAxesType(this.Type) == ChartAxesType.PolarAxes || this.Area.GetSeriesAxesType(this.Type) == ChartAxesType.RadarAxes ||
                    this.Area.GetSeriesAxesType(this.Type) == ChartAxesType.None || this.Type == ChartTypes.Funnel || this.Type == ChartTypes.Pyramid)
            {
                if (InteractiveCursorLoaded == false)
                {
                    InteractiveCursorLoaded = true;
                    this.ICVisibility = this.InteractiveCursor.CursorVisibility;
                }
                return;
            }
            InteractiveCursorLoaded = true;
            if (this.HorizontalCursor != null || this.VerticalCursor != null)
            {
                Area.seriesGrid.Children.Remove(HorizontalCursor);
                Area.seriesGrid.Children.Remove(VerticalCursor);
            }

            //InteractiveCursor.chartseries = this;
            HorizontalCursor = new Line();
            HorizontalCursor.X1 = 0;
            Binding seriescanvaswidthbind = new Binding();
            seriescanvaswidthbind.Source = Area;
            seriescanvaswidthbind.Path = new PropertyPath("seriescanvaswidth");

            //Setting HorizontalCursor Mouse Pointer
            HorizontalCursor.Cursor = Cursors.SizeNS;

            //Binding HorizontalCursor Stroke
            Binding HorizontalCursorStrokebind = new Binding();
            HorizontalCursorStrokebind.Source = InteractiveCursor;
            HorizontalCursorStrokebind.Path = new PropertyPath("HorizontalCursorStroke");
            BindingOperations.SetBinding(HorizontalCursor, Line.StrokeProperty, HorizontalCursorStrokebind);

            //Binding Stroke thickness
            Binding strokethicknessbind = new Binding();
            strokethicknessbind.Source = InteractiveCursor;
            strokethicknessbind.Path = new PropertyPath("CursorStrokeThickness");
            BindingOperations.SetBinding(HorizontalCursor, Line.StrokeThicknessProperty, strokethicknessbind);

            //Binding Visibility
            Binding CursorVisibilitybind = new Binding();
            CursorVisibilitybind.Source = InteractiveCursor;
            CursorVisibilitybind.Path = new PropertyPath("CursorVisibility");
            BindingOperations.SetBinding(HorizontalCursor, Line.VisibilityProperty, CursorVisibilitybind);
            
            VerticalCursor = new Line();
            VerticalCursor.Y1 = 0;
            Binding seriescanvaswheightbind = new Binding();
            seriescanvaswheightbind.Source = Area;
            seriescanvaswheightbind.Path = new PropertyPath("seriescanvasheight");

            //Setting VerticalCursor Mouse Pointer
            VerticalCursor.Cursor = Cursors.SizeWE;

            //Binding VerticalCursor Stroke
            Binding VerticalCursorStrokebind = new Binding();
            VerticalCursorStrokebind.Source = InteractiveCursor;
            VerticalCursorStrokebind.Path = new PropertyPath("VerticalCursorStroke");
            BindingOperations.SetBinding(VerticalCursor, Line.StrokeProperty, VerticalCursorStrokebind);

            //Binding Stroke thickness
            BindingOperations.SetBinding(VerticalCursor, Line.StrokeThicknessProperty, strokethicknessbind);

            //Binding Visibility
            BindingOperations.SetBinding(VerticalCursor, Line.VisibilityProperty, CursorVisibilitybind);
            
            Area.seriesGrid.Children.Add(HorizontalCursor);
            Area.seriesGrid.Children.Add(VerticalCursor);

            if (this.ICVisibility == Visibility.Collapsed)
                this.ICVisibility = this.InteractiveCursor.CursorVisibility;

            InteractiveCursor.XPosition = InteractiveCursor.XPosition > this.Data.Count ? this.Data.Count : InteractiveCursor.XPosition;
            InteractiveCursor.YPosition = InteractiveCursor.YPosition > this.Data.Count ? this.Data.Count : InteractiveCursor.YPosition;

            BindingOperations.SetBinding(HorizontalCursor, Line.X2Property, seriescanvaswidthbind);
            BindingOperations.SetBinding(VerticalCursor, Line.Y2Property, seriescanvaswheightbind);

            if (this.IsRotated(this.Type))
            {
                HorizontalCursor.MouseLeftButtonDown += new MouseButtonEventHandler(VerticalCursor_MouseLeftButtonDown);
                HorizontalCursor.MouseLeftButtonUp += new MouseButtonEventHandler(VerticalCursor_MouseLeftButtonUp);
                VerticalCursor.MouseLeftButtonDown += new MouseButtonEventHandler(HorizontalCursor_MouseLeftButtonDown);
                VerticalCursor.MouseLeftButtonUp += new MouseButtonEventHandler(HorizontalCursor_MouseLeftButtonUp);
            }
            else
            {
                HorizontalCursor.MouseLeftButtonDown += new MouseButtonEventHandler(HorizontalCursor_MouseLeftButtonDown);
                HorizontalCursor.MouseLeftButtonUp += new MouseButtonEventHandler(HorizontalCursor_MouseLeftButtonUp);
                VerticalCursor.MouseLeftButtonDown += new MouseButtonEventHandler(VerticalCursor_MouseLeftButtonDown);
                VerticalCursor.MouseLeftButtonUp += new MouseButtonEventHandler(VerticalCursor_MouseLeftButtonUp);
            }

            if (InteractiveCursor.XPosition >= 1 && InteractiveCursor.YPosition >= 1)
            {
                if (this.IsRotated(this.Type))
                    DrawCursorLinesForBarType();
                else
                    DrawCursorLinesForOtherTypes();
            }
        }

        void DrawCursorLinesForOtherTypes()
        {
            HorizontalCursor.Y1 = Area.ValueToPoint(Area.SecondaryAxis, this.Data[(int)this.InteractiveCursor.YPosition - 1].Y);
            HorizontalCursor.Y2 = Area.ValueToPoint(Area.SecondaryAxis, this.Data[(int)this.InteractiveCursor.YPosition - 1].Y);

            VerticalCursor.X1 = Area.ValueToPoint(Area.PrimaryAxis, this.Data[(int)this.InteractiveCursor.XPosition - 1].X);
            VerticalCursor.X2 = Area.ValueToPoint(Area.PrimaryAxis, this.Data[(int)this.InteractiveCursor.XPosition - 1].X);
        }

        void DrawCursorLinesForBarType()
        {
            HorizontalCursor.Y1 = Area.ValueToPoint(Area.PrimaryAxis, this.Data[(int)this.InteractiveCursor.XPosition - 1].X);
            HorizontalCursor.Y2 = Area.ValueToPoint(Area.PrimaryAxis, this.Data[(int)this.InteractiveCursor.XPosition - 1].X);

            VerticalCursor.X1 = Area.ValueToPoint(Area.SecondaryAxis, this.Data[(int)this.InteractiveCursor.YPosition - 1].Y);
            VerticalCursor.X2 = Area.ValueToPoint(Area.SecondaryAxis, this.Data[(int)this.InteractiveCursor.YPosition - 1].Y);
        }

        void VerticalCursor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Area.Cursor = Cursors.Arrow;
            HCFlag = false;
            VCFlag = false;
            Area.MouseLeave -= new MouseEventHandler(Area_MouseLeave);
            Area.MouseLeftButtonUp -= new MouseButtonEventHandler(Area_MouseLeftButtonUp);
            Area.MouseMove -= new MouseEventHandler(Area_MouseMove);
        }

        void VerticalCursor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VCFlag = true;
            Area.MouseLeave += new MouseEventHandler(Area_MouseLeave);
            Area.MouseLeftButtonUp += new MouseButtonEventHandler(Area_MouseLeftButtonUp);
            Area.MouseMove += new MouseEventHandler(Area_MouseMove);
        }

        void HorizontalCursor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Area.Cursor = Cursors.Arrow;
            HCFlag = false;
            VCFlag = false;
            Area.MouseLeave -= new MouseEventHandler(Area_MouseLeave);
            Area.MouseLeftButtonUp -= new MouseButtonEventHandler(Area_MouseLeftButtonUp);
            Area.MouseMove -= new MouseEventHandler(Area_MouseMove);
        }

        void HorizontalCursor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HCFlag = true;
            Area.MouseLeave += new MouseEventHandler(Area_MouseLeave);
            Area.MouseLeftButtonUp += new MouseButtonEventHandler(Area_MouseLeftButtonUp);
            Area.MouseMove += new MouseEventHandler(Area_MouseMove);
        }

        void Area_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Area.Cursor = Cursors.Arrow;
            HCFlag = false;
            VCFlag = false;
            Area.MouseLeave -= new MouseEventHandler(Area_MouseLeave);
            Area.MouseMove -= new MouseEventHandler(Area_MouseMove);
            Area.MouseLeftButtonUp -= new MouseButtonEventHandler(Area_MouseLeftButtonUp);
        }

        void Area_MouseMove(object sender, MouseEventArgs e)
        {
            if (HCFlag == true)
            {
                this.Area.Cursor = Cursors.SizeNS;
                if (this.IsRotated(this.Type)) { this.Area.Cursor = Cursors.SizeWE; }// this.Type.ToString() == "Bar")
                Point pt = e.GetPosition(this.Area.seriesGrid);
                double cursor_datapoint_x = this.Area.PointToValue(Area.PrimaryAxis, pt);
                double cursor_datapoint_y = this.Area.PointToValue(Area.SecondaryAxis, pt);
                int near = 0;
                for (int i = 0; i < this.Data.Count; i++)
                {
                    if (((cursor_datapoint_y - this.Data[i].Y) * (cursor_datapoint_y - this.Data[i].Y)) <
                        ((cursor_datapoint_y - this.Data[near].Y) * (cursor_datapoint_y - this.Data[near].Y)))
                    {
                        near = i;
                    }
                }
                this.InteractiveCursor.XPosition = near + 1;
                this.InteractiveCursor.YPosition = near + 1;
            }
            if (VCFlag == true)
            {
                this.Area.Cursor = Cursors.SizeWE;
                if (this.IsRotated(this.Type)) { this.Area.Cursor = Cursors.SizeNS; }//this.Type.ToString() == "Bar")
                Point pt = e.GetPosition(this.Area.seriesGrid);
                double cursor_datapoint_x = this.Area.PointToValue(Area.PrimaryAxis, pt);
                double cursor_datapoint_y = this.Area.PointToValue(Area.SecondaryAxis, pt);

                int near = 0;
                for (int i = 0; i < this.Data.Count; i++)
                {
                    if (((cursor_datapoint_x - this.Data[i].X) * (cursor_datapoint_x - this.Data[i].X)) <
                        ((cursor_datapoint_x - this.Data[near].X) * (cursor_datapoint_x - this.Data[near].X)))
                    {
                        near = i;
                    }
                }
                this.InteractiveCursor.XPosition = near + 1;
                this.InteractiveCursor.YPosition = near + 1;
            }
        }

        void Area_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Area.Cursor = Cursors.Arrow;
            HCFlag = false;
            VCFlag = false;
            Area.MouseMove -= new MouseEventHandler(Area_MouseMove);
            Area.MouseLeftButtonUp -= new MouseButtonEventHandler(Area_MouseLeftButtonUp);
        }

        /// <summary>
        /// Identifies the InteractiveCursor dependency property.
        /// </summary>
        public static readonly DependencyProperty InteractiveCursorProperty =
            DependencyProperty.Register("InteractiveCursor", typeof(InteractiveCursor),
            typeof(ChartSeries), new PropertyMetadata(null));
        /// <summary>
        /// Get or Set InteractiveCursorProperty
        /// </summary>
        public InteractiveCursor InteractiveCursor
        {
            get { return (InteractiveCursor)GetValue(InteractiveCursorProperty); }
            set { SetValue(InteractiveCursorProperty, value); }
        }

        #endregion

        internal static ChartSeries zoomseries = null;
        private double m_actualopacityonzoom = 1d;
        internal double sum = 0d, minimum = 0d, maximum = 0d;
        internal bool isseriesvisible = true, isdatasourcedata = false;
        private SegmentsCollection m_adornments = new SegmentsCollection();
        internal SegmentsCollection Adornments
        {
            get
            {
                return m_adornments;
            }
        }


        /// <summary>
        /// Identifies the EmptyPointStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyPointStyleProperty = DependencyProperty.Register("EmptyPointStyle", typeof(EmptyPointStyle), typeof(ChartSeries), new PropertyMetadata(EmptyPointStyle.Symbol, new PropertyChangedCallback(OnEmptyPointPropertyChanged)));

        /// <summary>
        /// Identifies the EmptyPointInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyPointInteriorProperty = DependencyProperty.Register("EmptyPointInterior", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(new SolidColorBrush(Colors.Orange), new PropertyChangedCallback(OnEmptyPointPropertyChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EmptyPointValueProperty = DependencyProperty.Register("EmptyPointValue", typeof(EmptyPointValue), typeof(ChartSeries), new PropertyMetadata(EmptyPointValue.Zero, new PropertyChangedCallback(OnEmptyPointPropertyChanged)));

        /// <summary>
        /// Identifies the ShowEmptyPoints dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowEmptyPointsProperty = DependencyProperty.Register("ShowEmptyPoints", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnEmptyPointPropertyChanged)));

        private static void OnEmptyPointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Identifies the currSegment dependency property.
        /// </summary>
        public static readonly DependencyProperty currSegmentProperty = DependencyProperty.Register("currSegment", typeof(Segment), typeof(ChartSeries), new PropertyMetadata(null));
        /// <summary>
        /// Get or Set currSegment
        /// </summary>
        public Segment currSegment
        {
            set { SetValue(currSegmentProperty, value); }
            get { return (Segment)GetValue(currSegmentProperty); }
        } 


        /// <summary>
        /// Gets or sets a value indicating whether to show empty points.
        /// </summary>
        /// <value><c>true</c> if show empty points otherwise, <c>false</c>.</value>
        public bool ShowEmptyPoints
        {
            get { return (bool)GetValue(ShowEmptyPointsProperty); }
            set { SetValue(ShowEmptyPointsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the empty point interior.
        /// </summary>
        /// <value>The empty point interior.</value>
        public Brush EmptyPointInterior
        {
            get { return (Brush)GetValue(EmptyPointInteriorProperty); }
            set { SetValue(EmptyPointInteriorProperty, value); }
        }
        /// <summary>
        /// Get or Set EmptyPointValue
        /// </summary>
        public EmptyPointValue EmptyPointValue
        {
            get { return (EmptyPointValue)GetValue(EmptyPointValueProperty); }
            set { SetValue(EmptyPointValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the empty point style.
        /// </summary>
        /// <value>The empty point style.</value>
        public EmptyPointStyle EmptyPointStyle
        {
            get { return (EmptyPointStyle)GetValue(EmptyPointStyleProperty); }
            set { SetValue(EmptyPointStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Highlighted. This is a dependency property.
        /// </summary>
        /// <value>The Highlighted.</value>
        public bool Highlighted
        {
            get { return (bool)GetValue(HighlightedProperty); }
            set { SetValue(HighlightedProperty, value); }
        }

        /// <summary>
        /// Identifies the Highlighted dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightedProperty =
            DependencyProperty.Register("Highlighted", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value for HighlightedSegmentInterior . This is a dependency property.
        /// </summary>
        /// <value>The HighlightedSegmentInterior .</value>
        public Brush HighlightedSegmentInterior 
        {
            get { return (Brush)GetValue(HighlightedSegmentInteriorProperty); }
            set { SetValue(HighlightedSegmentInteriorProperty, value); }
        }

        /// <summary>
        /// Identifies the HighlightedSegment dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightedSegmentProperty =
            DependencyProperty.Register("HighlightedSegment", typeof(Segment), typeof(ChartSeries), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets a value for HighlightedSegment. This is a dependency property.
        /// </summary>
        /// <value>The HighlightedSegment.</value>
        public Segment HighlightedSegment
        {
            get { return (Segment)GetValue(HighlightedSegmentProperty); }
            set { SetValue(HighlightedSegmentProperty, value); }
        }

        /// <summary>
        /// Identifies the HighlightedSegment dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightedDataProperty =
            DependencyProperty.Register("HighlightedData", typeof(ChartDataPoint), typeof(ChartSeries), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets a value for HighlightedData. This is a dependency property.
        /// </summary>
        /// <value>The HighlightedData.</value>
        public ChartDataPoint HighlightedData
        {
            get { return (ChartDataPoint)GetValue(HighlightedDataProperty); }
            set { SetValue(HighlightedDataProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedSegment dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedSegmentProperty =
            DependencyProperty.Register("SelectedSegment", typeof(Segment), typeof(ChartSeries), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets a value for SelectedSegment. This is a dependency property.
        /// </summary>
        /// <value>The SelectedSegment.</value>
        public Segment SelectedSegment
        {
            get { return (Segment)GetValue(SelectedSegmentProperty); }
            set { SetValue(SelectedSegmentProperty, value); }
        }

        /// <summary>
        /// Identifies the HighlightedSegment dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDataProperty =
            DependencyProperty.Register("SelectedData", typeof(ChartDataPoint), typeof(ChartSeries), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets a value for SelectedData. This is a dependency property.
        /// </summary>
        /// <value>The SelectedData.</value>
        public ChartDataPoint SelectedData
        {
            get { return (ChartDataPoint)GetValue(SelectedDataProperty); }
            set { SetValue(SelectedDataProperty, value); }
        }
        
        /// <summary>
        /// Identifies the AllowSegmentHighlight  dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowSegmentHighlightProperty =
            DependencyProperty.Register("AllowSegmentHighlight", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value for AllowSegmentHighlight . This is a dependency property.
        /// </summary>
        /// <value>The AllowSegmentHighlight .</value>
        public bool AllowSegmentHighlight 
        {
            get { return (bool)GetValue(AllowSegmentHighlightProperty); }
            set { SetValue(AllowSegmentHighlightProperty, value); }
        }

        /// <summary>
        /// Identifies the AllowSegmentSelection dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowSegmentSelectionProperty =
            DependencyProperty.Register("AllowSegmentSelection", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value for AllowSegmentSelection. This is a dependency property.
        /// </summary>
        /// <value>The AllowSegmentSelection.</value>
        public bool AllowSegmentSelection
        {
            get { return (bool)GetValue(AllowSegmentSelectionProperty); }
            set { SetValue(AllowSegmentSelectionProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedSegmentInterior  dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedSegmentInteriorProperty =
            DependencyProperty.Register("SelectedSegmentInterior ", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnSegmentInteriorChanged)));

        /// <summary>
        /// Gets or sets a value for SelectedSegmentInterior . This is a dependency property.
        /// </summary>
        /// <value>The SelectedSegmentInterior .</value>
        public Brush SelectedSegmentInterior 
        {
            get { return (Brush)GetValue(SelectedSegmentInteriorProperty); }
            set { SetValue(SelectedSegmentInteriorProperty, value); }
        }

        /// <summary>
        /// Identifies the HighlightedSegmentInterior  dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightedSegmentInteriorProperty =
            DependencyProperty.Register("HighlightedSegmentInterior", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Occurs when left mouse button is pressed over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseLeftButtonDown;

        /// <summary>
        /// Occurs when any mouse button is clicked while pointer is over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseMove;

        /// <summary>
        /// Occurs when mouse pointer enters the bounds of the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseEnter;

        /// <summary>
        /// Occurs when segment gets highlighted.
        /// </summary>
        public  event ChartSegmentEventHandler SegmentHighlightChanged;

        /// <summary>
        /// Occurs when segment gets selected.
        /// </summary>
        public  event ChartSegmentEventHandler SegmentSelectionChanged;

        /// <summary>
        /// Occurs when mouse pointer leaves the bounds of the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseLeave;

        /// <summary>
        /// Occurs when left mouse button is released over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseLeftButtonUp;

        /// <summary>
        /// Delegate implemetation for ChartMouseEventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ChartMouseEventHandler(object sender, ChartMouseEventArgs e);
        /// <summary>
        /// Delegate implemetation for ChartSegmentEventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ChartSegmentEventHandler(object sender, ChartSegmentEventArgs e);

        
        /// <summary>
        /// Raises MouseLeftButtonDown event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseLeftButtonDown(object sender, ChartMouseEventArgs args)
        {
            if (MouseLeftButtonDown != null)
            {
                Segment segment = args.Segment;
                foreach (Segment seg in segment.Series.Segments)
                {
                    seg.IsSelected = false;
                }

                segment.IsSelected = true;
                MouseLeftButtonDown(sender, args);
            }

            if (this.Area.AllowSegmentDragDrop)
            {
                this.Area.dragSegment = args.Segment;                               
            }

            if (this.AllowSegmentSelection && (this.Type != ChartTypes.Area && this.Type != ChartTypes.Radar&& this.Type != ChartTypes.Polar && this.Type != ChartTypes.SplineArea && this.Type != ChartTypes.StepArea && this.Type != ChartTypes.StackingArea && this.Type != ChartTypes.FastLine && this.Type != ChartTypes.FastScatter && this.Type != ChartTypes.FastColumn && this.Type != ChartTypes.ThreeLineBreak && this.Type != ChartTypes.SplineArea && this.Type != ChartTypes.StackingArea && this.Type != ChartTypes.StepArea))
            {                
                Segment segment = args.Segment;
                if (this.OldSelectedSegment != segment)
                {
                    if (this.OldSelectedSegment != null)
                    {
                        this.OldSelectedSegment.Interior = this.OldSelectedSegment.SelectedSegmentInterior;
                        this.OldSelectedSegment.IsSegmentSelected = false;
                    }
                    segment.SelectedSegmentInterior = segment.IsSegmentHighlightedOnMouseMove? segment.HighlightedSegmentInterior:segment.Interior;
                    segment.Interior = this.SelectedSegmentInterior;
                    this.SelectedSegment = segment;
                    this.SelectedData = segment.DataPoint;
                    segment.IsSegmentSelected = true;
                    this.HighlightedSegment = null;
                    this.HighlightedData = null;
                    OnSegmentSelectionChanged(sender, new ChartSegmentEventArgs(segment, this.OldSelectedSegment));                                        
                }
            }
        }

        /// <summary>
        /// Raises MouseHover event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseMove(object sender, ChartMouseEventArgs args)
        {
            if (MouseMove != null)
            {
                MouseMove(sender, args);
            }

            if (this.Type == ChartTypes.FastColumn || this.Type == ChartTypes.FastScatter || this.Type == ChartTypes.FastLine)
            {
                currSegment = GetFastTypeTooltipSegment(args.MouseEventArgs.GetPosition(this.seriesGrid));
            }
        }

        ScatterSegment tooltipTempsegment = null;
        ScatterSegment GetFastTypeTooltipSegment(Point point)
        {
            double X = Math.Round(this.Area.PointToValue(this.XAxis, point), 0);
            double Y = Math.Round(this.Area.PointToValue(this.YAxis, point), 0);
            ChartPoint chartPoint = this.Data.FirstOrDefault(res => res.X == X);
            //if (tooltipTempsegment == null)
            //{
            if(chartPoint!=null)
                tooltipTempsegment = new ScatterSegment(chartPoint, chartPoint, this);
            //}
            //else
            //{
            //}

            return tooltipTempsegment;
        }
        internal Segment OldHighlightedSegment = null;

        internal Segment OldSelectedSegment = null;
        /// <summary>
        /// Raises MouseEnter event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseEnter(object sender, ChartMouseEventArgs args)
        {
            this.currSegment = args.Segment;
            this.Highlighted = true;
            if (MouseEnter != null)
            {
                args.Segment.Highlighted = true;
                MouseEnter(sender, args);
            }

            if (args.Segment.MouseAnimation != null && this.EnableAnimation && this.AnimateOption == AnimationOptions.Interactive && this.Type != ChartTypes.Pie && this.Type != ChartTypes.Doughnut && this.Type != ChartTypes.Pyramid && this.Type != ChartTypes.Funnel)
            {
                args.Segment.MouseAnimation.Begin();
            }
            if ((this.AllowSegmentHighlight && !this.currSegment.IsSegmentSelected) && (this.Type != ChartTypes.Area && this.Type != ChartTypes.Radar && this.Type != ChartTypes.Polar && this.Type != ChartTypes.SplineArea && this.Type != ChartTypes.StepArea && this.Type != ChartTypes.StackingArea && this.Type != ChartTypes.FastLine && this.Type != ChartTypes.FastScatter && this.Type != ChartTypes.FastColumn && this.Type != ChartTypes.ThreeLineBreak && this.Type != ChartTypes.SplineArea && this.Type != ChartTypes.StackingArea && this.Type != ChartTypes.StepArea))
            {
                this.currSegment.HighlightedSegmentInterior = this.currSegment.Interior;
                this.currSegment.Interior = this.HighlightedSegmentInterior;
                this.HighlightedSegment = this.currSegment;
                this.HighlightedData = this.currSegment.DataPoint;
                this.currSegment.IsSegmentHighlightedOnMouseMove = true;
                OnSegmentHighlightChanged(sender, new ChartSegmentEventArgs(this.HighlightedSegment, this.OldHighlightedSegment));                 
            }
        }
        internal void OnSegmentSelectionChanged(object sender, ChartSegmentEventArgs args)
        {
            if (SegmentSelectionChanged != null)
            {
                SegmentSelectionChanged(sender, args);
            }
        }
        internal void OnSegmentHighlightChanged(object sender, ChartSegmentEventArgs args)
        {
            if (SegmentHighlightChanged != null)
            {
                SegmentHighlightChanged(sender, args);
            }
        }

        /// <summary>
        /// Raises MouseLeave event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseLeave(object sender, ChartMouseEventArgs args)
        {
            this.Highlighted = false;
            if (MouseLeave != null)
            {
                args.Segment.Highlighted = false;
                MouseLeave(sender, args);
            }
            if ((this.Type != ChartTypes.Area && this.Type != ChartTypes.Radar && this.Type != ChartTypes.Polar && this.Type != ChartTypes.SplineArea && this.Type != ChartTypes.StepArea && this.Type != ChartTypes.StackingArea && this.Type != ChartTypes.FastLine && this.Type != ChartTypes.FastScatter && this.Type != ChartTypes.FastColumn && this.Type != ChartTypes.ThreeLineBreak && this.Type != ChartTypes.SplineArea && this.Type != ChartTypes.StackingArea && this.Type != ChartTypes.StepArea))
            {
                Segment segment = args.Segment;                
                if (segment.HighlightedSegmentInterior != null && !segment.IsSegmentSelected)
                {                    
                    segment.Interior = segment.HighlightedSegmentInterior;
                    segment.HighlightedSegmentInterior = null;
                    this.currSegment.IsSegmentHighlightedOnMouseMove = false;
                    this.OldHighlightedSegment = this.HighlightedSegment;
                }

                segment.HighlightedSegmentInterior = null;
                this.HighlightedSegment = null;
                this.HighlightedData = null;
            }
            Segment argsSegment = args.Segment;
            if (args.Segment != null && argsSegment.IsSegmentSelected)
            {
                this.OldSelectedSegment = argsSegment;
            }
        }

        /// <summary>
        /// Raises MouseLeftButtonUp event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseLeftButtonUp(object sender, ChartMouseEventArgs args)
        {
            if (MouseLeftButtonUp != null)
            {
                MouseLeftButtonUp(sender, args);
            }

            if (args.Segment.MouseAnimation != null && this.EnableAnimation && this.AnimateOption == AnimationOptions.Interactive)
            {
                args.Segment.MouseAnimation.Begin();
            }
            Segment argsSegment = args.Segment;
            if (args.Segment != null && argsSegment.IsSegmentSelected)
            {
                this.OldSelectedSegment = args.Segment;
            }
        }

        internal ChartAnimation Animation
        {
            get;
            set;
        }

        /// <summary>
        /// Idenfities AnimationDuration dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan), typeof(ChartSeries), new PropertyMetadata(new TimeSpan(0, 0, 2), new PropertyChangedCallback(OnEaseAnimationChanged)));

        /// <summary>
        /// Gets or sets the AnimationDuration. This is dependency property.
        /// </summary>
        /// <value>The Timespan.</value>
        public TimeSpan AnimationDuration
        {
            get { return (TimeSpan)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        /// <summary>
        /// Idenfities AnimateOneByOneProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimateOneByOneProperty =
            DependencyProperty.Register("AnimateOneByOne", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnEaseAnimationChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether the AnimateOneByOne. This is dependency property.
        /// </summary>
        /// <value>The bool value.</value>
        public bool AnimateOneByOne
        {
            get { return (bool)GetValue(AnimateOneByOneProperty); }
            set { SetValue(AnimateOneByOneProperty, value); }
        }

        /// <summary>
        /// Idenfities AnimateOption dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimateOptionProperty =
            DependencyProperty.Register("AnimateOption", typeof(AnimationOptions), typeof(ChartSeries), new PropertyMetadata(AnimationOptions.Top, new PropertyChangedCallback(OnEaseAnimationChanged)));

        /// <summary>
        /// Gets or sets the AnimateOption. This is dependency property.
        /// </summary>
        /// <value>The AnimationDirection.</value>
        public AnimationOptions AnimateOption
        {
            get { return (AnimationOptions)GetValue(AnimateOptionProperty); }
            set { SetValue(AnimateOptionProperty, value); }
        }

        /// <summary>
        /// Idenfities EasingFunctionProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnEaseAnimationChanged)));

        /// <summary>
        /// Gets or sets the EasingFunction. This is dependency property.
        /// </summary>
        /// <value>The EaseAnimation.</value>
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        /// <summary>
        /// Idenfities EnableAnimation dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableAnimationChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether EnableAnimation. This is dependency property.
        /// </summary>
        /// <value>The bool value.</value>
        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        /// <summary>
        /// Idenfities AdditionalEffectVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty AdditionalEffectVisibilityProperty =
            DependencyProperty.Register("AdditionalEffectVisibility", typeof(Visibility), typeof(ChartSeries), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the AdditionalEffectVisibility. This is dependency property.
        /// </summary>
        /// <value>The visibility.</value>
        internal Visibility AdditionalEffectVisibility
        {
            get { return (Visibility)GetValue(AdditionalEffectVisibilityProperty); }
            set { SetValue(AdditionalEffectVisibilityProperty, value); }
        }

        /// <summary>
        /// Idenfities EnableEffects dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableEffectsProperty =
            DependencyProperty.Register("EnableEffects", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableEffectsChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether EnableEffects. This is dependency property.
        /// </summary>
        /// <value>The bool value.</value>
        public bool EnableEffects
        {
            get { return (bool)GetValue(EnableEffectsProperty); }
            set { SetValue(EnableEffectsProperty, value); }
        }

        /// <summary>
        /// Idenfities Interior dependency property.
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
            DependencyProperty.Register("Interior", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnInteriorChanged)));

        /// <summary>
        /// Gets or sets the Interior. This is dependency property.
        /// </summary>
        /// <value>The template.</value>
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

        /// <summary>
        /// Idenfities PaletteInterior dependency property.
        /// </summary>
        internal static readonly DependencyProperty PaletteInteriorProperty =
            DependencyProperty.Register("PaletteInterior", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the PaletteInterior. This is dependency property.
        /// </summary>
        /// <value>The Brush value.</value>
        internal Brush PaletteInterior
        {
            get { return (Brush)GetValue(PaletteInteriorProperty); }
            set { SetValue(PaletteInteriorProperty, value); }
        }

        /// <summary>
        /// Identifies the ColorEach dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorEachProperty =
            DependencyProperty.Register("ColorEach", typeof(bool?), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies whether each data point of a series is shown in a different color. 
        /// </summary>
        public bool? ColorEach
        {
            get
            {
                return (bool?)GetValue(ColorEachProperty);
            }
            set
            {
                SetValue(ColorEachProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ColorEachPalette dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register("Palette", typeof(ChartColorPalette), typeof(ChartSeries), new PropertyMetadata(ChartColorPalette.Default, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies the palette to apply for the data points of series
        /// </summary>
        public ChartColorPalette Palette
        {
            get
            {
                return (ChartColorPalette)GetValue(PaletteProperty);
            }
            set
            {
                SetValue(PaletteProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ColorEachCustomPalette dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomPaletteProperty =
            DependencyProperty.Register("CustomPalette", typeof(Brush[]), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies the custom palette to apply for the data points of series
        /// </summary>
        public Brush[] CustomPalette
        {
            get
            {
                return (Brush[])GetValue(CustomPaletteProperty);
            }
            set
            {
                SetValue(CustomPaletteProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ColorEachStrokePalette dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokePaletteProperty =
            DependencyProperty.Register("StrokePalette", typeof(ChartColorPalette), typeof(ChartSeries), new PropertyMetadata(ChartColorPalette.Default, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies the stroke palette to apply for the data points of series
        /// </summary>
        public ChartColorPalette StrokePalette
        {
            get
            {
                return (ChartColorPalette)GetValue(StrokePaletteProperty);
            }
            set
            {
                SetValue(StrokePaletteProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ColorEachCustomStrokePalette dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomStrokePaletteProperty =
            DependencyProperty.Register("CustomStrokePalette", typeof(Brush[]), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies the custom stroke palette to apply for the data points of series
        /// </summary>
        public Brush[] CustomStrokePalette
        {
            get
            {
                return (Brush[])GetValue(CustomStrokePaletteProperty);
            }
            set
            {
                SetValue(CustomStrokePaletteProperty, value);
            }
        }

        internal bool ColorEachDependent
        {
            get
            {
                return !(Type == ChartTypes.FastColumn || Type == ChartTypes.SplineArea || Type == ChartTypes.StackingArea ||
                         Type == ChartTypes.FastLine || Type == ChartTypes.FastScatter || Type == ChartTypes.RangeArea ||
                         Type == ChartTypes.Kagi || Type == ChartTypes.PointAndFigure || Type == ChartTypes.ThreeLineBreak ||
                         Type == ChartTypes.Area || Type == ChartTypes.StepArea || Type == ChartTypes.Polar || Type == ChartTypes.Radar);
            }
        }

        /// <summary>
        /// Identifies the AdornmentsInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty AdornmentsInfoProperty =
          DependencyProperty.Register("AdornmentsInfo", typeof(ChartAdornmentInfo), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnAdornmentsInfoChanged)));

        /// <summary>
        /// Gets or sets the adornments info. Property is intended to customize adornments
        /// on series. This is a dependency property.
        /// </summary>
        public ChartAdornmentInfo AdornmentsInfo
        {
            get
            {
                return (ChartAdornmentInfo)GetValue(AdornmentsInfoProperty);
            }

            set
            {
                SetValue(AdornmentsInfoProperty, value);
            }
        }

        ///<Summary>
        ///Identifies the Annotation Dependency Property
        ///</Summary>
        public static readonly DependencyProperty AnnotationsProperty = DependencyProperty.Register("Annotations", typeof(AnnotationsCollection), typeof(ChartSeries), new PropertyMetadata(null));
        /// <summary>
        /// Get or Set AnnotationProperty
        /// </summary>
        public AnnotationsCollection Annotations
        {
            get { return (AnnotationsCollection)GetValue(AnnotationsProperty); }
            set { SetValue(AnnotationsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ActualOpacityOnZoom.
        /// </summary>
        /// <value>The Opacity.</value>
        internal double ActualOpacityOnZoom
        {
            get
            {
                return (double)m_actualopacityonzoom;
            }

            set
            {
                m_actualopacityonzoom = value;
            }
        }

        /// <summary>
        /// Idenfities ActualSegmentTempalte dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualSegmentTemplateProperty =
            DependencyProperty.Register("ActualSegmentTemplate", typeof(DataTemplate), typeof(ChartSeries), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ActualSegmentTempalte. This is dependency property.
        /// </summary>
        /// <value>The template.</value>
        public DataTemplate ActualSegmentTemplate
        {
            get { return (DataTemplate)GetValue(ActualSegmentTemplateProperty); }
            set { SetValue(ActualSegmentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the InactiveSeriesOpacityOnZoom dependency property.
        /// </summary>
        public static readonly DependencyProperty InactiveSeriesOpacityOnZoomProperty =
            DependencyProperty.Register("InactiveSeriesOpacityOnZoom", typeof(double), typeof(ChartSeries), new PropertyMetadata(0.25d));

        /// <summary>
        /// Gets or sets the InactiveSeriesOpacityOnZoom. This is dependency property.
        /// </summary>
        /// <value>The Opacity.</value>
        public double InactiveSeriesOpacityOnZoom
        {
            get
            {
                return (double)GetValue(InactiveSeriesOpacityOnZoomProperty);
            }

            set
            {
                SetValue(InactiveSeriesOpacityOnZoomProperty, value);
            }
        }

        /// <summary>
        /// Idenfities IsZoomable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsZoomableProperty =
            DependencyProperty.Register("IsZoomable", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnIsZoomableChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether IsZoomable. This is dependency property.
        /// </summary>
        /// <value>The true of false.</value>
        public bool IsZoomable
        {
            get
            {
                return (bool)GetValue(IsZoomableProperty);
            }

            set
            {
                SetValue(IsZoomableProperty, value);
            }
        }

        /// <summary>
        /// Identifies Area Dependency Property
        /// </summary>
        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.Register("Area", typeof(ChartArea), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnAreaChanged)));

        /// <summary>
        /// Idenfities BindingPathsY dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingPathsYProperty =
DependencyProperty.Register("BindingPathsY", typeof(List<string>), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnBindingPathsYChanged)));

        /// <summary>
        /// Idenfities BindingPathX dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingPathXProperty =
    DependencyProperty.Register("BindingPathX", typeof(string), typeof(ChartSeries), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnBindingPathXChanged)));

        /// <summary>
        /// Idenfities ChartType dependency property.
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
                    DependencyProperty.Register("Type", typeof(ChartTypes), typeof(ChartSeries), new PropertyMetadata(ChartTypes.Column, OnChartTypePropertyChanged));

        /// <summary>
        /// Idenfities Data dependency property.
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(ChartPointsCollection), typeof(ChartSeries), new PropertyMetadata(null, OnDataPropertyChanged));

        /// <summary>
        /// Idenfities DataSource dependency property.
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty =
    DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnDataSourceChanged)));

        /// <summary>
        /// Idenfities IsIndexed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsIndexedProperty =
DependencyProperty.Register("IsIndexed", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnIsIndexedChanged)));

        /// <summary>
        /// Idenfities LegendLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnLegendLabelChanged)));


       
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSortData.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSortDataProperty =
            DependencyProperty.Register("IsSortData", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false,new PropertyChangedCallback(OnIsSortDataChanged)));


        
       /// <summary>
        /// Using a DependencyProperty as the backing store for SortingDirection.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty SortingDirectionProperty =
            DependencyProperty.Register("SortingDirection", typeof(Direction), typeof(ChartSeries), new PropertyMetadata(Direction.Ascending, new PropertyChangedCallback(OnIsSortDataChanged)));
                
        
      /// <summary>
        ///  Using a DependencyProperty as the backing store for SortBy.  This enables animation, styling, binding, etc...
      /// </summary>
        public static readonly DependencyProperty SortByProperty =
            DependencyProperty.Register("SortBy", typeof(SortingAxis), typeof(ChartSeries), new PropertyMetadata(SortingAxis.X, new PropertyChangedCallback(OnIsSortDataChanged)));

        

        internal ChartPointsCollection IndexActualData
        {
            get;
            set;
        }

        private ChartType m_type = new ChartColumnType();
        internal ChartPointsCollection StackedData
        {
            get;
            set;
        }

        internal DoubleRange XCidsRange
        {
            get;
            set;
        }

        internal DoubleRange YCidsRange
        {
            get;
            set;
        }

        internal SeriesPresenter Presenter
        {
            get;
            set;
        }

        /// <summary>
        /// Idenfities Segments dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentsProperty =
            DependencyProperty.Register("Segments", typeof(SegmentsCollection), typeof(ChartSeries), new PropertyMetadata(null));

        /// <summary>
        /// Idenfities Data dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentTemplateProperty =
            DependencyProperty.Register("SegmentTemplate", typeof(DataTemplate), typeof(ChartSeries), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Stroke dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnInteriorChanged)));

        /// <summary>
        /// Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
          DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ChartSeries), new PropertyMetadata(1d));

        /// <summary>
        /// Idenfities XAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnXAxisChanged)));

        /// <summary>
        /// Idenfities YAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
           DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnYAxisChanged)));
        /// <summary>
        /// Identifies the Indicators dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorsProperty =
          DependencyProperty.Register("Indicators", typeof(IndicatorCollection), typeof(ChartSeries), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set Indicators property
        /// </summary>
        public IndicatorCollection Indicators
        {
            get { return (IndicatorCollection)GetValue(IndicatorsProperty); }
            set { SetValue(IndicatorsProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see>
        ///                                       <cref>Series</cref>
        ///                                   </see>
        ///     class.
        /// </summary>
        public ChartSeries()
        {
            DefaultStyleKey = typeof(ChartSeries);
            this.Data = new ChartPointsCollection();
            this.Data.series = this;
            this.Interior = null;
            this.Segments = new SegmentsCollection();
            this.IndexActualData = new ChartPointsCollection();
            this.StackedData = new ChartPointsCollection();
            this.XCidsRange = DoubleRange.Empty;
            this.YCidsRange = DoubleRange.Empty;
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.OldAnimatedValues = new Dictionary<object, object>();
            this.Segments = new SegmentsCollection();
            //InteractiveCursor = new InteractiveCursor();
            this.Loaded += new RoutedEventHandler(ChartSeries_Loaded);
            BindingDataModel();
            this.Data.CollectionChanged += new NotifyCollectionChangedEventHandler(Data_CollectionChanged);
            this.DataModel.ChartPoints.CollectionChanged += new NotifyCollectionChangedEventHandler(ChartPoints_CollectionChanged);
        }

        void ChartPoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Area != null)
            {
                this.Area.LoadArea();
            }
        }

        void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Area != null)
            {
                //this.Area.LoadArea();
            }
        }

        /// <summary>
        /// Method implentation for Binding set to Dependency properties
        /// </summary>
        public void BindingDataModel()
        {
            //Binding datasourceBinding = new Binding("Source") { Source = this.DataModel, Mode = BindingMode.TwoWay };
            //BindingOperations.SetBinding(this, ChartSeries.DataSourceProperty, datasourceBinding);

            Binding pathXBinding = new Binding("PathX") { Source = this.DataModel, Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(this, ChartSeries.BindingPathXProperty, pathXBinding);

            Binding pathsYBinding = new Binding("PathsY") { Source = this.DataModel, Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(this, ChartSeries.BindingPathsYProperty, pathsYBinding);

            Binding indexedBinding = new Binding("IsIndexed") { Source = this.DataModel, Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(this, ChartSeries.IsIndexedProperty, indexedBinding);

            this.DataModel.ChartPoints.series = this;
        }

        void ChartSeries_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Area != null)
            {
                if (Presenter != null && this.Area.Axes.Count > 0 && this.Area.Host==Host.OLAPChart)
                {
                    ////this.CalculateSegments();
                    if (!this.callCalculateSegmentsWhenPresenterIsSet && this.Equals(this.Area.Series[this.Area.Series.Count - 1]))
                    {
                        this.Area.LoadArea();
                    }
                }

                if (this.Equals(this.Area.Series[0]) == true)
                {
                    Binding areaTypeBinding = new Binding();
                    areaTypeBinding.Source = this;
                    areaTypeBinding.Path = new PropertyPath("Type");
                    areaTypeBinding.Converter = new AreaTypeConverter();
                    areaTypeBinding.Mode = System.Windows.Data.BindingMode.OneWay;
                    areaTypeBinding.ConverterParameter = this.Area;
                    BindingOperations.SetBinding(this.Area, ChartArea.AreaTypeProperty, areaTypeBinding);
                }

                if (this.AdornmentsInfo != null && this.Segments != null && this.Segments.Count > 0 && this.AdornmentsInfo.SymbolInterior== new SolidColorBrush(Colors.Transparent))
                {
                    Binding SymbolInteriorBinding = new Binding() { Source = this.Segments[0], Path = new PropertyPath("Interior") };
                    BindingOperations.SetBinding(this.AdornmentsInfo, ChartAdornmentInfo.SymbolInteriorProperty, SymbolInteriorBinding);


                    Binding SymbolStrokeBinding = new Binding() { Source = this, Path = new PropertyPath("Stroke") };
                    BindingOperations.SetBinding(this.AdornmentsInfo, ChartAdornmentInfo.SymbolStrokeProperty, SymbolStrokeBinding);

                    Binding SymbolStrokeThicknessBinding = new Binding() { Source = this, Path = new PropertyPath("StrokeThickness") };
                    BindingOperations.SetBinding(this.AdornmentsInfo, ChartAdornmentInfo.SymbolStrokeThicknessProperty, SymbolStrokeThicknessBinding);

                }
                else 
                {                  

                    if (this.AdornmentsInfo != null && this.AdornmentsInfo.SymbolInterior== new SolidColorBrush(Colors.Transparent))
                    {
                        int count = 0;


                        Brush[] colors = null;
                        foreach (var ser in this.Area.Series)
                        {
                            if (count == 0)
                                colors = this.Area.ColorModel.GetBrushes(this.Area.ColorModel.Palette);
                            colors[count] = ser.Interior;
                            count++;
                        }
                        count = this.Area.Series.IndexOf(this) % 7;
                        this.Interior = colors[count];
                        Binding SymbolInteriorBinding = new Binding() { Source = this, Path = new PropertyPath("Interior") };

                        BindingOperations.SetBinding(this.AdornmentsInfo, ChartAdornmentInfo.SymbolInteriorProperty, SymbolInteriorBinding);


                        Binding SymbolStrokeBinding = new Binding() { Source = this, Path = new PropertyPath("Stroke") };

                        BindingOperations.SetBinding(this.AdornmentsInfo, ChartAdornmentInfo.SymbolStrokeProperty, SymbolStrokeBinding);

                        Binding SymbolStrokeThicknessBinding = new Binding() { Source = this, Path = new PropertyPath("StrokeThickness") };

                        BindingOperations.SetBinding(this.AdornmentsInfo, ChartAdornmentInfo.SymbolStrokeThicknessProperty, SymbolStrokeThicknessBinding);
                    }
                }
            }

            if (InteractiveCursor != null)
            {
                DrawCursorLines();
            }
        }

        /// <summary>
        /// Gets or sets the Area property. This is dependency property
        /// </summary>
        public ChartArea Area
        {
            get { return (ChartArea)GetValue(AreaProperty); }
            set { SetValue(AreaProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Stroke.
        /// </summary>
        /// <value>The Stroke.</value>
        public Brush Stroke
        {
            get
            {
                return (Brush)GetValue(StrokeProperty);
            }

            set
            {
                SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the StrokeThickness.
        /// </summary>
        /// <value>The StrokeThickness.</value>
        public double StrokeThickness
        {
            get
            {
                return (double)GetValue(StrokeThicknessProperty);
            }

            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Get or Set BindingPathY property
        /// </summary>
        [TypeConverter(typeof(ChartPathsConverter))]
        public List<string> BindingPathsY
        {
            get
            {
                return (List<string>)GetValue(BindingPathsYProperty);
            }
            
            set
            {
                SetValue(BindingPathsYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the BindingPathX.
        /// </summary>
        /// <value>The BindingPathX.</value>
        public string BindingPathX
        {
            get
            {
                return (string)GetValue(BindingPathXProperty);
            }
            
            set
            {
                SetValue(BindingPathXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the chart.
        /// </summary>
        /// <value>The type of the chart.</value>
        public ChartTypes Type
        {
            get { return (ChartTypes)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the data property. This is dependency property.
        /// </summary>
        /// <value>The data points for series.</value>
        public ChartPointsCollection Data
        {
            get { return (ChartPointsCollection)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataSource.
        /// </summary>
        /// <value>The DataSource value.</value>
        public IEnumerable DataSource
        {
            get
            {
                return (IEnumerable)GetValue(DataSourceProperty);
            }
            
            set
            {
                SetValue(DataSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsIndexed.
        /// </summary>
        /// <value>The IsIndexed value.</value>
        public bool IsIndexed
        {
            get
            {
                return (bool)GetValue(IsIndexedProperty);
            }
            
            set
            {
                SetValue(IsIndexedProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the IsSortData.
        /// </summary>
        public bool IsSortData
        {
            get { return (bool)GetValue(IsSortDataProperty); }
            set { SetValue(IsSortDataProperty, value); }
        }


        /// <summary>
        /// Gets os Sets the Sorting Direction
        /// </summary>
        public Direction SortingDirection
        {
            get { return (Direction)GetValue(SortingDirectionProperty); }
            set { SetValue(SortingDirectionProperty, value); }
        }


        /// <summary>
        /// Gets os Sets the Sorting Axis
        /// </summary>
        public SortingAxis SortBy
        {
            get { return (SortingAxis)GetValue(SortByProperty); }
            set { SetValue(SortByProperty, value); }
        }

        /// <summary>
        /// Gets or sets the LegendLabel.
        /// </summary>
        /// <value>The LegendLabel value.</value>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }
            
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segments collection. This is dependency property.
        /// </summary>
        /// <value>The segments.</value>
        public SegmentsCollection Segments
        {
            get { return (SegmentsCollection)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the segments template. This is dependency property.
        /// </summary>
        /// <value>The template.</value>
        public DataTemplate SegmentTemplate
        {
            get { return (DataTemplate)GetValue(SegmentTemplateProperty); }
            set { SetValue(SegmentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the X Axis property. This is dependency property
        /// </summary>
        public ChartAxis XAxis
        {
            get
            {
                return (ChartAxis)GetValue(XAxisProperty); 
            }

            set 
            {
              SetValue(XAxisProperty, value); 
            }
        }

        /// <summary>
        /// Gets the X range.
        /// </summary>
        /// <value>The X range.</value>
        public DoubleRange XRange
        {
            get
            {
                return XCidsRange;
            }
        }

        /// <summary>
        /// Gets or sets the Y Axis property. This is dependency property
        /// </summary>
        public ChartAxis YAxis
        {
            get {return (ChartAxis)GetValue(YAxisProperty);}
            set {SetValue(YAxisProperty, value);}
        }

        /// <summary>
        /// Gets the Y range.
        /// </summary>
        /// <value>The Y range.</value>
        public DoubleRange YRange
        {
            get
            {
                return YCidsRange;
            }
        }

        /// <summary>
        /// Find the data values from the Binding Data from DataSource, BindingPathX, and BindingPathY
        /// </summary>
        public void BindDataFromDataSource()
        {
            List<string> pathy = BindingPathsY;
            if (DataSource != null && BindingPathX != null && pathy != null)
            {
                if (BindingPathsY.Count > 0)
                {
                    this.isdatasourcedata = true;
                  //  this.Data = DataBinding.GetDataBinding(DataSource, BindingPathX, BindingPathsY);
                    if (this.Data != null)
                        this.Data.series = this;
                }
            }
            else if (BindingPathX != null && this.Type == ChartTypes.Histogram)
            {
                this.isdatasourcedata = true;
                this.Data = DataBinding.GetXBindingData(DataSource, BindingPathX);
                this.Data.series = this;
            }
        }

        /// <summary>
        /// Calculate the Chart types segments and render chart
        /// </summary>
        internal void CalculateSegments()
        {
            this.m_type = KnownType(this.Type, this);
            if (this.m_type == null || this.Area == null)
            {
                return;
            }

            this.m_type.Calculate(this);
            Presenter = this.GetTemplateChild("SeriesPresenter_Part") as SeriesPresenter;
            if (Presenter != null)
            {
                if (this.EnableAnimation == true)
                {
                    if (this.Animation == null)
                    {
                        this.Animation = new ChartAnimation(this);
                    }
                    else
                    {
                        this.Animation.Storyboard.Stop();
                        this.Animation.Storyboard.Children.Clear();
                        this.Animation.StopAnimation();
                    }
                }

                Presenter.LoadSegments();
                if (this.Area != null)
                {
                    ChartSeries[] seriesdata = (from s in this.Area.Series where ((ChartSeries)s).Visibility == Visibility.Visible select s).ToArray<ChartSeries>();
                    if (seriesdata.Length != 0 && seriesdata[seriesdata.Length - 1].Presenter != null)
                    {
                        seriesdata[seriesdata.Length - 1].Presenter.LoadAdornments(this.Area.Series);
                    }
                }
            }
            else
                callCalculateSegmentsWhenPresenterIsSet = true;

            if (this.Presenter != null && this.ColorEachDependent && this.ColorEach != null)
            {
                this.UpdateColorEachSegments();
            }
        }

        bool callCalculateSegmentsWhenPresenterIsSet = false;
        /// <summary>
        /// Convert datapoint into the Actual Chart point.
        /// </summary>
        /// <returns>
        /// Chart Points Collection
        /// </returns>
        internal ChartPointsCollection ConvertToActualData(ChartPointsCollection datapoints)
        {
            double datacount = 1d;
            ChartPointsCollection normaldata = new ChartPointsCollection();
            foreach (ChartPoint data in datapoints)
            {
                if (this.XAxis != null && this.YAxis != null)
                {
                    if (this.XAxis.IsLogarithmic || this.YAxis.IsLogarithmic)
                    {
                        if (this.XAxis.IsLogarithmic)
                        {
                            if (data.X <= 0)
                            {
                                datacount++;
                                continue;
                            }
                        }
                        if (this.YAxis.IsLogarithmic)
                        {
                            if (data.Y <= 0)
                            {
                                datacount++;
                                continue;
                            }
                        }
                        normaldata.Add(new ChartPoint(datacount, data.Values) { Y = data.Y, Visible = data.Visible });
                        datacount++;
                    }
                    else
                    {
                        normaldata.Add(new ChartPoint(datacount, data.Values) { Y = data.Y, Visible = data.Visible });
                        datacount++;
                    }
                }
                else
                {
                    
                    normaldata.Add(new ChartPoint(datacount, data.Values) { Y = data.Y, Visible = data.Visible });
                    datacount++;
                }
            }

            return normaldata;
        }


        internal ChartPointsCollection ConvertToLogData(ChartPointsCollection datapoints)
        {
            ChartPointsCollection normaldata = new ChartPointsCollection();
            foreach (ChartPoint data in datapoints)
            {
                if (this.XAxis != null && this.YAxis != null)
                {
                    if (this.XAxis.IsLogarithmic && this.YAxis.IsLogarithmic)
                    {

                        if (data.X > 0 && data.Y > 0)
                            normaldata.Add(new ChartPoint(Math.Log(data.X, this.XAxis.LogarithmicBase), Math.Log(data.Y, this.YAxis.LogarithmicBase)));
                    }
                    else if (!this.XAxis.IsLogarithmic && !this.YAxis.IsLogarithmic)
                    {
                        normaldata.Add(data);
                    }
                    else if (this.XAxis.IsLogarithmic)
                    {
                        normaldata.Add(new ChartPoint(Math.Log(data.X, this.XAxis.LogarithmicBase), data.Y));
                    }
                    else if (this.YAxis.IsLogarithmic)
                    {
                        normaldata.Add(new ChartPoint(data.X, Math.Log(data.Y, this.YAxis.LogarithmicBase)));
                    }
                }
                else
                {
                    normaldata.Add(data);
                }
            }

            return normaldata;
        }


        /// <summary>
        /// Convert datapoint into the Actual Chart point for Y-Axis.
        /// </summary>
        /// <returns>
        /// Chart Points Collection
        /// </returns>
        internal ChartPointsCollection ConvertToActualDataYaxis(ChartPointsCollection datapoints)
        {
            double datacount = 1d;
            ChartPointsCollection normaldata = new ChartPointsCollection();
            foreach (ChartPoint data in datapoints)
            {
                normaldata.Add(new ChartPoint(data.X, datacount));
                datacount++;
            }

            return normaldata;
        }

        /// <summary>
        /// Identify the chart point when series is Indexed is set
        /// </summary>
        /// <returns>
        /// List of Objects
        /// </returns>
        internal List<object> Indexeddata(ChartPointsCollection originaldata)
        {
            List<object> idata = new List<object>();
            foreach (ChartPoint data in originaldata)
            {
                idata.Add((object)data.X);
            }

            string str = "";
            idata.Insert(0, (object)str);
            idata.Insert(idata.Count, (object)str);
            return idata;
        }

        /// <summary>
        /// Index Y-Axis contents
        /// </summary>
        /// <returns>
        /// List of Objects
        /// </returns>
        internal List<object> IndexeddataYaxis(ChartPointsCollection originaldata)
        {
            List<object> idata = new List<object>();
            foreach (ChartPoint data in originaldata)
            {
                idata.Add((object)data.Y);
            }

            string str = "";
            idata.Insert(0, (object)str);
            idata.Insert(idata.Count, (object)str);
            return idata;
        }

        internal bool IsRotated(ChartTypes types)
        {
            return types == ChartTypes.Bar || types == ChartTypes.StackingBar || types == ChartTypes.Gantt ||
                types == ChartTypes.Tornado || types == ChartTypes.StackingBar100 || types == ChartTypes.RotatedSpline;
        }

        internal bool IsRequiredYPoints()
        {
            int ypoints = 1;
            bool result = false;
            switch (this.Type)
            {
                case ChartTypes.Bubble:
                case ChartTypes.RangeColumn:
                case ChartTypes.HiLo:
                case ChartTypes.RangeArea:
                case ChartTypes.PointAndFigure:
                case ChartTypes.Gantt:
                case ChartTypes.Tornado:
                    ypoints = 2;
                    break;
                case ChartTypes.Candle:
                case ChartTypes.HiLoOpenClose:
                    ypoints = 4;
                    break;
                case ChartTypes.BoxAndWhisker:
                    ypoints = 5;
                    break;
            }

            if (this.Data.Count != 0 && this.Data[0].Values.Length >= ypoints)
            {
                result = true;
            }

            return this.Data.Count == 0 ? true : result;
        }

        /// <summary>
        /// Returns known chart type instance by passed enum ChartTypes.
        /// </summary>
        /// <param name="type"><see cref="ChartTypes"/> parameter.</param>
        /// <param name="series"></param>
        /// <returns>Returns <see cref="ChartType"/>.</returns>
        internal static ChartType KnownType(ChartTypes type, ChartSeries series)
        {
            ChartType chart;
            switch (type)
            {
                case ChartTypes.Column:
                    chart = new ChartColumnType();
                    break;
                case ChartTypes.Line:
                    chart = new ChartLineType();
                    break;
                case ChartTypes.Scatter:
                    chart = new ChartScatterType();
                    break;
                case ChartTypes.Bar:
                    chart = new ChartBarType();
                    break;
                case ChartTypes.StackingColumn:
                    chart = new ChartStackingColumnType();
                    break;
                case ChartTypes.StackingBar:
                    chart = new ChartStackingBarType();
                    break;
                case ChartTypes.Area:
                    chart = new ChartAreaType();
                    break;
                case ChartTypes.StackingArea:
                    chart = new ChartStackingAreaType();
                    break;
                case ChartTypes.StackingArea100:
                    chart = new ChartStackingArea100Type();
                    break;
                case ChartTypes.FastLine:
                    chart = new ChartFastLineType();
                    break;
                case ChartTypes.Pie:
                    chart = new ChartPieType();
                    break;
                case ChartTypes.Doughnut:
                    chart = new ChartDoughnutType();
                    break;
                case ChartTypes.Bubble:
                    chart = new ChartBubbleType();
                    break;
                case ChartTypes.HiLo:
                    chart = new ChartHiLoType();
                    break;
                case ChartTypes.HiLoOpenClose:
                    chart = new ChartHiLoOpenCloseType();
                    break;
                case ChartTypes.Gantt:
                    chart = new ChartGanttType();
                    break;
                case ChartTypes.BoxAndWhisker:
                    chart = new ChartBoxAndWhiskerType();
                    break;
                case ChartTypes.Pyramid:
                    chart = new ChartPyramidType();
                    break;
                case ChartTypes.Candle:
                    chart = new ChartCandleType();
                    break;
                case ChartTypes.RangeColumn:
                    chart = new ChartRangeColumnType();
                    break;
                case ChartTypes.RangeArea:
                    chart = new ChartRangeAreaType();
                    break;
                case ChartTypes.Tornado:
                    chart = new ChartTornadoType();
                    break;
                case ChartTypes.Funnel:
                    chart = new ChartFunnelType();
                    break;
                case ChartTypes.StepLine:
                    chart = new ChartStepLineType();
                    break;
                case ChartTypes.StepArea:
                    chart = new ChartStepAreaType();
                    break;
                case ChartTypes.StackingColumn100:
                    chart = new ChartStackingColumn100Type();
                    break;
                case ChartTypes.StackingBar100:
                    chart = new ChartStackingBar100Type();
                    break;
                case ChartTypes.Spline:
                    chart = new ChartSplineType();
                    break;
                case ChartTypes.SplineArea:
                    chart = new ChartSplineAreaType();
                    break;
                case ChartTypes.RotatedSpline:
                    chart = new ChartRotatedSplineType();
                    break;
                case ChartTypes.ThreeLineBreak:
                    chart = new ChartThreeLineBreakType();
                    break;
                case ChartTypes.Kagi:
                    chart = new ChartKagiType();
                    break;
                case ChartTypes.Renko:
                    chart = new ChartRenkoType();
                    break;
                case ChartTypes.PointAndFigure:
                    chart = new ChartPointAndFigureType();
                    break;
                case ChartTypes.Radar:
                    chart = new ChartRadarType();
                    break;
                case ChartTypes.Polar:
                    chart = new ChartPolarType();
                    break;
                case ChartTypes.Histogram:
                    chart = new ChartHistogramType();
                    break;
                case ChartTypes.FastScatter:
                    chart = new ChartFastScatterType();
                    break;
                case ChartTypes.FastColumn:
                    chart = new FastColumnType();
                    break;
                case ChartTypes.Custom:
                    chart = series.ChartType;
                    break;
                default:
                    chart = new ChartColumnType();
                    break;
            }

            return chart;
        }

        internal Grid seriesGrid = null;

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            //Grid parentGrid = VisualTreeHelper.GetParent(this) as Grid;

            //if (parentGrid != null)
            //{
            //    availableSize = new Size(parentGrid.ActualWidth, parentGrid.ActualHeight);
            //}

            if (this.Area != null && this.Area.seriesItemControl != null)
            {
                if (double.IsPositiveInfinity(availableSize.Width))
                {
                    availableSize.Width = this.Area.seriesItemControl.ActualWidth;
                }

                if (double.IsPositiveInfinity(availableSize.Height))
                {
                    availableSize.Height = this.Area.seriesItemControl.ActualHeight;
                }
            }

            return availableSize;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            //if (this.XAxis != null && this.YAxis != null)
            //{
            //    this.CalculateSegments();
            //}

            base.OnApplyTemplate();
            ToolTip tooltip = ToolTipService.GetToolTip(this) as ToolTip;
            if (tooltip != null)
                tooltip.DataContext = this;

            Presenter = GetTemplateChild("SeriesPresenter_Part") as SeriesPresenter;
            seriesGrid = GetTemplateChild("series") as Grid;

            if (this.Area != null)
            {
                if (callCalculateSegmentsWhenPresenterIsSet)
                {
                    ////CalculateSegments();
                    if (this.Equals(this.Area.Series[this.Area.Series.Count - 1]))
                    {
                        this.Area.LoadArea();
                    }
                }
                else
                {
                    if (this.Area.isAreaLoaded == true && this.Area.isUpdateArea == true)
                    {
                        if (this.Equals(this.Area.Series[this.Area.Series.Count - 1]))
                        {
                            this.Area.LoadArea();
                        }
                    }
                }

                if (this.Area.Legends != null && this.Area.Legends.Items.Count == 0)
                {
                    this.Area.Legends.InvalidateMeasure();
                }

                if (this.XAxis != null&& this.YAxis!=null)
                {
                    this.Area.isUpdateArea = false;
                    this.IsXAxisInversed = this.XAxis.IsInversed;
                    this.IsYAxisInversed = this.YAxis.IsInversed;
                }
                }
        }

        /// <summary>
        /// Executes when IsZoomable Property value changed
        /// </summary>
        private static void OnIsZoomableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.IsZoomable == true)
            {
                zoomseries = series;
                if (series.XAxis != null)
                {
                    series.XAxis.EnableZooming = true;
                    ////series.ZoomCenter(series.XAxis);
                }

                if (series.YAxis != null)
                {
                    series.YAxis.EnableZooming = true;
                    ////series.ZoomCenter(series.YAxis);
                }
            }
            else
            {
                zoomseries = null;
            }
        }

        /// <summary>
        /// Executes when series Interior changed
        /// </summary>
        private static void OnInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.ColorEach == true)
            {
                series.UpdateColorEachSegments();
            }
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Executes when series Stroke changed
        /// </summary>
        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.ColorEach == true)
            {
                series.UpdateColorEachSegments();
            }
        }


        /// <summary>
        /// Executes when ColorEach changed
        /// </summary>
        private static void OnColorEachInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;

            if (series != null && series.Presenter != null && series.ColorEachDependent && series.ColorEach != null)
            {
                series.UpdateColorEachSegments();
            }
            if (series != null && series.Area != null && series.Area.Legends != null)
            {
                series.Area.Legends.GenerateItems();
            }
        }

        internal void UpdateColorEachSegments()
        {
            Panel seriesPresenter = this.Presenter as Panel;
            if(seriesPresenter!=null)
            foreach (UIElement segmentElement in seriesPresenter.Children)
            {
                Segment segment = (segmentElement as ContentPresenter).Content as Segment;
                if (segment != null && !(segment is ChartAdornment))
                {
                    int colorIndex = segment.Series.Segments.IndexOf(segment);
                    Brush[] colors = null;
                    Brush[] strokeColors = null;
                    if ((bool)segment.Series.ColorEach)
                    {
                        ChartStyleModel model = new ChartStyleModel();
                        model.Palette = segment.Series.Palette;
                        model.CustomPalette = segment.Series.CustomPalette;
                        colors = model.GetBrushes(segment.Series.Palette);
                        segment.Interior = colors[colorIndex % colors.Length];

                        ChartStyleModel strokeModel = new ChartStyleModel();
                        strokeModel.Palette = segment.Series.StrokePalette;
                        strokeModel.CustomPalette = segment.Series.CustomStrokePalette;
                        strokeColors = strokeModel.GetBrushes(segment.Series.StrokePalette);
                        segment.Stroke = strokeColors[colorIndex % strokeColors.Length];
                    }
                    else
                    {
                        colors = segment.Series.Area.ColorModel.GetBrushes(segment.Series.Area.ColorModel.Palette);
                        segment.Interior = (segment.Series.Interior != null) ? (segment.Series.Interior) :
                                            ((colors == null || colors.Length == 0) ? new SolidColorBrush(Colors.Transparent) : colors[segment.Series.Area.Series.IndexOf(segment.Series) % colors.Length]);
                       segment.Stroke = segment.Series.Stroke;
                    }
                }
            }
            
        }

        /// <summary>
        /// Executes when series Adornments Information changed
        /// </summary>
        private static void OnAdornmentsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.AdornmentsInfo != null)
                series.AdornmentsInfo.series = series;
            ////if (series.Area != null)
            ////{
            ////    series.Area.LoadArea();
            ////}
        }

        private static void OnEnableEffectsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.Area != null)
            {
                series.Area.LoadArea();
            }
        }
        /// <summary>
        /// Method implementation for StartAnimation for ChartSeries
        /// </summary>
        public void StartAnimation()
        {
            if (this.Animation != null && this.EnableAnimation == true)
            {
                this.Animation.AnimateSeries(true);
            }
        }

        private static void OnEnableAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Presenter != null)
            {
                if (series.EnableAnimation == true)
                {
                    series.Animation = new ChartAnimation(series);
                    //series.Animation.AnimateSeries(true);
                    series.StartAnimation();
                }
                else
                {
                    series.Animation.Storyboard.Stop();
                    series.Animation.Storyboard.Children.Clear();
                    series.Animation = null;
                }
            }
        }
        
        private static void OnSegmentInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.SelectedSegment != null)
            {
                series.SelectedSegment.Interior = (Brush)e.NewValue;
            }
        }

        private static void OnEaseAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Presenter != null && series.EnableAnimation == true)
            {
                series.Animation.AnimateSeries(true);
            }
        }

        /// <summary>
        /// Executes when series corresponding area changed
        /// </summary>
        private static void OnAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                if (series.Area.Axes == null)
                {
                    series.Area.Axes = new AxesCollection();
                }
            }
        }

        /// <summary>
        /// Executes when Data property changed
        /// </summary>
        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.isdatasourcedata == true)
            {
                series.isdatasourcedata = false;
            }
            else if (series.DataSource != null)
            {
                series.isdatasourcedata = true;
                //series.Data = (ChartPointsCollection)e.OldValue;
                series.Data.series = series;
            }

            if (series.Area != null)
            {
                 if (series.XAxis != null && series.YAxis != null)
                {
                    series.XAxis.VisibleRange = DoubleRange.Empty;
                    series.YAxis.VisibleRange = DoubleRange.Empty;
                }

                ////if (series.DataSource == null)
                ////{
                //       //series.Area.LoadArea();
                ////}
            }
        }

        /// <summary>
        /// Executes when Type property changed.
        /// </summary>
        private static void OnChartTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            
            if (series != null)
            {
                series.IsRefreshAnimation = true;
                series.OldAnimatedValues.Clear();
            }

            if (series.XAxis != null && series.YAxis != null)
            {
                series.XAxis.VisibleRange = DoubleRange.Empty;
                series.YAxis.VisibleRange = DoubleRange.Empty;
            }

            if (series.Area != null && ((((ChartTypes)e.OldValue == ChartTypes.StackingColumn100 || (ChartTypes)e.OldValue == ChartTypes.StackingBar100) &&
                ((ChartTypes)e.NewValue != ChartTypes.StackingColumn100 || (ChartTypes)e.NewValue != ChartTypes.StackingBar100)) ||
                (((ChartTypes)e.NewValue == ChartTypes.StackingColumn100 || (ChartTypes)e.NewValue == ChartTypes.StackingBar100) &&
                ((ChartTypes)e.OldValue != ChartTypes.StackingColumn100 || (ChartTypes)e.OldValue != ChartTypes.StackingBar100))))
            {
                series.Area.ZoomResetCommand();
            }

            if (series.Area != null)
            {
                series.Area.LoadArea();
                if (series.Area.Legends != null)
                {
                    series.Area.Legends.InvalidateMeasure();
                }

                if (series.InteractiveCursor != null)
                {
                    series.InteractiveCursor.CursorVisibility = series.ICVisibility;
                    series.DrawCursorLines();
                }
                if (series.seriesGrid != null)
                {
                    if (series.Type == ChartTypes.Gantt || series.Type == ChartTypes.StackingBar || series.Type == ChartTypes.Bar || series.Type == ChartTypes.StackingBar100 || series.Type == ChartTypes.Tornado)
                    {
                        if (series.Area.PrimaryAxis.IsInversed && series.Area.SecondaryAxis.IsInversed)
                        {
                            series.seriesGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                            series.seriesGrid.RenderTransform = new CompositeTransform() { ScaleX = series.ScaleX, ScaleY = series.ScaleY };
                        }
                        else if (series.Area.PrimaryAxis.IsInversed || series.Area.SecondaryAxis.IsInversed)
                        {
                            series.seriesGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                            series.seriesGrid.RenderTransform = new CompositeTransform() { ScaleX = -series.ScaleX, ScaleY = -series.ScaleY };
                        }
                    }
                    else
                    {
                        series.seriesGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                        series.seriesGrid.RenderTransform = new CompositeTransform() { ScaleX = series.ScaleX, ScaleY = series.ScaleY };
                    }
                }
            }
        }

        /// <summary>
        /// Executed when BindingPathX property value changed
        /// </summary>
        private static void OnBindingPathXChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = (ChartSeries)d;
            if (series.DataSource != null)
            {
                if (series.BindingPathsY != null)
                {
                    if (!string.IsNullOrEmpty(series.BindingPathX) && series.BindingPathsY.Count > 0)
                    {
                        series.BindDataFromDataSource();
                        if (series.Area != null)
                        {
                            //series.Area.LoadArea();
                        }
                    }
                    else if (string.IsNullOrEmpty(series.BindingPathX))
                    {
                        series.Data.Clear();
                        series.Data = null;
                    }
                }
                else if (series.BindingPathX != string.Empty && series.Type == ChartTypes.Histogram)
                {                   
                    series.BindDataFromDataSource();
                    if (series.Area != null)
                    {
                        series.Area.LoadArea();
                    }
                }
                else if (string.IsNullOrEmpty(series.BindingPathX))
                {
                    series.Data.Clear();
                    series.Data = null;
                }

                if (series.XAxis != null && (series.XAxis.ContentPath==null || series.XAxis.ContentPath==string.Empty))
                {
                    series.XAxis.ContentPath = series.BindingPathX;
                    series.XAxis.PositionPath = series.BindingPathX;
                }
            }
        }

        /// <summary>
        /// Executed when DataSource property value changed
        /// </summary>
        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = (ChartSeries)d;
            if (args.NewValue != null)
            {
                if (series != null)
                {
                    series.isdatasourcedata = true;
                    series.DataModel.Source = series.DataSource;
                    if (args.OldValue == null && series.Data != null && series.Data.Count > 0)
                    {
                        throw new InvalidOperationException("Data property must be empty before using DataSource");
                    }

                    series.populatechart((IEnumerable)args.NewValue);

                    Binding datasourceBinding = new Binding("ChartPoints") { Source = series.DataModel, Mode = BindingMode.OneWay };
                    BindingOperations.SetBinding(series, ChartSeries.DataProperty, datasourceBinding);
                }

                if (series.DataSource != null)
                {
                    if (series.BindingPathsY != null)
                    {
                        if (series.BindingPathX != string.Empty && series.BindingPathsY.Count > 0)
                        {
                            series.BindDataFromDataSource();
                            if (series.XAxis != null)
                            {
                                if (series.XAxis.ActualLabelsSourceSet == false)
                                {
                                    series.XAxis.InternalLabelsSourceSet = true;
                                    series.XAxis.LabelsSource = series.DataSource;
                                }
                                else
                                {
                                    if (series.Area != null)
                                    {
                                        series.Area.LoadArea();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    series.Data = new ChartPointsCollection();
                    if (series.XAxis != null)
                    {
                        if (series.XAxis.ActualLabelsSourceSet == false)
                        {
                            series.XAxis.InternalLabelsSourceSet = true;
                            series.XAxis.LabelsSource = series.DataSource;
                        }
                    }
                }

                if (series.XAxis == null && series.Area != null)
                {
                    series.Area.LoadArea();
                }

                if (series.XAxis != null && series.YAxis != null && series.Area == null)
                {
                    series.CalculateSegments();
                }
            }
        }

        /// <summary>
        /// used to populate chart when collection changed
        /// </summary>
        private IEnumerable SeriesColl;
        private void populatechart(IEnumerable newvalue)
        {
            if (newvalue is INotifyCollectionChanged)
            {
                SeriesColl = newvalue;
                ((INotifyCollectionChanged)SeriesColl).CollectionChanged += new NotifyCollectionChangedEventHandler(Series_CollectionChanged);
            }
        }

        /// <summary>
        /// Executed when Series property value changed
        /// </summary>
        void Series_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.BindDataFromDataSource();
            //this.Area.LoadArea();
        }

        /// <summary>
        /// Executed when LegendLabel property value changed
        /// </summary>
        private static void OnLegendLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.YAxis !=null)
            {
                if (!series.YAxis.axisBindedSeriesList.Contains(series))
                    series.YAxis.axisBindedSeriesList.Add(series);

                if (series.XAxis != null && series.Area==null)
                {
                    series.CalculateSegments();
                }

                if (series.seriesGrid != null)
                {
                    series.IsYAxisInversed = series.XAxis.IsInversed;
                }
            }
        }

        /// <summary>
        /// Executed when X-Axis property value changed
        /// </summary>
        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.XAxis !=null)
            {
                Type type = DataBinding.GetPropertyType(series.DataSource, series.BindingPathX);
                if (type == typeof(string))
                {
                    series.XAxis.ActualValueType = ChartValueType.String;
                }
                else if (type == typeof(DateTime))
                {
                    series.XAxis.ActualValueType = ChartValueType.DateTime;
                }

                if (series.DataSource != null && series.BindingPathX!=null)
                {
                    if (series.XAxis.LabelsSource == null)
                    {
                        if (series.XAxis.ActualLabelsSourceSet == false)
                        {
                            series.XAxis.InternalLabelsSourceSet = true;
                            series.XAxis.LabelsSource = series.DataSource;
                        }
                    }

                    if (series.XAxis.ContentPath == null)
                    {
                        series.XAxis.ContentPath = series.BindingPathX;
                    }

                    if (series.XAxis.PositionPath == null)
                    {
                        series.XAxis.PositionPath = series.BindingPathX;
                    }
                }

                if (!series.XAxis.axisBindedSeriesList.Contains(series))
                {
                    if (series.DataModel != null)
                    {
                        series.DataModel.ContentPath = series.XAxis.ContentPath;
                        series.DataModel.PositionPath = series.XAxis.PositionPath;
                        series.DataModel.RefreshAxisPositionPath(series.XAxis.PositionPath);
                        series.XAxis.AxisDataContents = series.DataModel.GetAxisContents;
                        series.XAxis.AxisDataPosition = series.DataModel.GetAxisPositions;
                        series.XAxis.SeriesData = series.Data;
                        if (series.XAxis.AxisDataContents != null)
                        {
                            series.XAxis.AxisDataContents.Insert(0, "");
                        }
                    }
                    series.XAxis.axisBindedSeriesList.Add(series);
                }

                if (series.YAxis != null && series.Area==null)
                {
                    series.CalculateSegments();
                }

                if (series.seriesGrid != null)
                {
                    series.IsXAxisInversed = series.XAxis.IsInversed;
                }
            }
        }

        /// <summary>
        /// Used to perform the Zooming functionalities in chart
        /// </summary>
        internal void ZoomCenter(ChartAxis axis)
        {
            if (axis.EnableZooming == true || this.IsZoomable == true)
            {
                ZoomingScrollBar zoomscrollbar = null;
                if (this.Area.HorizontalBar != null && axis.Orientation == Orientation.Horizontal)
                {
                    zoomscrollbar = this.Area.HorizontalBar;
                }
                else if (this.Area.VerticalBar != null && axis.Orientation == Orientation.Vertical)
                {
                    zoomscrollbar = this.Area.VerticalBar;
                }

                //if (axis.Orientation == Orientation.Horizontal)
                //{
                //    if (axis.ZoomFactor < 1.0)
                //    {
                //        this.Area.HorizontalScrollBarVisibility = Visibility.Visible;
                //    }
                //    else
                //    {
                //        this.Area.HorizontalScrollBarVisibility = Visibility.Collapsed;
                //    }
                //}
                //else if (axis.Orientation == Orientation.Vertical)
                //{
                //    if (axis.ZoomFactor < 1.0)
                //    {
                //        this.Area.VerticalScrollBarVisibility = Visibility.Visible;
                //    }
                //    else
                //    {
                //        this.Area.VerticalScrollBarVisibility = Visibility.Collapsed;
                //    }
                //}

                //if (axis.isUpdateZoomrange == true)
                //{
                    //if (axis.IgnoreRangePaddingsOnZoom == false)
                    //{
                    //    axis.ZoomRange = new DoubleRange(axis.Range.Start, axis.Range.End);
                    //}
                    //else
                    //{
                    //    axis.ZoomRange = new DoubleRange(axis.Range.Start + axis.startpadding, axis.Range.End - axis.endpadding);
                    //}
                //}

                //if (zoomscrollbar != null )//&& axis.isUpdateScrollbar==true)
                //{
                //    if (axis.Orientation == Orientation.Vertical)
                //    {
                //        zoomscrollbar.Value = 1 - (1 / (axis.Range.End - axis.Range.Start) * (axis.ZoomPosition - axis.Range.Start));
                //    }
                //    else
                //    {
                //        zoomscrollbar.Value = 1 / (axis.Range.End - axis.Range.Start) * (axis.ZoomPosition - axis.Range.Start);
                //    }

                //    axis.isUpdateScrollbar = false;
                //}

                //axis.ZoomVisibleRange = new DoubleRange(axis.Range.Start, axis.Range.End);
                //axis.ZoomVisisbleInterval = axis.m_visibleInterval;
                //axis.IsAutoSetRange = false;
                //axis.Range = new DoubleRange(axis.ZoomVisibleRange.Start, axis.ZoomVisibleRange.End);
                //axis.isUpdateViewportsize = (axis.IsFractionEnabledOnZoom == false && axis.PreviousRange.Equals(axis.Range) == true) ? false : axis.isUpdateViewportsize;

                if (zoomscrollbar != null)
                {
                    if (axis.ZoomFactor == 1)
                    {
                        zoomscrollbar.ViewportSize = double.MaxValue;
                    }
                    else if (axis.ZoomFactor >= 0.5 && axis.isUpdateViewportsize == true)
                    {
                        ////zoomscrollbar.ViewportSize = zoomscrollbar.Maximum - zoomscrollbar.Minimum;
                        zoomscrollbar.ViewportSize = axis.ZoomFactor * 16;
                        axis.isUpdateViewportsize = false;
                    }
                    else if (axis.ZoomFactor < 0.5 && axis.isUpdateViewportsize == true)
                    {
                        zoomscrollbar.ViewportSize = axis.ZoomFactor * 4;
                        axis.isUpdateViewportsize = false;
                    }
                }
            }
        }

        /// <summary>
        /// Executed when IsIndexed property value changed
        /// </summary>
        private static void OnIsIndexedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null && series.XAxis != null && series.YAxis != null)
            {
                series.XAxis.VisibleRange = DoubleRange.Empty;
                series.YAxis.VisibleRange = DoubleRange.Empty;
                series.Area.LoadArea();
                series.XAxis.RefreshAxis();
            }
        }

        private static void OnIsSortDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.Area != null)
            {
                series.Area.LoadArea();
                series.XAxis.RefreshAxis();
            }
        }

        private static void OnSortDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = d as ChartSeries;
            if (series!= null && series.Area != null)
            {
                series.Area.LoadArea();
                series.XAxis.RefreshAxis();
            }
        }

        private static void OnSortByChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.Area != null)
            {
                series.Area.LoadArea();
                series.XAxis.RefreshAxis();
            }
        }
        /// <summary>
        /// Executed when BindingPathsY property value changed
        /// </summary>
        private static void OnBindingPathsYChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = (ChartSeries)d;
            if (series.DataSource != null)
            {
                if (series.BindingPathsY != null)
                {
                    if (series.BindingPathX != string.Empty && series.BindingPathsY.Count > 0)
                    {
                        series.BindDataFromDataSource();
                        if (series.DataSource != null && series.XAxis != null && series.XAxis.LabelsSource == null)
                        {
                            series.XAxis.InternalLabelsSourceSet = true;
                            series.XAxis.LabelsSource = series.DataSource;
                        }

                        if (series.Area != null)
                        {
                            //series.Area.LoadArea();
                        }
                    }
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //MouseLeftButtonDown=null;
            //MouseMove=null;
            //MouseEnter=null;
            //MouseLeave=null;
            //MouseLeftButtonUp = null;
            this.Loaded -= new RoutedEventHandler(ChartSeries_Loaded);
            if(this.Segments !=null)
            this.Segments.Clear();
            this.Segments = null;
            if (SeriesColl != null)
                ((INotifyCollectionChanged)SeriesColl).CollectionChanged -= new NotifyCollectionChangedEventHandler(Series_CollectionChanged);

            if (this.Template != null)
            {
                this.Template = null;
            }

            if (this.m_type != null)
            {
                m_type.Dispose();
                m_type = null;
            }
            if (this.BindingPathsY != null)
            {
                this.BindingPathsY.Clear();
                this.BindingPathsY = null;
            }
            
            //if (this._SeriesContainer != null)
            //{
            //    this._SeriesContainer.Children.Clear();
            //    this._SeriesContainer = null;
            //}

            if (this.InteractiveCursor != null)
            {
                if (this.HorizontalCursor != null)
                {
                    HorizontalCursor.MouseLeftButtonDown += new MouseButtonEventHandler(HorizontalCursor_MouseLeftButtonDown);
                    HorizontalCursor.MouseLeftButtonUp += new MouseButtonEventHandler(HorizontalCursor_MouseLeftButtonUp);

                    HorizontalCursor.MouseLeftButtonDown += new MouseButtonEventHandler(VerticalCursor_MouseLeftButtonDown);
                    HorizontalCursor.MouseLeftButtonUp += new MouseButtonEventHandler(VerticalCursor_MouseLeftButtonUp);
                }
                if (this.VerticalCursor != null)
                {
                    VerticalCursor.MouseLeftButtonDown += new MouseButtonEventHandler(VerticalCursor_MouseLeftButtonDown);
                    VerticalCursor.MouseLeftButtonUp += new MouseButtonEventHandler(VerticalCursor_MouseLeftButtonUp);

                    VerticalCursor.MouseLeftButtonDown += new MouseButtonEventHandler(HorizontalCursor_MouseLeftButtonDown);
                    VerticalCursor.MouseLeftButtonUp += new MouseButtonEventHandler(HorizontalCursor_MouseLeftButtonUp);
                }

                this.InteractiveCursor.Dispose();
                this.InteractiveCursor = null;
            }
            if (this.Segments != null)
            {
                for (int temp = 0; temp < this.Segments.Count; temp++)
                {
                    this.Segments[temp].Dispose();
                }
                this.Segments.Clear();
                Segments = null;              
            }
            if (zoomseries != null)
                zoomseries = null;
            if (m_adornments != null)
            {
                for (int temp = 0; temp < this.m_adornments.Count; temp++)
                    this.m_adornments[temp].Dispose();
                this.m_adornments.Clear();
                m_adornments = null;
            }

            if (this.Adornments != null)
            {
                for (int temp = 0; temp < this.Adornments.Count; temp++)
                    this.Adornments[temp].Dispose();
                this.Adornments.Clear();
            }

            if (this.Animation != null)
            {
                this.Animation.Dispose();
                this.Animation = null;
            }
            if (this.AdornmentsInfo != null)
            {
                this.AdornmentsInfo.Dispose();
                this.AdornmentsInfo = null;
            }
            if (this.Annotations != null)
            {
                for (int temp = 0; temp < this.Annotations.Count; temp++)
                    this.Annotations[temp].Dispose();
                this.Annotations.Clear();
                this.Annotations = null;
            }

            if (this.IndexActualData != null)
            {
                this.IndexActualData.Clear();
                this.IndexActualData.series = null;
                this.IndexActualData = null;
            }
            if (this.StackedData != null)
            {
                this.StackedData.Clear();
                this.StackedData.series = null;
                this.StackedData = null;
            }

            if (this.Area != null)
            {
                this.Area.ClearValue(ChartArea.AreaTypeProperty);
                this.Area = null;
            }

            this.DataSource = null;
            if (this.Data != null)
            {
                this.Data.Clear();
                this.Data.series = null;
                this.Data = null;
            }

            if (this.XAxis != null)
                this.XAxis = null;
            if (this.YAxis != null)
                this.YAxis = null;

            if (m_type != null)
            {
                m_type = null;
            }

            if (this.Presenter != null)
            {
                this.Presenter.Dispose();
                this.Presenter = null;
            }
     
            this.ClearValue(ChartSeries.DataSourceProperty);
            this.ClearValue(ChartSeries.AnimateOptionProperty);
            this.ClearValue(ChartSeries.AdditionalEffectVisibilityProperty);
            this.ClearValue(ChartSeries.EnableEffectsProperty);
            this.ClearValue(ChartSeries.TypeProperty);
            this.ClearValue(ChartSeries.BindingPathsYProperty);
            this.ClearValue(ChartSeries.SegmentsProperty);

            this.Resources.Clear();
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Determine the Visibility for the chart series Effects
    /// </summary>
    public class EffectsVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)values;
            ChartSeries series = (ChartSeries)parameter;
            return val && isApplyeffect(series.Type) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }

        internal bool isApplyeffect(ChartTypes type)
        {
            bool result = false;
            switch (type)
            {
                case ChartTypes.Column:
                case ChartTypes.StackingColumn:
                case ChartTypes.Bar:
                case ChartTypes.StackingBar:
                case ChartTypes.Line:
                case ChartTypes.Scatter:
                case ChartTypes.Bubble:
                case ChartTypes.Pie:
                case ChartTypes.Doughnut:
                    result = true;
                    break;
            }

            return result;
        }
    }


    /// <summary>
    /// Determine the Effects transform for the chart series Effects
    /// </summary>
    /// 
    public class EffectsConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double myvalues = (Double)value;
            myvalues = myvalues - 5;
            return myvalues;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Represents chart mouse click event arguments.
    /// </summary>
    public class ChartMouseEventArgs
    {
        #region Members
        /// <summary>
        /// Initializes m_segment
        /// </summary>
        private Segment m_segment;

        /// <summary>
        /// Initializes m_mouseArgs
        /// </summary>
        private MouseEventArgs m_mouseArgs;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartMouseEventArgs"/> class.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="segment">The segment.</param>
        public ChartMouseEventArgs(MouseEventArgs args, Segment segment)
        {
            m_mouseArgs = args;
            m_segment = segment;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the segment.
        /// </summary>
        /// <value>The segment.</value>
        public Segment Segment
        {
            get
            {
                return m_segment;
            }
        }

        /// <summary>
        /// Gets the mouse event args.
        /// </summary>
        /// <value>The mouse event args.</value>
        public MouseEventArgs MouseEventArgs
        {
            get
            {
                return m_mouseArgs;
            }
        }
        #endregion
    }



    /// <summary>
    /// Class implementation for ChartSegmentEventArgs
    /// </summary>
    public class ChartSegmentEventArgs
    {
        #region Members
        /// <summary>
        /// Initializes m_segment
        /// </summary>
        private Segment m_oldsegment;

        private Segment m_newsegment;
        
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartMouseEventArgs"/> class.
        /// </summary>
        /// <param name="newsegment">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="oldsegment">The segment.</param>
        public ChartSegmentEventArgs(Segment newsegment, Segment oldsegment)
        {            
            m_oldsegment = oldsegment;
            m_newsegment = newsegment;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the segment.
        /// </summary>
        /// <value>The segment.</value>
        public Segment OldSegment
        {
            get
            {
                return m_oldsegment;
            }
        }

        /// <summary>
        /// Get or Set NewSegment property
        /// </summary>
        public Segment NewSegment
        {
            get
            {
                return m_newsegment;
            }
        }
        
        #endregion
    }
}
