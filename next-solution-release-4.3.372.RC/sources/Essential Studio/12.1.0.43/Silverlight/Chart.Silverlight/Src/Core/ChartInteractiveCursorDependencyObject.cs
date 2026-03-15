#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implmentation for interactiveCursor.
    /// </summary>
    public class InteractiveCursor : Control, IDisposable
    {
        /// <summary>
        ///  Identifies the CursorVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty CursorVisibilityProperty =
            DependencyProperty.Register("CursorVisibility", typeof(Visibility), typeof(InteractiveCursor),
            new PropertyMetadata(Visibility.Visible,new PropertyChangedCallback(VisibilityChanged)));
        /// <summary>
        /// Called when Cursor Visibility property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        private static void VisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            InteractiveCursor ic = (InteractiveCursor)d;
            //if (ic.chartseries != null)
            //{
            //    ic.chartseries.ICVisibility = ic.CursorVisibility;
            //}
        }
        /// <summary>
        /// Get or Set CursorVisibility property
        /// </summary>
        public Visibility CursorVisibility
        {
            get { return (Visibility)GetValue(CursorVisibilityProperty); }
            set { SetValue(CursorVisibilityProperty, value); }
        }
        /// <summary>
        ///  Identifies the CursorStrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty CursorStrokeThicknessProperty =
            DependencyProperty.Register("CursorStrokeThickness", typeof(double), typeof(InteractiveCursor),
            new PropertyMetadata(2.0));
        /// <summary>
        /// Get or Set cursorStrokeThickness property
        /// </summary>
        public double CursorStrokeThickness
        {
            get { return (double)GetValue(CursorStrokeThicknessProperty); }
            set { SetValue(CursorStrokeThicknessProperty, value); }
        }
        /// <summary>
        ///  Identifies the VerticalcursorInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalCursorInteriorProperty =
            DependencyProperty.Register("VerticalCursorInterior", typeof(Brush), typeof(InteractiveCursor),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        /// <summary>
        /// Get or Set VerticalCursorInterior property
        /// </summary>
        public Brush VerticalCursorInterior
        {
            get { return (Brush)GetValue(VerticalCursorInteriorProperty); }
            set { SetValue(VerticalCursorInteriorProperty, value); }
        }
        /// <summary>
        ///  Identifies the HorizontalCursorInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalCursorInteriorProperty =
            DependencyProperty.Register("HorizontalCursorInterior", typeof(Brush), typeof(InteractiveCursor),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        /// <summary>
        /// Get or Set HorizontalCursorInterior property
        /// </summary>
        public Brush HorizontalCursorInterior
        {
            get { return (Brush)GetValue(HorizontalCursorInteriorProperty); }
            set { SetValue(HorizontalCursorInteriorProperty, value); }
        }
        /// <summary>
        ///  Identifies the HorizontalCursorTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalCursorTemplateProperty =
            DependencyProperty.Register("HorizontalCursorTemplate", typeof(DataTemplate), typeof(InteractiveCursor),
            new PropertyMetadata(null));
        /// <summary>
        /// get or Set HorizontalCursorTemplate property
        /// </summary>
        public DataTemplate HorizontalCursorTemplate
        {
            get { return (DataTemplate)GetValue(HorizontalCursorTemplateProperty); }
            set { SetValue(HorizontalCursorTemplateProperty, value); }
        }
        /// <summary>
        ///  Identifies the HorizontalCursorVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalCursorVisibilityProperty =
            DependencyProperty.Register("HorizontalCursorVisibility", typeof(Visibility), typeof(InteractiveCursor),
            new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Get or Set HorizontalCursorVisibility property
        /// </summary>
        public Visibility HorizontalCursorVisibility
        {
            get { return (Visibility)GetValue(HorizontalCursorVisibilityProperty); }
            set { SetValue(HorizontalCursorVisibilityProperty, value); }
        }
        /// <summary>
        ///  Identifies the VerticalCursorVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalCursorVisibilityProperty =
            DependencyProperty.Register("VerticalCursorVisibility", typeof(Visibility), typeof(InteractiveCursor),
            new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Get or Set VerticalCursorVisibility
        /// </summary>
        public Visibility VerticalCursorVisibility
        {
            get { return (Visibility)GetValue(VerticalCursorVisibilityProperty); }
            set { SetValue(VerticalCursorVisibilityProperty, value); }
        }        
        /// <summary>
        ///  Identifies the VerticalCursorTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalCursorTemplateProperty =
            DependencyProperty.Register("VerticalCursorTemplate", typeof(DataTemplate), typeof(InteractiveCursor),
            new PropertyMetadata(null));
        /// <summary>
        /// Get or Set VerticalcursorTemplate
        /// </summary>
        public DataTemplate VerticalCursorTemplate
        {
            get { return (DataTemplate)GetValue(VerticalCursorTemplateProperty); }
            set { SetValue(VerticalCursorTemplateProperty, value); }
        }
        /// <summary>
        ///  Identifies the IsbindWithSegment dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBindWithSegmentProperty =
            DependencyProperty.Register("IsBindWithSegment", typeof(bool), typeof(InteractiveCursor),
            new PropertyMetadata(false, new PropertyChangedCallback(OnValueChanged)));
        /// <summary>
        /// Get or Set IsBindWithSegment property
        /// </summary>
        public bool IsBindWithSegment
        {
            get { return (bool)GetValue(IsBindWithSegmentProperty); }
            set { SetValue(IsBindWithSegmentProperty, value); }
        }
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            InteractiveCursor ic = (InteractiveCursor)d;
            if (ic != null && ic.Area != null && args.NewValue.ToString() == "True")
            {
                foreach (ChartSeries series in ic.Area.Series)
                {
                    int i = 0; double prevX = 0d;
                    ic.Selectedseries = series;
                    foreach (ChartPoint pt in series.Data)
                    {
                        double x = ic.Area.PointToValue(series.XAxis, new Point(ic.XPosition, ic.YPosition));
                        double y = ic.Area.PointToValue(series.YAxis, new Point(ic.XPosition, ic.YPosition));

                        if (x >= prevX && x <= pt.X)
                        {
                            double xpoint = ic.Area.ValueToPoint(series.XAxis, pt.X);
                            double ypoint = ic.Area.ValueToPoint(series.YAxis, pt.Y);

                            ic.XPosition = xpoint;
                            ic.YPosition = ypoint;
                            foreach (ChartAxis axis in ic.Area.Axes)
                            {
                                if (axis.Orientation == Orientation.Horizontal)
                                {
                                    axis.HorizontalLableMargin = ic.XPosition;
                                    axis.VerticalLableMargin = 0d;
                                    axis.LabelContent = new ChartPoint(Math.Round(axis.Area.PointToValue(axis, new Point(ic.XPosition, ic.YPosition)), 2), y);
                                    axis.CursorLabelTemplate = axis.HorizontalLabelTemplate;
                                    axis.CursorLabelVisibility = axis.HorizontalLabelVisibility;
                                }
                                else
                                {
                                    axis.VerticalLableMargin = ic.YPosition;
                                    axis.HorizontalLableMargin = 0d;
                                    axis.LabelContent = new ChartPoint(x, Math.Round(axis.Area.PointToValue(axis, new Point(ic.XPosition, ic.YPosition)), 2));
                                    axis.CursorLabelTemplate = axis.VerticalLabelTemplate;
                                    axis.CursorLabelVisibility = axis.VerticalLabelVisibility;
                                }
                            }
                            break;
                        }
                        else if (x <= prevX && ic.XPosition >= 0)
                        {
                            double xpoint = ic.Area.ValueToPoint(series.XAxis, pt.X);
                            double ypoint = ic.Area.ValueToPoint(series.YAxis, pt.Y);

                            ic.XPosition = xpoint;
                            ic.YPosition = ypoint;
                            foreach (ChartAxis axis in ic.Area.Axes)
                            {
                                if (axis.Orientation == Orientation.Horizontal)
                                {
                                    axis.HorizontalLableMargin = ic.XPosition;
                                    axis.VerticalLableMargin = 0d;
                                    axis.LabelContent = new ChartPoint(Math.Round(axis.Area.PointToValue(axis, new Point(ic.XPosition, ic.YPosition)), 2), y);
                                    axis.CursorLabelTemplate = axis.HorizontalLabelTemplate;
                                    axis.CursorLabelVisibility = axis.HorizontalLabelVisibility;
                                }
                                else
                                {
                                    axis.VerticalLableMargin = ic.YPosition;
                                    axis.HorizontalLableMargin = 0d;
                                    axis.LabelContent = new ChartPoint(x, Math.Round(axis.Area.PointToValue(axis, new Point(ic.XPosition, ic.YPosition)), 2));
                                    axis.CursorLabelTemplate = axis.VerticalLabelTemplate;
                                    axis.CursorLabelVisibility = axis.VerticalLabelVisibility;
                                }
                            }
                            break;
                        }

                        prevX = pt.X;
                        i = i + 1;
                    }
                }
            }
            else if (args.NewValue.ToString() == "false")
            {
                ic.Selectedseries = null;
            }
        }
        /// <summary>
        /// Identifies the BindWithMouseMoveOnSegment dependency property.
        /// </summary>
        public static readonly DependencyProperty BindWithMouseMoveOnSegmentProperty =
           DependencyProperty.Register("BindWithMouseMoveOnSegment", typeof(bool), typeof(InteractiveCursor),
           new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));
        /// <summary>
        /// Get or Set BindWithMouseMoveOnSegmentProperty
        /// </summary>
        public bool BindWithMouseMoveOnSegment
        {
            get { return (bool)GetValue(BindWithMouseMoveOnSegmentProperty); }
            set { SetValue(BindWithMouseMoveOnSegmentProperty, value); }
        }
        /// <summary>
        ///  Identifies the IsBindWithMouseMove dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBindWithMouseMoveProperty =
           DependencyProperty.Register("IsBindWithMouseMove", typeof(bool), typeof(InteractiveCursor),
           new PropertyMetadata(false));
        /// <summary>
        /// Get or Set IsBindWithMouseMoveProperty
        /// </summary>
        public bool IsBindWithMouseMove
        {
            get { return (bool)GetValue(IsBindWithMouseMoveProperty); }
            set { SetValue(IsBindWithMouseMoveProperty, value); }
        }
        /// <summary>
        ///  Identifies the XPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty XPositionProperty =
            DependencyProperty.Register("XPosition", typeof(double), typeof(InteractiveCursor),
            new PropertyMetadata(1.0, new PropertyChangedCallback(XPositionChanged)));
        /// <summary>
        /// Get or Set Xposition property
        /// </summary>
        public double XPosition
        {
            get { return (double)GetValue(XPositionProperty); }
            set { SetValue(XPositionProperty, value); }
        }
        private static void XPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            InteractiveCursor ic = (InteractiveCursor)d;
        }
        /// <summary>
        ///  Identifies the YPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty YPositionProperty =
            DependencyProperty.Register("YPosition", typeof(double), typeof(InteractiveCursor),
            new PropertyMetadata(1.0, new PropertyChangedCallback(YPositionChanged)));
        /// <summary>
        /// Get or Set YPosition
        /// </summary>
        public double YPosition
        {
            get { return (double)GetValue(YPositionProperty); }
            set { SetValue(YPositionProperty, value); }
        }
        /// <summary>
        ///  Identifies the Height dependency property.
        /// </summary>
        public new static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(InteractiveCursor),
            new PropertyMetadata(0d));
        /// <summary>
        /// Get or Set Height property
        /// </summary>
        public new double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
        /// <summary>
        ///  Identifies the Width dependency property.
        /// </summary>
        public new static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(InteractiveCursor),
            new PropertyMetadata(0d));
        /// <summary>
        /// Get or Set Width property
        /// </summary>
        public new double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            InteractiveCursor ic = (InteractiveCursor)d;
            //if (!ic.BindWithMouseMoveOnSegment)
            //    ic.Selectedseries = null;
        }

        private static void YPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            InteractiveCursor ic = (InteractiveCursor)d;
            //if (ic.chartseries != null)
            //{
            //    if (ic.chartseries.Data.Count != 0)
            //    {
            //        if (ic.YPosition == 0)
            //            ic.YPosition = 1;
            //        else if (ic.YPosition > ic.chartseries.Data.Count)
            //            ic.YPosition = ic.chartseries.Data.Count;
            //        ic.chartseries.SetValuesForCursor();
            //    }
            //}
        }
        /// <summary>
        /// Identifies the XValue, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty XValueProperty = DependencyProperty.Register("XValue", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        /// <value>The X value.</value>
        internal double XValue
        {
            get { return (double)GetValue(XValueProperty); }
            set { SetValue(XValueProperty, value); }
        }      

        /// <summary>
        /// Identifies the YValue, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty YValueProperty = DependencyProperty.Register("YValue", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the Y value.
        /// </summary>
        /// <value>The Y value.</value>
        internal double YValue
        {
            get { return (double)GetValue(YValueProperty); }
            set { SetValue(YValueProperty, value); }
        }        

        private ChartSeries series;
        /// <summary>
        /// Gets or sets the chartseries.
        /// </summary>
        /// <value>The chartseries.</value>        
        public ChartSeries Selectedseries
        {
            get { return series; }
            set { series = value; }
        }

        private ChartArea cs;
        internal ChartArea Area
        {
            set { cs = value; }
            get { return cs; }
        }

        #region constructor
        /// <summary>
        /// Called when instance created for Interactive cursor
        /// </summary>
        public InteractiveCursor()
        {
            DefaultStyleKey = typeof(InteractiveCursor);
            this.Loaded += new RoutedEventHandler(InteractiveCursor_Loaded);
        }

        void InteractiveCursor_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Area != null)
            {
                this.Area.MouseLeftButtonDown += new MouseButtonEventHandler(Area_MouseLeftButtonDown);
                this.Area.MouseMove += new MouseEventHandler(Area_MouseMove);
                this.Area.MouseLeftButtonUp += new MouseButtonEventHandler(Area_MouseLeftButtonUp);

                if (Area.seriesGrid != null)
                {
                    this.Area.seriesGrid.MouseLeave += new MouseEventHandler(seriesGrid_MouseLeave);
                }
            }
        }

        void seriesGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            m_IsPixelCursor = false;
            m_IsSegmentCursor = false;
            //if (!this.BindWithMouseMoveOnSegment)
            //    this.Selectedseries = null;
            IsCursorSelected = false;
        }
        private bool m_IsPixelCursor = false;
        private bool m_IsSegmentCursor = false;
        void Area_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_IsPixelCursor = false;
            m_IsSegmentCursor = false;
            IsCursorSelected = false;
            (sender as ChartArea).Cursor = Cursors.Arrow;
            //if (!this.BindWithMouseMoveOnSegment)
            //    this.Selectedseries = null;
        }

        void Area_MouseMove(object sender, MouseEventArgs e)
        {
            ChartArea area = sender as ChartArea;

            if (area != null && area.seriesGrid != null)
            {
                if ((sender as ChartArea).seriesGrid != null)
                {
                    Point pt = e.GetPosition((sender as ChartArea).seriesGrid);
                    if ((pt.X >= (this.XPosition - 1) && pt.X <= (this.XPosition + 1)) || (pt.Y >= (this.YPosition - 1) && pt.Y <= (this.YPosition + 1)))
                        this.Cursor = Cursors.Hand;
                }
                if (m_IsPixelCursor)
                {
                    Point pt = e.GetPosition(area.seriesGrid);
                    if (pt.X >= 0 && pt.X < area.seriesGrid.ActualWidth)
                        this.XPosition = pt.X;
                    if (pt.Y >= 0 && pt.Y <= area.seriesGrid.ActualHeight)
                        this.YPosition = pt.Y;
                                       
                    foreach (ChartAxis axis in area.Axes)
                    {
                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            axis.HorizontalLableMargin = pt.X;
                            axis.VerticalLableMargin = 0d;
                            axis.LabelContent = new ChartPoint(Math.Round(this.XPosition, 0), Math.Round(this.YPosition, 0));
                            axis.CursorLabelTemplate = axis.HorizontalLabelTemplate;
                            axis.CursorLabelVisibility = axis.HorizontalLabelVisibility;
                        }
                        else
                        {
                            axis.VerticalLableMargin = pt.Y;
                            axis.HorizontalLableMargin = 0d;
                            axis.LabelContent = new ChartPoint(Math.Round(this.XPosition, 0), Math.Round(this.YPosition, 0));
                            axis.CursorLabelTemplate = axis.VerticalLabelTemplate;
                            axis.CursorLabelVisibility = axis.VerticalLabelVisibility;
                        }
                    }
                }

                if (m_IsSegmentCursor)
                {
                    CursorWithSegment(area, area.seriesGrid, e.GetPosition(area));
                }

                if (this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment)
                {
                    SetValueToCursor(area, area.seriesGrid, e.GetPosition(area.seriesGrid));
                }

                if (this.IsBindWithMouseMove && this.IsBindWithSegment == false)
                {
                    Point pt = e.GetPosition(area.seriesGrid);
                    if (pt.X >= 0 && pt.X < area.seriesGrid.ActualWidth)
                        this.XPosition = pt.X;
                    if (pt.Y >= 0 && pt.Y <= area.seriesGrid.ActualHeight)
                        this.YPosition = pt.Y;

                    foreach (ChartAxis axis in area.Axes)
                    {
                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            axis.HorizontalLableMargin = pt.X;
                            axis.VerticalLableMargin = 0d;
                            axis.LabelContent = new ChartPoint(Math.Round(this.XPosition, 0), Math.Round(this.YPosition, 0));
                            axis.CursorLabelTemplate = axis.HorizontalLabelTemplate;
                            axis.CursorLabelVisibility = axis.HorizontalLabelVisibility;
                        }
                        else
                        {
                            axis.VerticalLableMargin = pt.Y;
                            axis.HorizontalLableMargin = 0d;
                            axis.LabelContent = new ChartPoint(Math.Round(this.XPosition, 0), Math.Round(this.YPosition, 0));
                            axis.CursorLabelTemplate = axis.VerticalLabelTemplate;
                            axis.CursorLabelVisibility = axis.VerticalLabelVisibility;
                        }
                    }
                }
            }
        }

        void CursorWithSegment(ChartArea chartarea, Grid seriesgrid, Point pt)
        {
            pt = new Point(pt.X - chartarea.AxesThickness.Left, pt.Y - chartarea.AxesThickness.Bottom);            
            foreach (ChartSeries series in chartarea.Series)
            {
                ChartPointsCollection Datas = new ChartPointsCollection();
                foreach (ChartPoint point in Selectedseries.Data)
                {
                    if (point.X >= series.XAxis.ActualVisibleRange.Start && point.X <= series.XAxis.ActualVisibleRange.End)
                    {
                        Datas.Add(point);
                    }
                }
                foreach (ChartPoint point in Datas)
                {
                   
                    {
                        #region RotatedChartTypes
                        if (series.Type == ChartTypes.Bar ||
                                    series.Type == ChartTypes.RotatedSpline ||
                                    series.Type == ChartTypes.StackingBar ||
                                    series.Type == ChartTypes.StackingBar100 ||
                                    series.Type == ChartTypes.Tornado ||
                                    series.Type == ChartTypes.Gantt)
                        {

                            double X = chartarea.ValueToPoint(series.XAxis, point.X);
                            double Y = chartarea.ValueToPoint(series.YAxis, point.Y);
                            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
                            //double start = chartarea.ValueToPoint(series.XAxis, point.YDataMeasure.Start);
                            //double end = chartarea.ValueToPoint(series.XAxis, segment.YDataMeasure.End);

                            double Ystart = chartarea.ValueToPoint(series.YAxis, point.Y - 0.5);
                            double Yend = chartarea.ValueToPoint(series.YAxis, point.Y + 0.5);

                            double prevX = 0; double prevY = 0;
                            if (series.Data.IndexOf(point) > 0)
                            {
                                int index = series.Data.IndexOf(point);
                                prevX = chartarea.ValueToPoint(series.XAxis, series.Segments[index - 1].DataPoint.X);
                                prevY = chartarea.ValueToPoint(series.YAxis, series.Segments[index - 1].DataPoint.Y);
                            }
                            if (pt.Y > 0 && pt.Y < X && series.Data.IndexOf(point) == (series.Data.Count - 1) && Selectedseries == null)
                            {
                                this.Selectedseries = series;
                                break;
                            }
                            else if (pt.Y <= Ystart && pt.Y >= Yend)
                            {
                                this.Selectedseries = series;
                                break;
                            }

                        }
                        #endregion
                        #region OtherTypes
                        else
                        {
                            double X = chartarea.ValueToPoint(series.XAxis, point.X);
                            double Y = chartarea.ValueToPoint(series.YAxis, point.Y);
                            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
                            double start = chartarea.ValueToPoint(series.XAxis, point.X - (series.XAxis.VisibleInterval / 2));
                            double end = chartarea.ValueToPoint(series.XAxis, point.X + (series.XAxis.VisibleInterval / 2));

                            double Ystart = chartarea.ValueToPoint(series.YAxis, point.Y - (series.XAxis.VisibleInterval / 2));
                            double Yend = chartarea.ValueToPoint(series.YAxis, point.Y + (series.XAxis.VisibleInterval / 2));

                            double prevX = 0; double prevY = 0;
                            if (series.Data.IndexOf(point) > 0)
                            {
                                int index = series.Data.IndexOf(point);
                                prevX = chartarea.ValueToPoint(series.XAxis, series.Data[index - 1].X);
                                prevY = chartarea.ValueToPoint(series.YAxis, series.Data[index - 1].Y);
                            }
                            if (pt.X < X && pt.Y < Y && series.Data.IndexOf(point) == 0 && Selectedseries == null)
                            {
                                this.Selectedseries = series;
                                break;
                            }
                            else if (pt.X >= start && pt.X <= end && pt.Y <= Ystart && pt.Y >= Yend)
                            {
                                this.Selectedseries = series;
                                break;
                            }
                        }
                        #endregion
                    }
                }
            }

            SetValueToCursor(chartarea, seriesgrid, pt);

        }

        void SetValueToCursor(ChartArea chartarea, Grid seriesgrid, Point pt)
        {
            if (Selectedseries != null)
            {
                ChartPointsCollection Datas = new ChartPointsCollection();
                foreach (ChartPoint point in Selectedseries.Data)
                {
                    if (point.X >= series.XAxis.ActualVisibleRange.Start && point.X <= series.XAxis.ActualVisibleRange.End)
                    {
                        Datas.Add(point);
                    }
                }
                foreach (ChartPoint point in Datas)
                {                    
                    {
                        if (series.Type == ChartTypes.Bar ||
                                    series.Type == ChartTypes.RotatedSpline ||
                                    series.Type == ChartTypes.StackingBar ||
                                    series.Type == ChartTypes.StackingBar100 ||
                                    series.Type == ChartTypes.Tornado ||
                                    series.Type == ChartTypes.Gantt)
                        {
                            #region RotatedChartTypes

                            double X = chartarea.ValueToPoint(series.XAxis, point.X);
                            double Y = chartarea.ValueToPoint(series.YAxis, point.Y);
                            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
                            
                            double start = point.X - (series.XAxis.VisibleInterval / 2);
                            double end = point.X + (series.XAxis.VisibleInterval / 2);

                            double x = chartarea.PointToValue(series.XAxis, pt);
                            double prevX = 0; double prevY = 0;
                            if (series.Data.IndexOf(point) > 0)
                            {
                                int index = series.Data.IndexOf(point);
                                prevX = chartarea.ValueToPoint(series.XAxis, series.Segments[index - 1].DataPoint.X);
                                prevY = chartarea.ValueToPoint(series.YAxis, series.Segments[index - 1].DataPoint.Y);
                            }
                            if (pt.X < X && series.Data.IndexOf(point) == series.Segments.Count - 1)
                            {
                                this.Selectedseries = series;
                                ChartPoint point1 = new ChartPoint(Math.Round(point.X, 2), Math.Round(point.Y, 2));                            
                            }
                            else if (x >= start && x<= end)
                            {
                                this.Selectedseries = series;
                                this.YPosition = X;
                                this.XPosition = Y;
                                
                                foreach (ChartAxis axis in chartarea.Axes)
                                {
                                    if (axis.Orientation == Orientation.Horizontal)
                                    {
                                        axis.HorizontalLableMargin = X;
                                        axis.VerticalLableMargin = 0d;
                                        axis.LabelContent = new ChartPoint(Math.Round(axis.Area.PointToValue(axis, new Point(X, Y)), 2), point.Y);
                                        axis.CursorLabelTemplate = axis.HorizontalLabelTemplate;
                                        axis.CursorLabelVisibility = axis.HorizontalLabelVisibility;
                                    }
                                    else
                                    {
                                        axis.VerticalLableMargin = Y;
                                        axis.HorizontalLableMargin = 0d;
                                        axis.LabelContent = new ChartPoint(point.X, Math.Round(axis.Area.PointToValue(axis, new Point(X, Y)), 2));
                                        axis.CursorLabelTemplate = axis.VerticalLabelTemplate;
                                        axis.CursorLabelVisibility = axis.VerticalLabelVisibility;
                                    }
                                }
                            }                            
                            #endregion
                        }
                        else
                        {
                            #region OtherTypes
                            double X = chartarea.ValueToPoint(series.XAxis, point.X);
                            double Y = chartarea.ValueToPoint(series.YAxis, point.Y);
                            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
                            double start = chartarea.ValueToPoint(series.XAxis, point.X - (double.IsNaN(sbsInfo.Start) ? (series.XAxis.VisibleInterval / 2) : sbsInfo.Start));
                            double end = chartarea.ValueToPoint(series.XAxis, point.X + (double.IsNaN(sbsInfo.End) ? series.XAxis.VisibleInterval / 2 : sbsInfo.End));
                                                        
                            double prevX = 0; double prevY = 0;
                            if (series.Data.IndexOf(point) > 0)
                            {
                                int index = series.Data.IndexOf(point);
                                prevX = chartarea.ValueToPoint(series.XAxis, series.Data[index - 1].X);
                                prevY = chartarea.ValueToPoint(series.YAxis, series.Data[index - 1].Y);
                            }
                            if (pt.X < X && series.Data.IndexOf(point) == 0)
                            {
                                this.Selectedseries = series;
                                this.XPosition = X;
                                this.YPosition = Y;
                                foreach (ChartAxis axis in chartarea.Axes)
                                {
                                    if (axis.Orientation == Orientation.Horizontal)
                                    {
                                        axis.HorizontalLableMargin = X;
                                        axis.VerticalLableMargin = 0d;
                                        axis.LabelContent = new ChartPoint(Math.Round(axis.Area.PointToValue(axis, new Point(X, Y)), 2), point.Y);
                                        axis.CursorLabelTemplate = axis.HorizontalLabelTemplate;
                                        axis.CursorLabelVisibility = axis.HorizontalLabelVisibility;
                                    }
                                    else
                                    {
                                        axis.VerticalLableMargin = Y;
                                        axis.HorizontalLableMargin = 0d;
                                        axis.LabelContent = new ChartPoint(point.X, Math.Round(axis.Area.PointToValue(axis, new Point(X, Y)), 2));
                                        axis.CursorLabelTemplate = axis.VerticalLabelTemplate;
                                        axis.CursorLabelVisibility = axis.VerticalLabelVisibility;
                                    }
                                }
                            }
                            else if (pt.X >= start && pt.X <= end)
                            {
                                this.Selectedseries = series;
                                this.XPosition = X;
                                this.YPosition = Y;
                                
                                foreach (ChartAxis axis in chartarea.Axes)
                                {
                                    if (axis.Orientation == Orientation.Horizontal)
                                    {
                                        axis.HorizontalLableMargin = X;
                                        axis.VerticalLableMargin = 0d;
                                        axis.LabelContent = new ChartPoint(Math.Round(axis.Area.PointToValue(axis, new Point(X, Y)),2), point.Y);
                                        axis.CursorLabelTemplate = axis.HorizontalLabelTemplate;
                                        axis.CursorLabelVisibility = axis.HorizontalLabelVisibility;
                                    }
                                    else
                                    {
                                        axis.VerticalLableMargin = Y;
                                        axis.HorizontalLableMargin = 0d;
                                        axis.LabelContent = new ChartPoint(point.X, Math.Round(axis.Area.PointToValue(axis, new Point(X,Y)),2));
                                        axis.CursorLabelTemplate = axis.VerticalLabelTemplate;
                                        axis.CursorLabelVisibility = axis.VerticalLabelVisibility;
                                    }
                                }
                            }
                            
                            #endregion
                        }
                    }
                }
            }
        }
        void Area_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChartArea area = sender as ChartArea;

            if (area != null)
            {
                if ((sender as ChartArea).seriesGrid != null)
                {
                    Point pt = e.GetPosition((sender as ChartArea).seriesGrid);
                    if ((pt.X >= (this.XPosition - 1) && pt.X <= (this.XPosition + 1)) || ( pt.Y >= (this.YPosition-1) && pt.Y<= (this.YPosition +1)))
                    {
                        this.IsCursorSelected = true;
                        area.Cursor = Cursors.Hand;
                    }
                }
                if (IsCursorSelected)
                {
                    if (!IsBindWithSegment)
                    {
                        m_IsPixelCursor = true;
                    }
                    else
                    {
                        m_IsSegmentCursor = true;
                    }
                }
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseLeftButtonDown(e);
        }

        internal Canvas canvas = null;
        private Canvas horizontalCanvas = null;
        private Canvas verticalCanvas = null;
        private bool IsCursorSelected = false;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            canvas = this.GetTemplateChild("interactivecursor") as Canvas;
            int i = 0;
            foreach (var item in canvas.Children)
            {
                if (item is Canvas)
                {
                    if (i == 0)
                    {
                        horizontalCanvas = item as Canvas;
                        horizontalCanvas.MouseEnter += new MouseEventHandler(horizontalCanvas_MouseEnter);
                        horizontalCanvas.MouseLeave += new MouseEventHandler(horizontalCanvas_MouseLeave);
                    }
                    else
                    {
                        verticalCanvas = item as Canvas;
                        verticalCanvas.MouseEnter += new MouseEventHandler(verticalCanvas_MouseEnter);
                        verticalCanvas.MouseLeave += new MouseEventHandler(verticalCanvas_MouseLeave);
                    }
                }
            }
            base.OnApplyTemplate();
        }

        void verticalCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            IsCursorSelected = false;
            this.Cursor = Cursors.Arrow;
        }

        void verticalCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            IsCursorSelected = true;
            this.Cursor = Cursors.Hand;
        }

        void horizontalCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            IsCursorSelected = false;
            this.Cursor = Cursors.Arrow;
        }

        void horizontalCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            IsCursorSelected = true;
            this.Cursor = Cursors.Hand;
        }

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

            if (this.Area != null && this.Area.InteractiveCursorItemsControl != null)
            {
                if (double.IsPositiveInfinity(availableSize.Width))
                {
                    availableSize.Width = this.Area.InteractiveCursorItemsControl.ActualWidth;
                }

                if (double.IsPositiveInfinity(availableSize.Height))
                {
                    availableSize.Height = this.Area.InteractiveCursorItemsControl.ActualHeight;
                }
                this.Height = availableSize.Height;
                this.Width = availableSize.Width;
                //this.VerticalLableMargin = new Thickness(0, availableSize.Height, 0, 0);
                //this.HorizontalLableMargin = new Thickness(-this.Area.AxesThickness.Left, 0, 0, 0);
            }

            return availableSize;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (cs != null)
                cs = null;
        }

        #endregion
    }
}
